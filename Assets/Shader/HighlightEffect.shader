// Unity Shader
// 이 쉐이더는 UI Image와 함께 사용되어 화면의 특정 영역을 밝게 강조하는 효과를 만듭니다.
Shader "UI/HighlightEffect"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _Center ("Center", Vector) = (0.5, 0.5, 0, 0) // 하이라이트 원의 중심 (화면 좌표, 0-1)
        _Radius ("Radius", Float) = 0.1 // 원의 반지름 (0-1)
        _Softness ("Softness", Float) = 0.05 // 원의 경계 부드러움 정도

        _HighlightColor ("Highlight Color", Color) = (0,0,0,0) // 밝은 영역 색상 (투명)
        _DimColor ("Dim Color", Color) = (0,0,0,0.7) // 어두운 영역 색상 (반투명 검정)
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="true"
        }

        LOD 100
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

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            // Properties
            sampler2D _MainTex;
            fixed4 _Color;
            float4 _Center;
            float _Radius;
            float _Softness;
            fixed4 _HighlightColor;
            fixed4 _DimColor;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
               
                float dist = distance(IN.texcoord, _Center.xy);

               
               
                float mask = smoothstep(_Radius, _Radius - _Softness, dist);

                
                fixed4 finalColor = lerp(_DimColor, _HighlightColor, mask);

             
                finalColor.a *= IN.color.a;

                return finalColor;
            }
            ENDCG
        }
    }
}
