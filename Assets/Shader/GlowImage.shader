Shader "UI/GlowImage"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1,1,1,1)
        _GlowColor ("Glow Color", Color) = (0,0,0,0) // 빛나는 색상 (HDR)
        _GlowIntensity ("Glow Intensity", Range(0, 20)) = 1.0 // 빛나는 강도 (높을수록 밝음)
        _UseAlphaForGlow ("Use Alpha for Glow (0=No, 1=Yes)", Float) = 0.0 // 알파값 기준으로 발광 적용 여부
        _AlphaThreshold ("Alpha Threshold", Range(0, 1)) = 0.1 // 알파 기준 발광 적용 임계값 (UseAlphaForGlow=1일 때)
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
        LOD 100

        // 일반적인 투명 블렌딩: SrcAlpha는 소스 이미지의 알파값, OneMinusSrcAlpha는 1-소스 알파값
        // 이를 통해 반투명한 부분이 정확히 블렌딩됩니다.
        Blend SrcAlpha OneMinusSrcAlpha 
        // 더 강한 발광 효과를 원하면 'Blend One One'을 사용하세요.
        // 이는 소스 색상에 발광 색상을 완전히 더하여 더 밝게 만듭니다.
        // Blend One One 

        Cull Off // 양면 렌더링
        Lighting Off // 3D 씬의 라이팅 영향 받지 않음 (UI용)
        ZWrite Off // Z버퍼에 쓰지 않음 (UI가 다른 3D 오브젝트를 가리지 않도록)

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc" // 유니티 내장 셰이더 함수 포함

            // 앱 데이터 (버텍스 셰이더 입력)
            struct appdata
            {
                float4 vertex : POSITION;   // 버텍스 위치
                float2 uv : TEXCOORD0;      // 텍스처 UV 좌표
                float4 color : COLOR;       // UI 요소의 기본 색상 (Image.color에서 전달됨)
            };

            // 버텍스 셰이더에서 프래그먼트 셰이더로 전달될 데이터
            struct v2f
            {
                float2 uv : TEXCOORD0;      // 텍스처 UV 좌표
                float4 vertex : SV_POSITION; // 클립 공간에서의 버텍스 위치 (화면 좌표)
                float4 color : COLOR;       // 버텍스 색상
            };

            sampler2D _MainTex;          // 메인 텍스처
            float4 _MainTex_ST;          // 메인 텍스처 스케일 및 오프셋
            float4 _Color;               // Tint 색상 (Properties에서 설정)
            float4 _GlowColor;           // 발광 색상 (Properties에서 설정)
            float _GlowIntensity;        // 발광 강도 (Properties에서 설정)
            float _UseAlphaForGlow;      // 알파 기준 발광 사용 여부 (0 또는 1)
            float _AlphaThreshold;       // 알파 기준 발광 임계값

            // 버텍스 셰이더: 3D 모델의 각 정점을 처리하여 2D 화면 좌표로 변환
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex); // 오브젝트 공간에서 클립 공간으로 변환
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);     // UV 좌표 변환 (텍스처 tiling/offset 적용)
                o.color = v.color * _Color;                // Image.color와 셰이더 Properties의 _Color를 곱하여 최종 색상 계산
                return o;
            }

            // 프래그먼트 셰이더: 화면의 각 픽셀(프래그먼트)에 대한 최종 색상 계산
            fixed4 frag (v2f i) : SV_Target
            {
                // 1. 기본 이미지 색상 가져오기
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;

                // 2. 발광 색상 계산
                fixed3 glow = _GlowColor.rgb * _GlowIntensity; // 발광 색상에 강도 적용

                // 3. 발광 적용 조건 확인
                // _UseAlphaForGlow가 1 (true)이고, 현재 픽셀의 알파값이 _AlphaThreshold보다 높을 때만 발광 적용
                // 아니면 _UseAlphaForGlow가 0 (false)일 경우 항상 발광 적용
                float alphaForGlowFactor = 1.0;
                if (_UseAlphaForGlow > 0.5) // _UseAlphaForGlow가 1에 가까우면 (true)
                {
                    alphaForGlowFactor = step(_AlphaThreshold, col.a); // 알파가 임계값보다 높으면 1, 아니면 0
                }
                
                // 4. 최종 색상에 발광 더하기
                col.rgb += glow * alphaForGlowFactor; // 계산된 발광 색상을 이미지 색상에 더함

                // 선택 사항: 발광이 강할수록 알파값도 살짝 올려서 블렌딩 시 더 밝게 보이도록
                // 이는 'Blend One One' 사용 시 효과가 더 극대화됩니다.
                // col.a = col.a * (1.0 + glow.r * 0.1); 

                return col; // 최종 픽셀 색상 반환
            }
            ENDCG
        }
    }
}