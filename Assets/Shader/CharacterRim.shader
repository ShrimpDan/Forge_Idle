// 2D Sprite 전용 그라데이션/글로우 쉐이더
Shader "Custom/SpriteGradientGlow"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Sprite Color Tint", Color) = (1,1,1,1)
        _GradientColor ("Gradient Color", Color) = (0, 0.5, 0.5, 1) // 그라데이션 색상
        _GradientSize ("Gradient Size", Range(0, 10)) = 3 // 그라데이션의 퍼짐 
        _GradientFalloff ("Gradient Falloff", Range(0.1, 5.0)) = 1.5 // 그라데이션 r감쇠ㅏ
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha 

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            fixed4 _Color;
            fixed4 _GradientColor;
            float _GradientSize;
            float _GradientFalloff; // 그라데이션 감쇠율 속성

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
               //픽셀 가져옴
                fixed4 spriteColor = tex2D(_MainTex, i.uv) * i.color;

               //샘플링 알파값
                float blurredAlpha = 0.0;
                float2 texel = _MainTex_TexelSize.xy * _GradientSize;
                
                //그리드 효과 만들기 쉬벌 뭐가 틀리넉ㄴ데
                blurredAlpha += tex2D(_MainTex, i.uv + texel * float2(-1, -1)).a;
                blurredAlpha += tex2D(_MainTex, i.uv + texel * float2( 0, -1)).a;
                blurredAlpha += tex2D(_MainTex, i.uv + texel * float2( 1, -1)).a;
                blurredAlpha += tex2D(_MainTex, i.uv + texel * float2(-1,  0)).a;
                blurredAlpha += tex2D(_MainTex, i.uv + texel * float2( 0,  0)).a;
                blurredAlpha += tex2D(_MainTex, i.uv + texel * float2( 1,  0)).a;
                blurredAlpha += tex2D(_MainTex, i.uv + texel * float2(-1,  1)).a;
                blurredAlpha += tex2D(_MainTex, i.uv + texel * float2( 0,  1)).a;
                blurredAlpha += tex2D(_MainTex, i.uv + texel * float2( 1,  1)).a;
                blurredAlpha /= 9.0;

                float glowMask = saturate(blurredAlpha);
                glowMask = pow(glowMask, _GradientFalloff);
                glowMask *= (1.0 - saturate(spriteColor.a)); 


                //마지막 합쳐
                fixed4 finalColor = _GradientColor * glowMask;
                
                    //스프라이트 겹쳐
                finalColor.rgb = lerp(finalColor.rgb, spriteColor.rgb, spriteColor.a);
                finalColor.a = saturate(spriteColor.a + finalColor.a);

                return finalColor;
            }
            ENDCG
        }
    }
}
