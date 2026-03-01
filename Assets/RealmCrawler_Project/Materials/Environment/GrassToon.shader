Shader "Custom/URP/GrassToon"
{
    Properties
    {
        [Header(Grass Textures)]
        _MainTex ("Grass Texture", 2D) = "white" {}
        _Color ("Grass Tint", Color) = (1,1,1,1)
        _Cutoff ("Alpha Cutoff", Range(0, 1)) = 0.5
        
        [Header(Wind Settings)]
        _WindSpeed ("Wind Speed", Range(0, 5)) = 1.0
        _WindStrength ("Wind Strength", Range(0, 2)) = 0.3
        _WindScale ("Wind Scale", Range(0.001, 1)) = 0.1
        _WindDirection ("Wind Direction (XZ)", Vector) = (1, 0, 0, 0)
        _SwaySpeed ("Sway Speed", Range(0, 5)) = 0.5
        _SwayStrength ("Sway Strength", Range(0, 1)) = 0.2
        [Toggle(_)] _UseVertexColor ("Use Vertex Color (R = Wind)", Float) = 1
        
        [Header(Toon Shading)]
        _BaseColor_Step ("Shadow Step", Range(0, 1)) = 0.5
        _BaseShade_Feather ("Shadow Feather", Range(0.0001, 1)) = 0.05
        _1st_ShadeColor ("Shadow Color", Color) = (0.4,0.5,0.3,1)
        [Toggle(_)] _Is_LightColor_Base ("Receive Light Color", Float) = 1
        
        [Header(Rim Light)]
        _RimColor ("Rim Color", Color) = (0.8,1,0.8,1)
        _RimPower ("Rim Power", Range(0.01, 10)) = 4
        _RimIntensity ("Rim Intensity", Range(0, 1)) = 0.3
        
        [Header(Rendering)]
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull Mode", Float) = 0
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType" = "TransparentCutout"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "AlphaTest"
            "IgnoreProjector" = "True"
        }
        
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            
            Cull [_Cull]
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma instancing_options procedural:setup
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                void setup()
                {
                }
            #endif
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 viewDirWS : TEXCOORD2;
                float2 uv : TEXCOORD3;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float4 _WindDirection;
                float4 _1st_ShadeColor;
                float4 _RimColor;
                
                float _Cutoff;
                float _WindSpeed;
                float _WindStrength;
                float _WindScale;
                float _SwaySpeed;
                float _SwayStrength;
                float _UseVertexColor;
                
                float _BaseColor_Step;
                float _BaseShade_Feather;
                float _Is_LightColor_Base;
                float _RimPower;
                float _RimIntensity;
            CBUFFER_END
            
            float3 ApplyWind(float3 positionWS, float3 positionOS, float vertexColorR)
            {
                float windMask = positionOS.y;
                if (_UseVertexColor > 0.5)
                {
                    windMask *= vertexColorR;
                }
                
                float time = _Time.y;
                
                float2 windDir = normalize(_WindDirection.xz);
                float2 worldXZ = positionWS.xz;
                
                float windPhase = (worldXZ.x * windDir.x + worldXZ.y * windDir.y) * _WindScale;
                float windWave = sin(time * _WindSpeed + windPhase) * 0.5 + 0.5;
                windWave = windWave * windWave;
                
                float swayPhase = (worldXZ.x + worldXZ.y) * _WindScale * 2.0;
                float sway = sin(time * _SwaySpeed + swayPhase) * _SwayStrength;
                
                float3 windOffset = float3(
                    windDir.x * windWave * _WindStrength + sway,
                    0,
                    windDir.y * windWave * _WindStrength + sway * 0.5
                );
                
                return positionWS + windOffset * windMask;
            }
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                
                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS);
                
                float3 positionWS = positionInputs.positionWS;
                positionWS = ApplyWind(positionWS, input.positionOS.xyz, input.color.r);
                
                output.positionCS = TransformWorldToHClip(positionWS);
                output.positionWS = positionWS;
                output.normalWS = normalInputs.normalWS;
                output.viewDirWS = GetWorldSpaceViewDir(positionWS);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.color = input.color;
                
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                
                half4 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv) * _Color;
                
                clip(albedo.a - _Cutoff);
                
                half3 normalWS = normalize(input.normalWS);
                
                Light mainLight = GetMainLight(TransformWorldToShadowCoord(input.positionWS));
                half3 lightDir = normalize(mainLight.direction);
                half3 viewDir = normalize(input.viewDirWS);
                
                half NdotL = dot(normalWS, lightDir);
                half lightAttenuation = mainLight.shadowAttenuation * mainLight.distanceAttenuation;
                
                half toonRamp = smoothstep(_BaseColor_Step - _BaseShade_Feather, 
                    _BaseColor_Step + _BaseShade_Feather, NdotL * lightAttenuation);
                
                half3 shadowColor = _1st_ShadeColor.rgb;
                if (_Is_LightColor_Base > 0.5)
                {
                    shadowColor *= mainLight.color;
                }
                
                half3 litColor = albedo.rgb * mainLight.color;
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
                    finalColor += albedo.rgb * light.color * additionalToon * 0.3;
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
            Cull [_Cull]
            
            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            #pragma multi_compile_instancing
            #pragma instancing_options procedural:setup
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
            
            #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                void setup()
                {
                }
            #endif
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float4 _WindDirection;
                float _Cutoff;
                float _WindSpeed;
                float _WindStrength;
                float _WindScale;
                float _SwaySpeed;
                float _SwayStrength;
                float _UseVertexColor;
            CBUFFER_END
            
            float3 ApplyWind(float3 positionWS, float3 positionOS, float vertexColorR)
            {
                float windMask = positionOS.y;
                if (_UseVertexColor > 0.5)
                {
                    windMask *= vertexColorR;
                }
                
                float time = _Time.y;
                
                float2 windDir = normalize(_WindDirection.xz);
                float2 worldXZ = positionWS.xz;
                
                float windPhase = (worldXZ.x * windDir.x + worldXZ.y * windDir.y) * _WindScale;
                float windWave = sin(time * _WindSpeed + windPhase) * 0.5 + 0.5;
                windWave = windWave * windWave;
                
                float swayPhase = (worldXZ.x + worldXZ.y) * _WindScale * 2.0;
                float sway = sin(time * _SwaySpeed + swayPhase) * _SwayStrength;
                
                float3 windOffset = float3(
                    windDir.x * windWave * _WindStrength + sway,
                    0,
                    windDir.y * windWave * _WindStrength + sway * 0.5
                );
                
                return positionWS + windOffset * windMask;
            }
            
            Varyings ShadowPassVertex(Attributes input)
            {
                Varyings output;
                
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                positionWS = ApplyWind(positionWS, input.positionOS.xyz, input.color.r);
                
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, 0));
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                
                return output;
            }
            
            half4 ShadowPassFragment(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                half alpha = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv).a * _Color.a;
                clip(alpha - _Cutoff);
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
            Cull [_Cull]
            
            HLSLPROGRAM
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment
            #pragma multi_compile_instancing
            #pragma instancing_options procedural:setup
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                void setup()
                {
                }
            #endif
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float4 _WindDirection;
                float _Cutoff;
                float _WindSpeed;
                float _WindStrength;
                float _WindScale;
                float _SwaySpeed;
                float _SwayStrength;
                float _UseVertexColor;
            CBUFFER_END
            
            float3 ApplyWind(float3 positionWS, float3 positionOS, float vertexColorR)
            {
                float windMask = positionOS.y;
                if (_UseVertexColor > 0.5)
                {
                    windMask *= vertexColorR;
                }
                
                float time = _Time.y;
                
                float2 windDir = normalize(_WindDirection.xz);
                float2 worldXZ = positionWS.xz;
                
                float windPhase = (worldXZ.x * windDir.x + worldXZ.y * windDir.y) * _WindScale;
                float windWave = sin(time * _WindSpeed + windPhase) * 0.5 + 0.5;
                windWave = windWave * windWave;
                
                float swayPhase = (worldXZ.x + worldXZ.y) * _WindScale * 2.0;
                float sway = sin(time * _SwaySpeed + swayPhase) * _SwayStrength;
                
                float3 windOffset = float3(
                    windDir.x * windWave * _WindStrength + sway,
                    0,
                    windDir.y * windWave * _WindStrength + sway * 0.5
                );
                
                return positionWS + windOffset * windMask;
            }
            
            Varyings DepthOnlyVertex(Attributes input)
            {
                Varyings output;
                
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                positionWS = ApplyWind(positionWS, input.positionOS.xyz, input.color.r);
                
                output.positionCS = TransformWorldToHClip(positionWS);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                
                return output;
            }
            
            half4 DepthOnlyFragment(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                half alpha = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv).a * _Color.a;
                clip(alpha - _Cutoff);
                return 0;
            }
            ENDHLSL
        }
    }
    
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
