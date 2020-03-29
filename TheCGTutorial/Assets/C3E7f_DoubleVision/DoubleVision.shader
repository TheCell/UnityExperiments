Shader "Unlit/DoubleVision"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_leftOffset ("LeftOffset", Vector) = (-0.05, 0, 0, 0)
		_rightOffset ("RightOffset", Vector) = (0.05, 0, 0, 0)
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
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float2 _leftOffset;
			float2 _rightOffset;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = float4(0, 0, 0, 1);
				float2 leftTexCoord = i.uv + _leftOffset;
				float2 rightTexCoord = i.uv + _rightOffset;
				fixed4 leftColor = tex2D(_MainTex, leftTexCoord);
				fixed4 rightColor = tex2D(_MainTex, rightTexCoord);
				col = lerp(leftColor, rightColor, 0.5);
                return col;
            }
            ENDCG
        }
    }
}
