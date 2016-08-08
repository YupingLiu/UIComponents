// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "MoreFun/Lightmap with Opaque Texture" {
	Properties
	{
	_MainTex("Base (RGB)", 2D) = "white" {}
	_Ambient("Ambient (RGBA)", Color) = (.5, .5, .5, 1)
}

	SubShader
	{
		Tags{ "Queue" = "Geometry" "RenderType" = "Opaque" }
		LOD 100

		Pass
		{

			Lighting Off

			CGPROGRAM
#pragma vertex vert
#pragma fragment frag
	

#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;

			// sampler2D unity_Lightmap; //Far lightmap.
			// float4	unity_LightmapST; //Lightmap atlasing data.

			uniform fixed4 _Ambient;

			struct appdata
			{
				float4 vertex : POSITION;
				fixed2 texcoord : TEXCOORD0;
				fixed2 texcoord1 : TEXCOORD1;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				fixed2 texcoord : TEXCOORD0;
				fixed2 lmuv : TEXCOORD1;
			};


			v2f vert(appdata v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.texcoord = v.texcoord;
				o.lmuv = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;


				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
				fixed4 c = tex2D(_MainTex, i.texcoord);

				c.rgb *= DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmuv)) * _Ambient;

				return c;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
