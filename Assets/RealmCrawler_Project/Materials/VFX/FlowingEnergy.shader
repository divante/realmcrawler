Shader "RealmCrawler/VFX/FlowingEnergy"
{
    Properties
    {
        [Header(Textures)]
        _MainTex ("Main Texture (RGB)", 2D) = "white" {}
        _MaskTex ("Mask Texture (A)", 2D) = "white" {}
        
        [Header(Mask Controls)]
        _MaskContrast ("Mask Contrast", Range(0.1, 5)) = 1
        _MaskMin ("Mask Min (Black Point)", Range(0, 1)) = 0
        _MaskMax ("Mask Max (White Point)", Range(0, 1)) = 1
        [Toggle] _InvertMask ("Invert Mask", Float) = 0
        
        [Header(Colors)]
        _Color ("Tint Color", Color) = (1,1,1,1)
        _EmissionStrength ("Emission Strength", Range(0, 10)) = 2
        
        [Header(Panning)]
        _PanSpeedX ("Pan Speed X", Range(-5, 5)) = 0.5
        _PanSpeedY ("Pan Speed Y", Range(-5, 5)) = 0.2
        
        [Header(Secondary Layer Optional)]
        [Toggle(USE_SECONDARY)] _UseSecondary ("Use Secondary Layer", Float) = 0
        _SecondaryTex ("Secondary Texture (RGB)", 2D) = "white" {}
        _SecondaryPanSpeedX ("Secondary Pan Speed X", Range(-5, 5)) = -0.3
        _SecondaryPanSpeedY ("Secondary Pan Speed Y", Range(-5, 5)) = 0.4
        _SecondaryBlend ("Secondary Blend", Range(0, 1)) = 0.5
        
        [Header(Rendering)]
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Src Blend", Float) = 5
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Dst Blend", Float) = 1
        [Enum(Off, 0, On, 1)] _ZWrite ("Z Write", Float) = 0
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Float) = 0
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
            
            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]
            Cull [_Cull]
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature USE_SECONDARY
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };
            
            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            TEXTURE2D(_MaskTex);
            SAMPLER(sampler_MaskTex);
            
            #ifdef USE_SECONDARY
            TEXTURE2D(_SecondaryTex);
            SAMPLER(sampler_SecondaryTex);
            #endif
            
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _MaskTex_ST;
                float4 _Color;
                float _EmissionStrength;
                float _PanSpeedX;
                float _PanSpeedY;
                float _MaskContrast;
                float _MaskMin;
                float _MaskMax;
                float _InvertMask;
                
                #ifdef USE_SECONDARY
                float4 _SecondaryTex_ST;
                float _SecondaryPanSpeedX;
                float _SecondaryPanSpeedY;
                float _SecondaryBlend;
                #endif
            CBUFFER_END
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionHCS = vertexInput.positionCS;
                output.uv = input.uv;
                output.color = input.color;
                
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                float2 panOffset = float2(_PanSpeedX, _PanSpeedY) * _Time.y;
                float2 pannedUV = input.uv + panOffset;
                
                half4 mainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, pannedUV * _MainTex_ST.xy + _MainTex_ST.zw);
                half4 maskTex = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, input.uv * _MaskTex_ST.xy + _MaskTex_ST.zw);
                
                half4 finalColor = mainTex;
                
                #ifdef USE_SECONDARY
                float2 secondaryPanOffset = float2(_SecondaryPanSpeedX, _SecondaryPanSpeedY) * _Time.y;
                float2 secondaryPannedUV = input.uv + secondaryPanOffset;
                half4 secondaryTex = SAMPLE_TEXTURE2D(_SecondaryTex, sampler_SecondaryTex, secondaryPannedUV * _SecondaryTex_ST.xy + _SecondaryTex_ST.zw);
                finalColor = lerp(mainTex, secondaryTex, _SecondaryBlend);
                #endif
                
                half maskValue = max(maskTex.r, max(maskTex.g, max(maskTex.b, maskTex.a)));
                
                maskValue = (maskValue - _MaskMin) / (_MaskMax - _MaskMin);
                maskValue = saturate(maskValue);
                maskValue = pow(maskValue, _MaskContrast);
                
                if (_InvertMask > 0.5)
                {
                    maskValue = 1.0 - maskValue;
                }
                
                finalColor *= _Color;
                finalColor *= input.color;
                finalColor.rgb *= _EmissionStrength;
                finalColor.a *= maskValue;
                
                return finalColor;
            }
            ENDHLSL
        }
    }
    
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
