Shader "Custom/ditheringShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
		_paletteSize ("paletteSize", Int) = 8
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
// Upgrade NOTE: excluded shader from DX11 because it uses wrong array syntax (type[size] name)
#pragma exclude_renderers d3d11
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		float4 screenPos:SV_Position;
		float3 palette[8];
		int paletteSize = 8;

		struct Input {
			float2 uv_MainTex;
		};

		float4x4 indexMatrix4x4 = {0,  8,  2,  10,
                                     12, 4,  14, 6,
                                     3,  11, 1,  9,
                                     15, 7,  13, 5};

		float indexValue()
		{
			int x = screenPos.x % 4;
			int y = screenPos.y % 4;
			return indexMatrix4x4[(x + y * 4)] / 16.0;
		}

		float hueDistance(float h1, float h2)
		{
			float diff = abs((h1 - h2));
			return min(abs((1.0 - diff)), diff);
		}

		void closestColors(float hue, out float3 result[2] )
		{
			float3 closest = float3(-2.0, 0.0, 0.0);
			float3 secondClosest = float3(-2.0, 0.0, 0.0);
			float3 temp;

			for (int i = 0; i < paletteSize; ++i)
			{
				temp = palette[i];
				float tempDistance = hueDistance(temp.x, hue);
				if (tempDistance < hueDistance(closest.x, hue))
				{
					secondClosest = closest;
					closest = temp;
				}
				else
				{
					if (tempDistance < hueDistance(secondClosest.x, hue))
					{
						secondClosest = temp;
					}
				}
			}

			result[0] = closest;
			result[1] = secondClosest;
		}

		float dither(float color)
		{
			float closestColor = (color < 0.5) ? 0 : 1;
			float secondClosestColor = 1 - closestColor;
			float d = indexValue();
			float distance = abs(closestColor - color);
			return (distance < d) ? closestColor : secondClosestColor;
		}

		float3 HUEtoRGB(in float H)
		{
			float R = abs(H * 6 - 3) - 1;
			float G = 2 - abs(H * 6 - 2);
			float B = 2 - abs(H * 6 - 4);
			return saturate(float3(R,G,B));
		}

		float3 HSLtoRGB(in float3 HSL)
		{
			float3 RGB = HUEtoRGB(HSL.x);
			float C = (1 - abs(2 * HSL.z - 1)) * HSL.y;
			return (RGB - 0.5) * C + HSL.z;
		}

		float3 ditherWithColor (float3 color)
		{
			float3 hsl = HSLtoRGB(color);
			float3 colors[2];
			closestColors(hsl.x, colors);
			float3 closestColor = colors[0];
			float3 secondClosestColor = colors[1];
			float d = indexValue();
			float hueDiff = hueDistance(hsl.x, closestColor.x) / hueDistance(secondClosestColor.x, closestColor.x);
			
			return HSLtoRGB(hueDiff < d ? closestColor : secondClosestColor);
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
			float ditherVal = dither(c.rgb / 3.0);
			float3 ditherColor = float3(ditherVal, ditherVal, ditherVal);
			float3 outputColor = ditherWithColor(c);
			c.rgb = outputColor;
			//c = float4(float3(dither(c)), 1);
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			//o.Alpha = c.a;
		}

		ENDCG
	}
	FallBack "Diffuse"
}
