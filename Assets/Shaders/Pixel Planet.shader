// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/Pixel Planet"
{
    Properties
    {
        _PlanetTexture ("Texture", 2D) = "white" {}
		_WaterMask("Water Mask", 2D) = "black" {}
		_Tint ("Tint Color", Color) = (1,1,1,1)

		_Offset ("Offset", Vector) = (0,0,0,0)
		_Radius ("Radius", Float) = 1

		_ShadowStrength ("Shadow Strength", Float) = 1
		_ShadowColor ("Shadow Color", Color) = (1,1,1,1)

		_AtmosphereColor ("Atmosphere Color", Color) = (0,0,1,1)
		_AtmosphereFalloff ("Atmosphere Falloff", Float) = 1
		_AtmosphereStrength ("Atmosphere Strength", Range(0,1)) = 1
		_AtmosphereLevels ("Atmosphere Levels", Int) = 1

		_LightDir ("Light Direction", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "LightMode" = "ForwardBase"}
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
				float3 worldvertpos : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _PlanetTexture;
            float4 _PlanetTexture_ST;

			sampler2D _WaterMask;
            float4 _WaterMask_ST;

			fixed4 _Tint;

			float2 _Offset;			
			float _Radius;

			float _ShadowStrength;
			fixed4 _ShadowColor;

			fixed4 _AtmosphereColor;
			float _AtmosphereFalloff;
			float _AtmosphereStrength;
			int _AtmosphereLevels;

			float4 _LightDir;
			static const float PI = 3.14159265359;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv + float2(_Offset.x / 2, _Offset.y / 2), _PlanetTexture); 
				o.worldvertpos = mul(unity_ObjectToWorld, v.vertex).xyz;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

			float circlePointToSpherePoint(float3 center, float radius, float3 ray_origin, float3 ray_dir) {
				float3 oc = ray_origin - center;
				float a = dot(ray_dir,ray_dir);
				float b = 2.0 * dot(oc, ray_dir);
				float c = dot(oc,oc) - radius * radius;
				float discriminant = b*b - 4*a*c;

				if(discriminant < 0) {
					return -1;
				}else {
					return (-b - sqrt(discriminant)) / (2.0*a);
				}
			}

            fixed4 frag (v2f i) : SV_Target
            {	
				// Normalize to range from [0,1] to [-1,1]
				float2 n = (i.uv - 0.5 ) * 2 - _Offset;
				// Convert to polar form
				float a = atan2(n.y , n.x ) / PI;
				float r = length(n) / _Radius;

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

						fixed4 planetColor = tex2D(_PlanetTexture, uv - _Offset) * _Tint;
						//planetColor.a = 1;

						float3 viewdir = normalize(_WorldSpaceCameraPos-i.worldvertpos);
						float3 center = float3(0,0,0);
						float3 origin = float3(n.x,n.y, -1);				
						float3 dir = float3(0,0,1);

						float discriminant = circlePointToSpherePoint(center, _Radius, origin, dir);
						fixed4 atmoColor = _AtmosphereColor;

						float3 normal = normalize((origin + dir * discriminant) - center);
						float ndot = saturate(dot(normal, _LightDir.xyz));
						float atmoStrength = pow(1 - saturate(round(dot(normal, viewdir) * _AtmosphereLevels) / _AtmosphereLevels),_AtmosphereFalloff) * _AtmosphereStrength * planetColor.a;

						float steppedNdotl = step(0.0001, fixed4(ndot,ndot,ndot,1)) * _ShadowStrength + (1 - _ShadowStrength);

						fixed4 shadow = (1 - steppedNdotl) * _ShadowColor * planetColor.a * _ShadowStrength;

						fixed4 color = planetColor;//lerp(planetColor, atmoColor, atmoStrength);

						return color * steppedNdotl + shadow;
					}
				}				
                return fixed4(0,0,0,0);
            }
            ENDCG
        }
    }
}
