Shader "Unlit/Pixel Planet"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Offset ("Offset", Vector) = (0,0,0,0)
		_Aperture ("Aperture", Float) = 178
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
		Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag alpha
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
			float2 _Offset;
			
			float _Aperture;
			static const float PI = 3.14159265359;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv + float2(_Offset.x / 2, _Offset.y / 2), _MainTex); 
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {	
				// Normalize to range from [0,1] to [-1,1]
				float2 n = (i.uv - 0.5 ) * 2 - _Offset;
				// Convert to polar form
				float a = atan2(n.y , n.x ) / PI;
				float r = length(n);

				fixed4 col = fixed4(0,0,0,0);

				if(r >= 0 && r <= 1) {
					// Get the new radius in polar form
					float nr = (r + (1 - sqrt(1-r*r))) / 2;
					if(nr <= 1) {
						// Convert back to cartesian
						float theta = atan2(n.y,n.x); 
						float nx = nr * cos(theta);
						float ny = nr * sin(theta);
						// Normalize  back to range of [0,1] from [-1,1] 
						float2 uv = float2(nx, ny) / 2 + 0.5;
						col = tex2D(_MainTex, uv - _Offset);
						col.a = 1;
					}
				}
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
