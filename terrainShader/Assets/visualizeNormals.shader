Shader "Unlit/visualizeNormals"
{
	Properties
	{
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

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 color : COLOR0;
			};
			
			v2f vert (appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos (v.vertex);
				o.color = v.normal;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return half4 (i.color, 1);
			}
			ENDCG
		}
	}
	Fallback "VertexLit"
}
