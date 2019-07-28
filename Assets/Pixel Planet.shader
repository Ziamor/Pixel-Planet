Shader "Unlit/Pixel Planet"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_BarrelPower ("Barrel Power", Float) = 1
		_Offset ("Barrel Power", Vector) = (0,0,0,0)
		_PlanetSize ("Barrel Power", Float) = 1
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
            // make fog work
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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float4 _MainTex_TexelSize;
			float _BarrelPower;
			float2 _Offset;
			float _PlanetSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

			float2 Distort(float2 p)
			{
				float theta  = atan2(p.y, p.x);
				float radius = length(p);
				radius = pow(radius, _BarrelPower);
				p.x = radius * cos(theta);
				p.y = radius * sin(theta);
				return 0.5 * (p + 1.0);
			}

            fixed4 frag (v2f i) : SV_Target
            {
				float2 p = (i.uv * 2 - 1);
				float2 t = float2(_Time.x, 0);
                // sample the texture
                fixed4 col = tex2D(_MainTex, (Distort(p) + _Offset + t));
				if(length(p) > _PlanetSize){
					col = fixed4(0,0,0,1);
				}
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
