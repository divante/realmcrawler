Shader "Custom/URP/RockToonTriplanar"
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
                float2 uv : TEXCOORD3;
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
            
            float3 TriplanarMapping(TEXTURE2D_PARAM(tex, texSampler), float3 worldPos, float3 worldNormal, float scale)
            {
                float3 blendWeights = abs(worldNormal);
                blendWeights = blendWeights * blendWeights * blendWeights;
                blendWeights = blendWeights / (blendWeights.x + blendWeights.y + blendWeights.z);
                
                float3 texX = SAMPLE_TEXTURE2D(tex, texSampler, worldPos.yz * scale).rgb;
                float3 texY = SAMPLE_TEXTURE2D(tex, texSampler, worldPos.xz * scale).rgb;
                float3 texZ = SAMPLE_TEXTURE2D(tex, texSampler, worldPos.xy * scale).rgb;
                
                return texX * blendWeights.x + texY * blendWeights.y + texZ * blendWeights.z;
            }
            
            float CalculateCurvature(float3 normalWS, float3 positionWS)
            {
                float3 ddxNormal = ddx(normalWS);
                float3 ddyNormal = ddy(normalWS);
                float curvature = length(ddxNormal) + length(ddyNormal);
                return curvature * _CurvatureStrength;
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
                output.uv = input.uv;
                
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                float3 normalWS = normalize(input.normalWS);
                float3 viewDir = normalize(input.viewDirWS);
                
                float3 rockTexture = TriplanarMapping(TEXTURE2D_ARGS(_RockTexture, sampler_RockTexture), 
                    input.positionWS, normalWS, _TextureScale);
                half4 baseColor = _BaseColor * half4(rockTexture, 1.0);
                
                float curvature = CalculateCurvature(normalWS, input.positionWS);
                float edgeMask = smoothstep(_CurvatureThreshold - _EdgeSmoothness, 
                    _CurvatureThreshold + _EdgeSmoothness, curvature);
                half4 colorWithEdges = lerp(baseColor, _EdgeColor, edgeMask);
                
                float slope = 1.0 - normalWS.y;
                float slopeMask = smoothstep(_SlopeThreshold + _GrassBlendSharpness, 
                    _SlopeThreshold - _GrassBlendSharpness, slope);
                
                float heightNormalized = saturate((input.positionWS.y - _HeightMin) / (_HeightMax - _HeightMin));
                float heightMask = pow(heightNormalized, _HeightFalloff);
                
                float grassBlend = slopeMask * heightMask;
                
                float3 grassTexture = TriplanarMapping(TEXTURE2D_ARGS(_GrassTexture, sampler_GrassTexture), 
                    input.positionWS, normalWS, _GrassScale);
                half4 grassColor = _GrassColor * half4(grassTexture, 1.0);
                
                half4 albedo = lerp(colorWithEdges, grassColor, grassBlend);
                
                Light mainLight = GetMainLight(TransformWorldToShadowCoord(input.positionWS));
                half3 lightDir = normalize(mainLight.direction);
                
                half NdotL = dot(normalWS, lightDir);
                half lightAttenuation = mainLight.shadowAttenuation * mainLight.distanceAttenuation;
                
                half toonRamp = smoothstep(_ShadowStep - _ShadowFeather, 
                    _ShadowStep + _ShadowFeather, NdotL * lightAttenuation);
                
                half3 shadowColor = _ShadowColor.rgb;
                if (_ReceiveLightColor > 0.5)
                {
                    shadowColor *= mainLight.color;
                }
                
                half3 litColor = albedo.rgb;
                half3 finalColor = lerp(shadowColor * albedo.rgb, litColor, toonRamp);
                
                half NdotV = saturate(dot(normalWS, viewDir));
                half rim = 1.0 - NdotV;
                rim = pow(rim, _RimPower) * _RimIntensity;
                finalColor += _RimColor.rgb * rim;
                
                #ifdef _ADDITIONAL_LIGHTS
                uint pixelLightCount = GetAdditionalLightsCount();
                for (uint lightIndex = 0; lightIndex < pixelLightCount; ++lightIndex)
                {
                    Light light = GetAdditionalLight(lightIndex, input.positionWS);
                    half3 additionalLightDir = normalize(light.direction);
                    half additionalNdotL = dot(normalWS, additionalLightDir);
                    half additionalToon = step(0.5, additionalNdotL * light.shadowAttenuation * light.distanceAttenuation);
                    finalColor += albedo.rgb * light.color * additionalToon * 0.5;
                }
                #endif
                
                return half4(finalColor, 1.0);
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
