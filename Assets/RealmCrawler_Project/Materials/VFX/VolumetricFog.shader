Shader "Custom/VolumetricFog"
{
    Properties
    {
        [Header(Fog Colors)]
        _FogColor ("Fog Color", Color) = (0.5, 0.5, 0.5, 1)
        _FogDensity ("Fog Density", Range(0, 1)) = 0.5
        
        [Header(Height Gradient)]
        _HeightFalloff ("Height Falloff", Range(0, 10)) = 2
        _FogBottom ("Fog Bottom Height", Float) = 0
        _FogTop ("Fog Top Height", Float) = 10
        
        [Header(Noise)]
        _NoiseTexture ("Noise Texture", 2D) = "white" {}
        _NoiseScale ("Noise Scale", Range(0, 5)) = 1
        _NoiseStrength ("Noise Strength", Range(0, 1)) = 0.3
        _NoiseSpeed ("Noise Speed", Vector) = (0.1, 0.1, 0, 0)
        
        [Header(Depth Fade)]
        _DepthFadeDistance ("Depth Fade Distance", Range(0, 50)) = 10
        
        [Header(Rendering)]
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull Mode", Float) = 2
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }
        
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }
            
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull [_Cull]
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
                float fogFactor : TEXCOORD3;
            };
            
            TEXTURE2D(_NoiseTexture);
            SAMPLER(sampler_NoiseTexture);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _FogColor;
                float _FogDensity;
                float _HeightFalloff;
                float _FogBottom;
                float _FogTop;
                float4 _NoiseTexture_ST;
                float _NoiseScale;
                float _NoiseStrength;
                float2 _NoiseSpeed;
                float _DepthFadeDistance;
            CBUFFER_END
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                
                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = positionInputs.positionCS;
                output.positionWS = positionInputs.positionWS;
                output.uv = TRANSFORM_TEX(input.uv, _NoiseTexture);
                output.screenPos = ComputeScreenPos(output.positionCS);
                output.fogFactor = ComputeFogFactor(output.positionCS.z);
                
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                float2 screenUV = input.screenPos.xy / input.screenPos.w;
                
                float sceneDepth = LinearEyeDepth(SampleSceneDepth(screenUV), _ZBufferParams);
                float fragDepth = LinearEyeDepth(input.positionCS.z, _ZBufferParams);
                float depthDifference = sceneDepth - fragDepth;
                
                float depthFade = saturate(depthDifference / _DepthFadeDistance);
                
                float heightGradient = saturate((input.positionWS.y - _FogBottom) / (_FogTop - _FogBottom));
                heightGradient = pow(1.0 - heightGradient, _HeightFalloff);
                
                float2 noiseUV = input.positionWS.xz * _NoiseScale + _Time.y * _NoiseSpeed;
                float noise = SAMPLE_TEXTURE2D(_NoiseTexture, sampler_NoiseTexture, noiseUV).r;
                noise = lerp(1.0, noise, _NoiseStrength);
                
                float finalAlpha = _FogDensity * heightGradient * noise * depthFade;
                
                half4 color = _FogColor;
                color.a *= finalAlpha;
                
                color.rgb = MixFog(color.rgb, input.fogFactor);
                
                return color;
            }
            ENDHLSL
        }
    }
    
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
