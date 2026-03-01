Shader "Custom/URP/ToonRock"
{
    Properties
    {
        [Header(Base Rock Color)]
        _BaseColor ("Base Rock Color", Color) = (0.5,0.5,0.5,1)
        _RockTexture ("Rock Texture (Optional)", 2D) = "white" {}
        _TextureScale ("Texture Scale", Float) = 1.0
        
        [Header(Edge Curvature)]
        _EdgeColor ("Edge Color", Color) = (0.3,0.3,0.3,1)
        _CurvatureStrength ("Curvature Strength", Range(0, 10)) = 2.0
        _CurvatureThreshold ("Curvature Threshold", Range(0, 1)) = 0.5
        _EdgeSmoothness ("Edge Smoothness", Range(0.001, 0.5)) = 0.1
        
        [Header(Grass Triplanar)]
        _GrassTexture ("Grass Texture", 2D) = "white" {}
        _GrassColor ("Grass Tint", Color) = (0.4,0.6,0.3,1)
        _GrassScale ("Grass Texture Scale", Float) = 1.0
        _GrassBlendSharpness ("Grass Blend Sharpness", Range(0.01, 1)) = 0.2
        
        [Header(Grass Placement)]
        _SlopeThreshold ("Slope Threshold (0=flat, 1=vertical)", Range(0, 1)) = 0.3
        _HeightMin ("Height Min (World Y)", Float) = 0
        _HeightMax ("Height Max (World Y)", Float) = 10
        _HeightFalloff ("Height Falloff", Range(0.1, 5)) = 1.0
        
        [Header(Toon Shading)]
        _ShadowStep ("Shadow Step", Range(0, 1)) = 0.5
        _ShadowFeather ("Shadow Feather", Range(0.0001, 1)) = 0.05
        _ShadowColor ("Shadow Color", Color) = (0.4,0.4,0.4,1)
        [Toggle(_)] _ReceiveLightColor ("Receive Light Color", Float) = 1
        
        [Header(Rim Light)]
        _RimColor ("Rim Color", Color) = (1,1,1,1)
        _RimPower ("Rim Power", Range(0.01, 10)) = 4
        _RimIntensity ("Rim Intensity", Range(0, 1)) = 0.3
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }
        
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _SHADOWS_SOFT
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 viewDirWS : TEXCOORD2;
            };
            
            TEXTURE2D(_RockTexture);
            SAMPLER(sampler_RockTexture);
            TEXTURE2D(_GrassTexture);
            SAMPLER(sampler_GrassTexture);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _EdgeColor;
                float4 _GrassColor;
                float4 _ShadowColor;
                float4 _RimColor;
                float _TextureScale;
                float _GrassScale;
                float _CurvatureStrength;
                float _CurvatureThreshold;
                float _EdgeSmoothness;
                float _SlopeThreshold;
                float _HeightMin;
                float _HeightMax;
                float _HeightFalloff;
                float _GrassBlendSharpness;
                float _ShadowStep;
                float _ShadowFeather;
                float _ReceiveLightColor;
                float _RimPower;
                float _RimIntensity;
            CBUFFER_END
            
            half3 SampleTriplanar(TEXTURE2D_PARAM(tex, samp), float3 position, float3 normal, float tiling)
            {
                half3 bf = normalize(abs(normal));
                bf /= dot(bf, half3(1, 1, 1));
                
                half3 tx = SAMPLE_TEXTURE2D(tex, samp, position.yz * tiling).rgb;
                half3 ty = SAMPLE_TEXTURE2D(tex, samp, position.xz * tiling).rgb;
                half3 tz = SAMPLE_TEXTURE2D(tex, samp, position.xy * tiling).rgb;
                
                return tx * bf.x + ty * bf.y + tz * bf.z;
            }
            
            half CalculateCurvature(float3 nrm)
            {
                half3 dpdx = ddx(nrm);
                half3 dpdy = ddy(nrm);
                return length(dpdx) + length(dpdy);
            }
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                
                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS);
                
                output.positionCS = positionInputs.positionCS;
                output.positionWS = positionInputs.positionWS;
                output.normalWS = normalInputs.normalWS;
                output.viewDirWS = GetWorldSpaceViewDir(positionInputs.positionWS);
                
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                half3 N = normalize(input.normalWS);
                half3 V = normalize(input.viewDirWS);
                
                half3 rockTex = SampleTriplanar(TEXTURE2D_ARGS(_RockTexture, sampler_RockTexture), 
                    input.positionWS, N, _TextureScale);
                half4 baseCol = _BaseColor * half4(rockTex, 1);
                
                half curv = CalculateCurvature(N) * _CurvatureStrength;
                half edgeMask = smoothstep(_CurvatureThreshold - _EdgeSmoothness, 
                    _CurvatureThreshold + _EdgeSmoothness, curv);
                half4 rockWithEdges = lerp(baseCol, _EdgeColor, edgeMask);
                
                half slope = 1 - N.y;
                half slopeFactor = smoothstep(_SlopeThreshold + _GrassBlendSharpness, 
                    _SlopeThreshold - _GrassBlendSharpness, slope);
                
                half heightFactor = saturate((input.positionWS.y - _HeightMin) / max(0.001, _HeightMax - _HeightMin));
                heightFactor = pow(heightFactor, _HeightFalloff);
                
                half grassMask = slopeFactor * heightFactor;
                
                half3 grassTex = SampleTriplanar(TEXTURE2D_ARGS(_GrassTexture, sampler_GrassTexture), 
                    input.positionWS, N, _GrassScale);
                half4 grassCol = _GrassColor * half4(grassTex, 1);
                
                half4 albedo = lerp(rockWithEdges, grassCol, grassMask);
                
                Light mainLight = GetMainLight(TransformWorldToShadowCoord(input.positionWS));
                half3 L = normalize(mainLight.direction);
                
                half NdotL = dot(N, L);
                half atten = mainLight.shadowAttenuation * mainLight.distanceAttenuation;
                
                half toonStep = smoothstep(_ShadowStep - _ShadowFeather, 
                    _ShadowStep + _ShadowFeather, NdotL * atten);
                
                half3 shadow = _ShadowColor.rgb;
                if (_ReceiveLightColor > 0.5)
                    shadow *= mainLight.color;
                
                half3 lit = albedo.rgb;
                half3 col = lerp(shadow * albedo.rgb, lit, toonStep);
                
                half fresnel = 1 - saturate(dot(N, V));
                half rim = pow(fresnel, _RimPower) * _RimIntensity;
                col += _RimColor.rgb * rim;
                
                #ifdef _ADDITIONAL_LIGHTS
                uint lightCount = GetAdditionalLightsCount();
                for (uint i = 0; i < lightCount; i++)
                {
                    Light light = GetAdditionalLight(i, input.positionWS);
                    half3 lightDir = normalize(light.direction);
                    half ndl = dot(N, lightDir);
                    half toon = step(0.5, ndl * light.shadowAttenuation * light.distanceAttenuation);
                    col += albedo.rgb * light.color * toon * 0.5;
                }
                #endif
                
                return half4(col, 1);
            }
            ENDHLSL
        }
        
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            
            ZWrite On
            ZTest LEqual
            ColorMask 0
            
            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };
            
            Varyings ShadowPassVertex(Attributes input)
            {
                Varyings output;
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, 0));
                return output;
            }
            
            half4 ShadowPassFragment(Varyings input) : SV_Target
            {
                return 0;
            }
            ENDHLSL
        }
        
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }
            
            ZWrite On
            ColorMask R
            
            HLSLPROGRAM
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };
            
            Varyings DepthOnlyVertex(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                return output;
            }
            
            half4 DepthOnlyFragment(Varyings input) : SV_Target
            {
                return 0;
            }
            ENDHLSL
        }
    }
    
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
