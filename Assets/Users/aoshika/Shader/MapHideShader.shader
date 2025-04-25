Shader "Unlit/MapHideShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _border ("Border", Float) = 0.5
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue" = "Transparent"
        }
        
        
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
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
                 UNITY_FOG_COORDS(1)
            };

            fixed4 _Color;
            float _border;
            sampler2D _MainTex;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                UNITY_TRANSFER_FOG(o, o.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                const float distancePerPixel = fwidth(i.uv.x);
                const float degree = _border*2-2;
                const float distance = abs(1-i.uv.x) + abs(1-i.uv.y) + degree;
                const float alpha = abs(i.uv.x) + abs(i.uv.y) - degree;
                fixed4 color=_Color;
                // fixed4 color = tex2D(_MainTex, i.uv);
                // UNITY_APPLY_FOG(i.fogCoord, color);

                const float distanceInPixels = distance / distancePerPixel;
                color.a *= alpha * saturate(0.5 - distanceInPixels);
                return color;
            }
            ENDCG
        }
    }
}
