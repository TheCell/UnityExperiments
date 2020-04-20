Shader "Unlit/BasicLight"
{
    Properties
    {
		_GlobalAmbient("Global Ambient Color", Color) = (1, 1, 1)
		_LightPosition("Light Position", Vector) = (1, 1, 0)
		_LightColor("Light Color", Vector) = (1, 1, 1)
		_Ke("Emission Color", Color) = (0, 0, 0)
		_Ka("Ambient Reflectance RGB Channel", Vector) = (1, 1, 1)
		_Kd("Diffuse Color", Vector) = (1, 1, 1)
		_Ks("Specular Color", Vector) = (1, 1, 1)
		_Shininess("Shininess", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

			// see what to use for the structs
			// https://docs.unity3d.com/Manual/SL-VertexProgramInputs.html
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 position : SV_POSITION;
				float4 objectPosition : POSITION1;
				float3 normal : NORMAL;
            };

			float3 _GlobalAmbient;
			float3 _LightPosition;
			float3 _LightColor;
			float3 _Ke;
			float3 _Ka;
			float3 _Kd;
			float3 _Ks;
			float _Shininess;

            v2f vert (appdata v)
            {
                v2f o;
                o.position = UnityObjectToClipPos(v.vertex);
                o.uv = float2(1, 1);
				o.normal = v.normal;
				o.objectPosition = v.vertex;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float3 P = i.objectPosition.xyz;
				float3 N = i.normal;

				float3 emissive = _Ke;
				float3 ambient = _Ka * _GlobalAmbient;

				// Compute the diffuse term
				// normalized vector towards light pos
				float3 L = normalize(_LightPosition - P);
				float diffuseLight = max(dot(N, L), 0);
				float3 diffuse = _Kd * _LightColor * diffuseLight;

				// Compute the specular term
				float3 V = normalize(_WorldSpaceCameraPos - P);
				float3 H = normalize(L + V);
				float specularLight = pow(max(dot(N, H), 0), _Shininess);

				if (diffuseLight <= 0)
				{
					specularLight = 0;
				}
				float3 specular = _Ks * _LightColor * specularLight;

                // sample the texture
                fixed4 col = float4(L,1);
				col.xyz = emissive + ambient + diffuse + specular;
				col.xyz /= 4;
                return col;
            }
            ENDCG
        }
    }
}
