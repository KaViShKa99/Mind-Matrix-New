Shader "UI/DiagonalShiny"
{
    Properties
    {
        _MainTex("Base Texture", 2D) = "white" {}
        _ShineColor("Shine Color", Color) = (1,1,1,1)
        _ShineWidth("Shine Width", Range(0.01, 0.5)) = 0.2
        _ShineOffset("Shine Offset", Range(-1,2)) = -1
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _ShineColor;
            float _ShineWidth;
            float _ShineOffset;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // Diagonal UV (0 bottom-left → 2 top-right)
                float diag = i.uv.x + i.uv.y;

                // Remap so shine fully sweeps the button
                float shine = smoothstep(_ShineOffset, _ShineOffset + _ShineWidth, diag) *
                              (1.0 - smoothstep(_ShineOffset + _ShineWidth, _ShineOffset + _ShineWidth*1.5, diag));

                col.rgb += _ShineColor.rgb * shine * _ShineColor.a;

                return col;
            }
            ENDHLSL
        }
    }
}
