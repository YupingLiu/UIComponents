Shader "MoreFun/LightProbe with Transparent Texture"
{
	Properties
	{
	_MainTex("Base (RGB)", 2D) = "white" {}
	_Ambient("Ambient (RGB)", Color) = (1.0, 1.0, 1.0, 1.0)
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent" "LightMode" = "ForwardBase" "Queue"="Transparent+1"}
		LOD 100

		Pass
		{
			Tags{ "LightMode" = "ForwardBase" }
			Cull Off
			ZWrite On
			Lighting Off
			Blend SrcAlpha OneMinusSrcAlpha

	
			Fog{Mode Linear Color (0.87,0.87,0.87,1) Density 0.1  Range 0,500}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest	

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;


			uniform fixed4 _Ambient;

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				fixed4 SHLighting : TEXCOORD2;
			};


			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord;

				float3 worldNormal = mul((float3x3)_Object2World, v.normal);
				float3 shl = ShadeSH9(float4(worldNormal, 1));

				o.SHLighting.rgb = shl * _Ambient;
				o.SHLighting.a = _Ambient.a;

				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
				fixed4 c = tex2D(_MainTex, i.uv);

				c.rgb *= i.SHLighting;
				c.a *= i.SHLighting.a;

				return c;
			}
			ENDCG
		}
	}
}