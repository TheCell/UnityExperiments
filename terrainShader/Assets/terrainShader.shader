Shader "Custom/terrainShader"
{
	Properties
	{
		// Color Input setup
		_RockColor ("RockColor", Color) = (1,1,1,1)
		_RockHighlightColor ("RockHighlightColor", Color) = (1,1,1,1)
		_GrassColor ("GrassColor", Color) = (1,1,1,1)
		_GrassHighlightColor ("GrassHighlightColor", Color) = (1,1,1,1)
		_ImpactColor ("ImpactColor", Color) = (1,1,1,1)

		// Texture Input setup
		//_MainTex ("Albedo (RGB)", 2D) = "white" {} // white, black or gray possible
		_RockTex ("Rock Texture", 2D) = "white" {}
		_GrassTex ("Grass Texture", 2D) = "white" {}
		_ImpactTex ("Impact Texture", 2D) = "white" {}
		_TextureScale ("Texture Scale", Float) = 1.0

		// custom options setup
		_RockStart ("Rock starting worldCoordinates", Float) = 135.0
		_RockToGrassOverlap ("Rock and Grass overlap Size", Float) = 4.0
		/*
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		*/
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 300

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		// declare variables used and link them
		sampler2D _RockTex, _GrassTex, _ImpactTex;
		fixed4 _RockColor;
		fixed4 _RockHighlightColor;
		fixed4 _GrassColor;
		fixed4 _GrassHighlightColor;
		fixed4 _ImpactColor;
		float _TextureScale;
		float _RockStart;
		float _RockToGrassOverlap;

		struct Input
		{
			float2 uvRockColor;
			float3 worldPos;
			float3 worldNormal;
		};

		/*
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		*/

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			float3 worldPosition;
			float3 worldNormal;
			float3 blending;
			fixed4 c;
			fixed4 c2;
			float2 UV;
			float transitionFromGrassToRockPercent;

			worldPosition = abs(IN.worldPos);
			worldNormal = IN.worldNormal;
			UV = IN.uvRockColor;
			UV = IN.worldPos.xy;

			// see https://gamedevelopment.tutsplus.com/articles/use-tri-planar-texture-mapping-for-better-terrain--gamedev-13821
			blending = normalize(max(worldNormal, 0.00001)); // Force weights to sum to 1.0
			float b = (blending.x + blending.y + blending.z);
			blending /= float3(b, b, b);

			if (worldPosition.y > (_RockStart + _RockToGrassOverlap))
			{
				float4 xaxis = tex2D(_RockTex, worldPosition.yz * _TextureScale);
				float4 yaxis = tex2D(_RockTex, worldPosition.xz * _TextureScale);
				float4 zaxis = tex2D(_RockTex, worldPosition.xy * _TextureScale);

				c = xaxis * blending.x + yaxis * blending.y + zaxis * blending.z;
				//c = tex2D(_RockTex, UV * _TextureScale);
				// set picked color as base
				o.Albedo =  max(_RockColor, c.rgb * _RockHighlightColor);
				/*
				// set picked color as highlight
				c.r = min(0.3, c.r);
				c.g = min(0.3, c.g);
				c.b = min(0.3, c.b);
				o.Albedo = c.rgb * _RockColor;
				*/
			}
			else if (worldPosition.y > _RockStart)
			{
				// multiple problems here:
				// 1. transition from green to gray (best with some noise and not linear)
				// 2. blend stone and grass highlights into each other
				transitionFromGrassToRockPercent = lerp(_RockStart, _RockStart + _RockToGrassOverlap, worldPosition.y);
				transitionFromGrassToRockPercent = 1.0 / (_RockStart + _RockToGrassOverlap) * transitionFromGrassToRockPercent;
				c = tex2D(_GrassTex, UV * _TextureScale);
				c2 = tex2D(_RockTex, UV * _TextureScale);
				o.Albedo = 0.3 * transitionFromGrassToRockPercent * _GrassColor + max((c.rgb * _GrassColor), (c2.rgb * _RockColor));
			}
			else
			{
				float4 xaxis = tex2D(_GrassTex, worldPosition.yz * _TextureScale);
				float4 yaxis = tex2D(_GrassTex, worldPosition.xz * _TextureScale);
				float4 zaxis = tex2D(_GrassTex, worldPosition.xy * _TextureScale);

				c = xaxis * blending.x + yaxis * blending.y + zaxis * blending.z;
				//c = tex2D(_RockTex, UV * _TextureScale);
				// set picked color as base
				o.Albedo = max(_GrassColor, c.rgb * _GrassHighlightColor);
				//c = tex2D(_GrassTex, UV * _TextureScale);
				//c += tex2D(_RockTex, UV * _TextureScale) * 1.0 - min(1.0, (1.0 / (_RockStart + _RockToGrassOverlap) * worldPosition.y));
				// set picked color as base
				//o.Albedo = _GrassColor + c.rgb * _GrassColor;
				/*
				// set picked color as highlight
				c.r = min(0.3, c.r);
				c.g = min(0.3, c.g);
				c.b = min(0.3, c.b);
				o.Albedo = c.rgb * _GrassColor;
				*/
			}

			/*
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
			*/
		}
		ENDCG
	}
	FallBack "Diffuse"
}
