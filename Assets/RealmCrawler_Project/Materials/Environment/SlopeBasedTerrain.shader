Shader "Custom/URP/SlopeBasedTerrain"
{
    Properties
    {
        _FlatTex ("Flat Texture (Grass)", 2D) = "white" {}
        _FlatColor ("Flat Tint", Color) = (1,1,1,1)
        _FlatNormal ("Flat Normal Map", 2D) = "bump" {}
        _FlatSmoothness ("Flat Smoothness", Range(0,1)) = 0.5
        
        _SteepTex ("Steep Texture (Cliff)", 2D) = "white" {}
        _SteepColor ("Steep Tint", Color) = (1,1,1,1)
        _SteepNormal ("Steep Normal Map", 2D) = "bump" {}
        _SteepSmoothness ("Steep Smoothness", Range(0,1)) = 0.3
        
        _SlopeThreshold ("Slope Threshold", Range(0,1)) = 0.5
        _SlopeBlend ("Slope Blend Smoothness", Range(0.01,0.5)) = 0.1
        _TextureScale ("Texture Tiling", Float) = 1.0
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
                float3 tangentWS : TEXCOORD2;
                float3 bitangentWS : TEXCOORD3;
                float2 uv : TEXCOORD4;
            };
            
            TEXTURE2D(_FlatTex);
            SAMPLER(sampler_FlatTex);
            TEXTURE2D(_FlatNormal);
            SAMPLER(sampler_FlatNormal);
            
            TEXTURE2D(_SteepTex);
            SAMPLER(sampler_SteepTex);
            TEXTURE2D(_SteepNormal);
            SAMPLER(sampler_SteepNormal);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _FlatColor;
                float4 _SteepColor;
                float _FlatSmoothness;
                float _SteepSmoothness;
                float _SlopeThreshold;
                float _SlopeBlend;
                float _TextureScale;
            CBUFFER_END
            
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
                output.uv = input.uv;
                
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                float2 scaledUV = input.positionWS.xz * _TextureScale;
                
                float slope = 1.0 - input.normalWS.y;
                float blendFactor = smoothstep(_SlopeThreshold - _SlopeBlend, _SlopeThreshold + _SlopeBlend, slope);
                
                half4 flatAlbedo = SAMPLE_TEXTURE2D(_FlatTex, sampler_FlatTex, scaledUV) * _FlatColor;
                half4 steepAlbedo = SAMPLE_TEXTURE2D(_SteepTex, sampler_SteepTex, scaledUV) * _SteepColor;
                half4 albedo = lerp(flatAlbedo, steepAlbedo, blendFactor);
                
                half3 flatNormal = UnpackNormal(SAMPLE_TEXTURE2D(_FlatNormal, sampler_FlatNormal, scaledUV));
                half3 steepNormal = UnpackNormal(SAMPLE_TEXTURE2D(_SteepNormal, sampler_SteepNormal, scaledUV));
                half3 tangentNormal = lerp(flatNormal, steepNormal, blendFactor);
                
                half3x3 tangentToWorld = half3x3(input.tangentWS, input.bitangentWS, input.normalWS);
                half3 normalWS = normalize(mul(tangentNormal, tangentToWorld));
                
                float smoothness = lerp(_FlatSmoothness, _SteepSmoothness, blendFactor);
                
                InputData inputData = (InputData)0;
                inputData.positionWS = input.positionWS;
                inputData.normalWS = normalWS;
                inputData.viewDirectionWS = GetWorldSpaceNormalizeViewDir(input.positionWS);
                inputData.shadowCoord = TransformWorldToShadowCoord(input.positionWS);
                
                SurfaceData surfaceData = (SurfaceData)0;
                surfaceData.albedo = albedo.rgb;
                surfaceData.alpha = 1.0;
                surfaceData.metallic = 0.0;
                surfaceData.smoothness = smoothness;
                surfaceData.normalTS = tangentNormal;
                surfaceData.occlusion = 1.0;
                surfaceData.emission = 0.0;
                
                return UniversalFragmentPBR(inputData, surfaceData);
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
