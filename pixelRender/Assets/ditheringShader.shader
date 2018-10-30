Shader "Custom/ditheringShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		float2 screenPos:TEXCOORD2;

		struct Input {
			float2 uv_MainTex;
		};

		float4x4 indexMatrix4x4 = {0,  8,  2,  10,
                                     12, 4,  14, 6,
                                     3,  11, 1,  9,
                                     15, 7,  13, 5};

		float indexValue()
		{
			int x = int(screenPos.x % 4);
			int y = int(screenPos.y % 4);
			return indexMatrix4x4[(x + y * 4)] / 16.0;
		}

		float dither(float color)
		{
			float closestColor = (color < 0.5) ? 0 : 1;
			float secondClosestColor = 1 - closestColor;
			float d = indexValue();
			float distance = abs(closestColor - color);
			return (distance < d) ? closestColor : secondClosestColor;
		}
		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			float ditherVal = dither(c.g);
			float3 ditherColor = float3(ditherVal, ditherVal, ditherVal);
			c.rgb = ditherColor;
			//c = float4(float3(dither(c)), 1);
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			//o.Alpha = c.a;

			
		}

		ENDCG
	}
	FallBack "Diffuse"
}
