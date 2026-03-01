Shader "Custom/URP/ToonTriplanarSlope"
{
    Properties
    {
        [Header(Flat Surface Textures - Triplanar)]
        _BaseMap ("Base Map (Grass/Flat)", 2D) = "white" {}
        _1st_ShadeMap ("1st Shade Map (Grass/Flat)", 2D) = "white" {}
        _2nd_ShadeMap ("2nd Shade Map (Grass/Flat)", 2D) = "white" {}
        _NormalMap ("Normal Map (Grass/Flat)", 2D) = "bump" {}
        _BumpScale ("Normal Strength", Range(0, 2)) = 1
        _TriplanarScale ("Texture Scale", Range(0.01, 10)) = 1.0
        
        [Header(Vertical Surface Colors - Solid Toon)]
        _BaseColor ("Base Color (Cliff)", Color) = (1,1,1,1)
        _1st_ShadeColor ("1st Shade Color (Cliff)", Color) = (0.5,0.5,0.5,1)
        _2nd_ShadeColor ("2nd Shade Color (Cliff)", Color) = (0.3,0.3,0.3,1)
        _Is_LightColor_Base ("Base Receives Light Color", Float) = 1
        _Is_LightColor_1st_Shade ("1st Shade Receives Light Color", Float) = 1
        _Is_LightColor_2nd_Shade ("2nd Shade Receives Light Color", Float) = 1
        
        [Header(Slope Blending)]
        _SlopeThreshold ("Slope Threshold", Range(0, 1)) = 0.5
        _SlopeBlend ("Slope Blend Smoothness", Range(0, 0.5)) = 0.1
        
        [Header(Toon Shading)]
        _BaseColor_Step ("Base/1st Shade Step", Range(0, 1)) = 0.5
        _BaseShade_Feather ("Base/1st Shade Feather", Range(0.0001, 1)) = 0.05
        _ShadeColor_Step ("1st/2nd Shade Step", Range(0, 1)) = 0.5
        _1st2nd_Shades_Feather ("1st/2nd Shade Feather", Range(0.0001, 1)) = 0.05
        _StepOffset ("Step Offset (All)", Range(-0.5, 0.5)) = 0
        
        [Header(Rim Light)]
        [Toggle(_)] _RimLight ("Enable Rim Light", Float) = 1
        _RimLightColor ("Rim Light Color", Color) = (1,1,1,1)
        _Is_LightColor_RimLight ("Rim Receives Light Color", Float) = 1
        _RimLight_Power ("Rim Light Power", Range(0, 10)) = 1
        _RimLight_InsideMask ("Rim Inside Mask", Range(0.0001, 1)) = 0.5
        [Toggle(_)] _RimLight_FeatherOff ("Rim Feather Off", Float) = 0
        
        [Header(Outline)]
        [Toggle(_)] _OUTLINE ("Enable Outline", Float) = 1
        _Outline_Width ("Outline Width", Range(0, 10)) = 1
        _Farthest_Distance ("Farthest Distance", Range(0.5, 1000)) = 100
        _Nearest_Distance ("Nearest Distance", Range(0, 50)) = 0.5
        _Outline_Color ("Outline Color", Color) = (0,0,0,1)
        [Toggle(_)] _Is_BlendBaseColor ("Blend Base Color to Outline", Float) = 0
        
        [Header(Rendering)]
        [Enum(UnityEngine.Rendering.CullMode)] _CullMode ("Cull Mode", Float) = 2
        [Enum(Off,0,On,1)] _ZWriteMode ("ZWrite", Float) = 1
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
            
            Cull [_CullMode]
            ZWrite [_ZWriteMode]
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile_fog
            
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
                float3 tangentWS : TEXCOORD2;
                float3 bitangentWS : TEXCOORD3;
                float2 uv : TEXCOORD4;
                float fogFactor : TEXCOORD5;
            };
            
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_1st_ShadeMap);
            SAMPLER(sampler_1st_ShadeMap);
            TEXTURE2D(_2nd_ShadeMap);
            SAMPLER(sampler_2nd_ShadeMap);
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _BaseColor;
                float4 _1st_ShadeColor;
                float4 _2nd_ShadeColor;
                float4 _RimLightColor;
                
                float _TriplanarScale;
                float _BumpScale;
                float _SlopeThreshold;
                float _SlopeBlend;
                
                float _BaseColor_Step;
                float _BaseShade_Feather;
                float _ShadeColor_Step;
                float _1st2nd_Shades_Feather;
                float _StepOffset;
                
                float _Is_LightColor_Base;
                float _Is_LightColor_1st_Shade;
                float _Is_LightColor_2nd_Shade;
                
                float _RimLight;
                float _Is_LightColor_RimLight;
                float _RimLight_Power;
                float _RimLight_InsideMask;
                float _RimLight_FeatherOff;
            CBUFFER_END
            
            half3 SampleTriplanar(Texture2D tex, SamplerState samp, float3 positionWS, float3 normalWS, float scale)
            {
                float3 blendWeights = abs(normalWS);
                blendWeights = blendWeights / (blendWeights.x + blendWeights.y + blendWeights.z);
                
                float2 uvX = positionWS.yz * scale;
                float2 uvY = positionWS.xz * scale;
                float2 uvZ = positionWS.xy * scale;
                
                half3 colX = SAMPLE_TEXTURE2D(tex, samp, uvX).rgb;
                half3 colY = SAMPLE_TEXTURE2D(tex, samp, uvY).rgb;
                half3 colZ = SAMPLE_TEXTURE2D(tex, samp, uvZ).rgb;
                
                return colX * blendWeights.x + colY * blendWeights.y + colZ * blendWeights.z;
            }
            
            half3 SampleTriplanarNormal(Texture2D tex, SamplerState samp, float3 positionWS, float3 normalWS, float scale, float strength)
            {
                float3 blendWeights = abs(normalWS);
                blendWeights = blendWeights / (blendWeights.x + blendWeights.y + blendWeights.z);
                
                float2 uvX = positionWS.yz * scale;
                float2 uvY = positionWS.xz * scale;
                float2 uvZ = positionWS.xy * scale;
                
                half3 tnormalX = UnpackNormalScale(SAMPLE_TEXTURE2D(tex, samp, uvX), strength);
                half3 tnormalY = UnpackNormalScale(SAMPLE_TEXTURE2D(tex, samp, uvY), strength);
                half3 tnormalZ = UnpackNormalScale(SAMPLE_TEXTURE2D(tex, samp, uvZ), strength);
                
                tnormalX = half3(0, tnormalX.y, tnormalX.x);
                tnormalZ = half3(tnormalZ.x, tnormalZ.y, 0);
                
                half3 worldNormal = normalize(
                    tnormalX.zyx * blendWeights.x +
                    tnormalY.xyz * blendWeights.y +
                    tnormalZ.xyz * blendWeights.z +
                    normalWS
                );
                
                return worldNormal;
            }
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                
                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                
                output.positionCS = positionInputs.positionCS;
                output.positionWS = positionInputs.positionWS;
                output.normalWS = normalInputs.normalWS;
                output.tangentWS = normalInputs.tangentWS;
                output.bitangentWS = normalInputs.bitangentWS;
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                output.fogFactor = ComputeFogFactor(positionInputs.positionCS.z);
                
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                float3 normalWS = normalize(input.normalWS);
                
                float slope = 1.0 - normalWS.y;
                float slopeMask = smoothstep(_SlopeThreshold - _SlopeBlend, _SlopeThreshold + _SlopeBlend, slope);
                
                half3 baseAlbedo;
                half3 shade1Albedo;
                half3 shade2Albedo;
                half3 finalNormal;
                
                if (slopeMask < 0.99)
                {
                    baseAlbedo = SampleTriplanar(_BaseMap, sampler_BaseMap, input.positionWS, normalWS, _TriplanarScale);
                    shade1Albedo = SampleTriplanar(_1st_ShadeMap, sampler_1st_ShadeMap, input.positionWS, normalWS, _TriplanarScale);
                    shade2Albedo = SampleTriplanar(_2nd_ShadeMap, sampler_2nd_ShadeMap, input.positionWS, normalWS, _TriplanarScale);
                    finalNormal = SampleTriplanarNormal(_NormalMap, sampler_NormalMap, input.positionWS, normalWS, _TriplanarScale, _BumpScale);
                }
                else
                {
                    baseAlbedo = half3(1, 1, 1);
                    shade1Albedo = half3(1, 1, 1);
                    shade2Albedo = half3(1, 1, 1);
                    finalNormal = normalWS;
                }
                
                half3 baseColor = lerp(_BaseColor.rgb, baseAlbedo * _BaseColor.rgb, 1.0 - slopeMask);
                half3 shade1Color = lerp(_1st_ShadeColor.rgb, shade1Albedo * _1st_ShadeColor.rgb, 1.0 - slopeMask);
                half3 shade2Color = lerp(_2nd_ShadeColor.rgb, shade2Albedo * _2nd_ShadeColor.rgb, 1.0 - slopeMask);
                
                Light mainLight = GetMainLight(TransformWorldToShadowCoord(input.positionWS));
                half3 lightDir = normalize(mainLight.direction);
                half3 viewDir = normalize(GetWorldSpaceViewDir(input.positionWS));
                
                half NdotL = dot(finalNormal, lightDir);
                half lightAttenuation = mainLight.shadowAttenuation * mainLight.distanceAttenuation;
                
                half adjustedNdotL = NdotL * lightAttenuation + _StepOffset;
                
                half baseShadeStep = smoothstep(
                    _BaseColor_Step - _BaseShade_Feather,
                    _BaseColor_Step + _BaseShade_Feather,
                    adjustedNdotL
                );
                
                half shadeColorStep = smoothstep(
                    _ShadeColor_Step - _1st2nd_Shades_Feather,
                    _ShadeColor_Step + _1st2nd_Shades_Feather,
                    adjustedNdotL
                );
                
                half3 shadedColor = lerp(shade2Color, shade1Color, shadeColorStep);
                shadedColor = lerp(shadedColor, baseColor, baseShadeStep);
                
                if (_Is_LightColor_Base > 0.5)
                {
                    shadedColor *= lerp(half3(1,1,1), mainLight.color, baseShadeStep);
                }
                
                half3 finalColor = shadedColor;
                
                if (_RimLight > 0.5)
                {
                    half NdotV = saturate(dot(finalNormal, viewDir));
                    half rim = pow(1.0 - NdotV, _RimLight_Power);
                    
                    if (_RimLight_FeatherOff < 0.5)
                    {
                        rim = smoothstep(_RimLight_InsideMask - 0.1, _RimLight_InsideMask + 0.1, rim);
                    }
                    else
                    {
                        rim = step(_RimLight_InsideMask, rim);
                    }
                    
                    half3 rimColor = _RimLightColor.rgb;
                    if (_Is_LightColor_RimLight > 0.5)
                    {
                        rimColor *= mainLight.color;
                    }
                    
                    finalColor += rimColor * rim;
                }
                
                #ifdef _ADDITIONAL_LIGHTS
                uint pixelLightCount = GetAdditionalLightsCount();
                for (uint lightIndex = 0; lightIndex < pixelLightCount; ++lightIndex)
                {
                    Light light = GetAdditionalLight(lightIndex, input.positionWS);
                    half addNdotL = dot(finalNormal, normalize(light.direction));
                    half addToon = step(0.5, addNdotL * light.shadowAttenuation * light.distanceAttenuation);
                    finalColor += baseColor * light.color * addToon * 0.3;
                }
                #endif
                
                finalColor = MixFog(finalColor, input.fogFactor);
                
                return half4(finalColor, 1.0);
            }
            ENDHLSL
        }
        
        Pass
        {
            Name "Outline"
            Tags { "LightMode" = "SRPDefaultUnlit" }
            
            Cull Front
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };
            
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _Outline_Color;
                float _OUTLINE;
                float _Outline_Width;
                float _Farthest_Distance;
                float _Nearest_Distance;
                float _Is_BlendBaseColor;
            CBUFFER_END
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                
                if (_OUTLINE < 0.5)
                {
                    output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                    return output;
                }
                
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                float distanceToCamera = distance(positionWS, _WorldSpaceCameraPos);
                float distanceFade = saturate((distanceToCamera - _Nearest_Distance) / (_Farthest_Distance - _Nearest_Distance));
                
                float outlineWidth = _Outline_Width * 0.001 * distanceFade;
                
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
                positionWS += normalWS * outlineWidth;
                
                output.positionCS = TransformWorldToHClip(positionWS);
                
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                if (_OUTLINE < 0.5)
                {
                    discard;
                }
                
                half3 outlineColor = _Outline_Color.rgb;
                
                if (_Is_BlendBaseColor > 0.5)
                {
                    outlineColor = lerp(outlineColor, _BaseColor.rgb * outlineColor, 0.5);
                }
                
                return half4(outlineColor, 1.0);
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
            Cull [_CullMode]
            
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
            Cull [_CullMode]
            
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
