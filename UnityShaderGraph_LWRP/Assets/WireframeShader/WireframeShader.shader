Shader "Custom/WireframeShader"
{
    Properties
    {
		// see https://twitter.com/phi6/status/1196849129042911232
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
		
		_WireframeThicknessNear ("Wireframe Thickness Near", Float) = 0.1
		_WireframeThicknessFar ("Wireframe Thickness Far", Float) = 0.1
		_WireframeCameraDistance ("Wireframe Camera Distance", Float) = 0.1
		_WireframeColor ("Wireframe Color", Color) = (1, 0, 0, 1)
		
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
		float _WireframeThicknessNear;
		float _WireframeThicknessFar;
		float _WireframeCameraDistance;
		fixed4 _WireframeColor;

		float edgeFactor(float3 bary, float dist)
		{
			float3 d = fwidth(bary);
			float3 a3 = smoothstep(float3(0, 0, 0), d * lerp(_WireframeThicknessNear, _WireframeThicknessFar, clamp(dist/_WireframeCameraDistance, 0.0, 1.0)), bary);
			return min(min(a3.x, a3.y), a3.z);
		}

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			/*
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
			*/
			o.Albedo = lerp(_WireframeColor.rgb, _Color.rgb, edgeFactor(IN.bary, IN.cameraDist));
			o.Alpha = _Color.a;
			o.Metallic = 0.0;
			o.Smoothness = 0.0;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
