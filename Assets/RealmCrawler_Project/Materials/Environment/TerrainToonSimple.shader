Shader "Custom/URP/TerrainToonSimple"
{
    Properties
    {
        [Header(Flat Textures)]
        _FlatTex ("Flat Texture (Grass)", 2D) = "white" {}
        _FlatColor ("Flat Tint", Color) = (1,1,1,1)
        _FlatNormal ("Flat Normal Map", 2D) = "bump" {}
        _FlatScale ("Flat Texture Scale", Float) = 1.0
        
        [Header(Mud Textures)]
        _MudTex ("Mud Texture", 2D) = "white" {}
        _MudColor ("Mud Tint", Color) = (0.6,0.5,0.4,1)
        _MudNormal ("Mud Normal Map", 2D) = "bump" {}
        _MudScale ("Mud Texture Scale", Float) = 1.0
        
        [Header(Noise Blend)]
        _NoiseMask ("Noise Mask (Grass-Mud Blend)", 2D) = "white" {}
        _NoiseScale ("Noise Scale", Float) = 1.0
        _NoiseContrast ("Noise Contrast", Range(0, 2)) = 1.0
        _NoiseBrightness ("Noise Brightness", Range(-1, 1)) = 0.0
        
        [Header(Height Blend Colors)]
        _LowColor ("Low Height Color", Color) = (0.3,0.25,0.2,1)
        _HighColor ("High Height Color", Color) = (0.5,0.5,0.5,1)
        _HeightMin ("Height Min (World Y)", Float) = 0
        _HeightMax ("Height Max (World Y)", Float) = 20
        
        [Header(Curvature Edges)]
        _EdgeColor ("Edge Color", Color) = (0.2,0.2,0.2,1)
        _CurvatureStrength ("Curvature Strength", Range(0, 10)) = 2.0
        _CurvatureThreshold ("Curvature Threshold", Range(0, 1)) = 0.5
        _EdgeSmoothness ("Edge Smoothness", Range(0.001, 0.5)) = 0.1
        
        [Header(Slope Settings)]
        _SlopeThreshold ("Slope Threshold", Range(0,1)) = 0.5
        _SlopeBlend ("Slope Blend Smoothness", Range(0.01,0.5)) = 0.1
        _TextureScale ("Texture Tiling", Float) = 0.1
        
        [Header(Toon Shading)]
        _BaseColor_Step ("Shadow Step", Range(0, 1)) = 0.5
        _BaseShade_Feather ("Shadow Feather", Range(0.0001, 1)) = 0.0001
        _1st_ShadeColor ("Shadow Color", Color) = (0.6,0.6,0.6,1)
        [Toggle(_)] _Is_LightColor_Base ("Receive Light Color", Float) = 1
        
        [Header(Rim Light)]
        _RimColor ("Rim Color", Color) = (1,1,1,1)
        _RimPower ("Rim Power", Range(0.01, 10)) = 3
        _RimIntensity ("Rim Intensity", Range(0, 1)) = 0.5
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
                float4 tangentOS : TANGENT;
                float2 uv : TEXCOORD0;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 viewDirWS : TEXCOORD2;
                float3 tangentWS : TEXCOORD3;
                float3 bitangentWS : TEXCOORD4;
            };
            
            TEXTURE2D(_FlatTex);
            SAMPLER(sampler_FlatTex);
            TEXTURE2D(_FlatNormal);
            SAMPLER(sampler_FlatNormal);
            
            TEXTURE2D(_MudTex);
            SAMPLER(sampler_MudTex);
            TEXTURE2D(_MudNormal);
            SAMPLER(sampler_MudNormal);
            
            TEXTURE2D(_NoiseMask);
            SAMPLER(sampler_NoiseMask);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _FlatColor;
                float4 _MudColor;
                float4 _LowColor;
                float4 _HighColor;
                float4 _EdgeColor;
                float4 _1st_ShadeColor;
                float4 _RimColor;
                
                float _FlatScale;
                float _MudScale;
                float _NoiseScale;
                float _NoiseContrast;
                float _NoiseBrightness;
                
                float _HeightMin;
                float _HeightMax;
                float _CurvatureStrength;
                float _CurvatureThreshold;
                float _EdgeSmoothness;
                
                float _SlopeThreshold;
                float _SlopeBlend;
                float _TextureScale;
                
                float _BaseColor_Step;
                float _BaseShade_Feather;
                float _Is_LightColor_Base;
                float _RimPower;
                float _RimIntensity;
            CBUFFER_END
            
            half CalculateCurvature(float3 normalWS)
            {
                half3 dpdx = ddx(normalWS);
                half3 dpdy = ddy(normalWS);
                return (length(dpdx) + length(dpdy)) * _CurvatureStrength;
            }
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                
                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                
                output.positionCS = positionInputs.positionCS;
                output.positionWS = positionInputs.positionWS;
                output.normalWS = normalInputs.normalWS;
                output.viewDirWS = GetWorldSpaceViewDir(positionInputs.positionWS);
                output.tangentWS = normalInputs.tangentWS;
                output.bitangentWS = normalInputs.bitangentWS;
                
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                float2 baseUV = input.positionWS.xz * _TextureScale;
                
                float2 grassUV = baseUV * _FlatScale;
                float2 mudUV = baseUV * _MudScale;
                float2 noiseUV = input.positionWS.xz * _NoiseScale;
                
                half noiseMask = SAMPLE_TEXTURE2D(_NoiseMask, sampler_NoiseMask, noiseUV).r;
                noiseMask = saturate(noiseMask + _NoiseBrightness);
                noiseMask = saturate((noiseMask - 0.5) * _NoiseContrast + 0.5);
                
                half4 grassAlbedo = SAMPLE_TEXTURE2D(_FlatTex, sampler_FlatTex, grassUV) * _FlatColor;
                half4 mudAlbedo = SAMPLE_TEXTURE2D(_MudTex, sampler_MudTex, mudUV) * _MudColor;
                half4 flatAlbedo = lerp(mudAlbedo, grassAlbedo, noiseMask);
                
                half3 grassNormal = UnpackNormal(SAMPLE_TEXTURE2D(_FlatNormal, sampler_FlatNormal, grassUV));
                half3 mudNormal = UnpackNormal(SAMPLE_TEXTURE2D(_MudNormal, sampler_MudNormal, mudUV));
                half3 flatNormal = lerp(mudNormal, grassNormal, noiseMask);
                
                float slope = 1.0 - input.normalWS.y;
                float blendFactor = smoothstep(_SlopeThreshold - _SlopeBlend, _SlopeThreshold + _SlopeBlend, slope);
                
                float heightNormalized = saturate((input.positionWS.y - _HeightMin) / max(0.001, _HeightMax - _HeightMin));
                half4 heightColor = lerp(_LowColor, _HighColor, heightNormalized);
                
                half4 albedo = lerp(flatAlbedo, heightColor, blendFactor);
                
                half3 tangentNormal = lerp(flatNormal, half3(0, 0, 1), blendFactor);
                
                half3x3 tangentToWorld = half3x3(input.tangentWS, input.bitangentWS, input.normalWS);
                half3 normalWS = normalize(mul(tangentNormal, tangentToWorld));
                
                half curvature = CalculateCurvature(normalWS);
                half edgeMask = smoothstep(_CurvatureThreshold - _EdgeSmoothness, 
                    _CurvatureThreshold + _EdgeSmoothness, curvature);
                albedo = lerp(albedo, _EdgeColor, edgeMask);
                
                Light mainLight = GetMainLight(TransformWorldToShadowCoord(input.positionWS));
                half3 lightDir = normalize(mainLight.direction);
                half3 viewDir = normalize(input.viewDirWS);
                
                half NdotL = dot(normalWS, lightDir);
                half lightAttenuation = mainLight.shadowAttenuation * mainLight.distanceAttenuation;
                
                half toonRamp = smoothstep(_BaseColor_Step - _BaseShade_Feather, _BaseColor_Step + _BaseShade_Feather, NdotL * lightAttenuation);
                
                half3 shadowColor = _1st_ShadeColor.rgb;
                if (_Is_LightColor_Base > 0.5)
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
