Shader "MoreFun/Unlit Blend Two Textures With Color" {
	Properties{
	_Color("Main Color", Color) = (1, 1, 1, 1) // color
	_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
	_BlendTex("Blend Text", 2D) = "white" {}
	}

	SubShader{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		Lighting Off Fog{ Mode Off }

		Pass{

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
				

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _BlendTex;
			float4 _BlendTex_ST;

			fixed4 _Color;


			struct appdata
			{
				float4 pos : POSITION;
				fixed2 texcoord : TEXCOORD0;
				fixed2 texcoord1 : TEXCOORD1;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				fixed2 texcoord : TEXCOORD0;
				fixed2 texcoord1 : TEXCOORD1;
			};


			v2f vert(appdata v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.pos);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.texcoord1 = TRANSFORM_TEX(v.texcoord1, _BlendTex);
				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
				fixed4 c = tex2D(_MainTex, i.texcoord) * _Color;
				c.a *= tex2D(_BlendTex, i.texcoord1).r * _Color.a;
				return c;
			}
			ENDCG
		}
	}
}