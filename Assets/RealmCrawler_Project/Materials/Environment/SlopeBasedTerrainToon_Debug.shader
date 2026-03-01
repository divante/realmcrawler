Shader "Custom/URP/SlopeBasedTerrainToon_Debug"
{
    Properties
    {
        _FlatTex ("Flat Texture (Grass)", 2D) = "white" {}
        _FlatColor ("Flat Tint", Color) = (1,1,1,1)
        
        _MudTex ("Mud Texture", 2D) = "white" {}
        _MudColor ("Mud Tint", Color) = (0.6,0.5,0.4,1)
        
        _NoiseMask ("Noise Mask (Grass-Mud Blend)", 2D) = "white" {}
        _NoiseScale ("Noise Scale", Float) = 1.0
        _NoiseContrast ("Noise Contrast", Range(0, 2)) = 1.0
        _NoiseBrightness ("Noise Brightness", Range(-1, 1)) = 0.0
        
        [Enum(Final, 0, NoiseOnly, 1, GrassMudBlend, 2)] _DebugMode("Debug Mode", Float) = 0
    }
    
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };
            
            TEXTURE2D(_FlatTex);
            SAMPLER(sampler_FlatTex);
            TEXTURE2D(_MudTex);
            SAMPLER(sampler_MudTex);
            TEXTURE2D(_NoiseMask);
            SAMPLER(sampler_NoiseMask);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _FlatColor;
                float4 _MudColor;
                float _NoiseScale;
                float _NoiseContrast;
                float _NoiseBrightness;
                float _DebugMode;
            CBUFFER_END
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = positionInputs.positionCS;
                output.positionWS = positionInputs.positionWS;
                output.uv = input.uv;
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                float2 noiseUV = input.positionWS.xz * _NoiseScale;
                
                half noiseMask = SAMPLE_TEXTURE2D(_NoiseMask, sampler_NoiseMask, noiseUV).r;
                noiseMask = saturate(noiseMask + _NoiseBrightness);
                noiseMask = saturate((noiseMask - 0.5) * _NoiseContrast + 0.5);
                
                if (_DebugMode < 0.5)
                {
                    half4 grassAlbedo = SAMPLE_TEXTURE2D(_FlatTex, sampler_FlatTex, input.positionWS.xz * 0.1) * _FlatColor;
                    half4 mudAlbedo = SAMPLE_TEXTURE2D(_MudTex, sampler_MudTex, input.positionWS.xz * 0.1) * _MudColor;
                    return lerp(mudAlbedo, grassAlbedo, noiseMask);
                }
                else if (_DebugMode < 1.5)
                {
                    return half4(noiseMask, noiseMask, noiseMask, 1);
                }
                else
                {
                    return noiseMask > 0.5 ? half4(0, 1, 0, 1) : half4(0.6, 0.3, 0.1, 1);
                }
            }
            ENDHLSL
        }
    }
}
