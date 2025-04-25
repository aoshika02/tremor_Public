Shader "Unlit/RandomNoise"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Seed ("Seed", Int) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            int _Seed;

          	float random(float2 st, int seed)
			{
				return frac(sin(dot(st, float2(12.9898, 78.233)) + seed) * 43758.5453123);
			}

            float blockNoise(float2 seeds,int seed)
            {
                return random(floor(seeds),seed);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col;
				col.rgb = random((blockNoise(i.uv * 500,_Seed)),_Seed);
				col.a = 1;
				return col;
            }

            ENDCG
        }
    }
}