Shader "PostEffect/Grayscale"
{
    Properties
    {
        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}
        _Blend ("Blend", Float) = 1.0
        _isDebug ("Debug", int) = 0
    }
    SubShader
    {
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Blend;
            int _isDebug;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                if (_isDebug == 1) { return col; }
                fixed grayscale = dot(col.rgb, fixed3(0.2126, 0.7152, 0.0722));
                fixed4 outCol = col * (1 - _Blend) + grayscale * _Blend ;
                return outCol;
            }
            ENDCG
        }
    }
}