Shader "MoreFun/Hair_SoftEdge" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Diffuse (RGB) Alpha (A)", 2D) = "gray" {}
        _SpecularTex ("Specular (RGB)", 2D) = "gray" {}
        _BumpMap ("Normal (Normal)", 2D) = "bump" {}
        _Cutoff ("Alpha Cut-Off Threshold", Range(0,1)) = 0.5
		_SpecularIntensity ("Specular Intensity", Range (0, 5)) = 0
    }
 
    SubShader {
        Tags { "RenderType" = "TransparentCutout" }
 			Cull Off
        CGPROGRAM
            #pragma surface surf ExplorationSoftHair fullforwardshadows exclude_path:prepass nolightmap nodirlightmap
            #pragma target 3.0
 
            struct SurfaceOutputHair {
                fixed3 Albedo;
                fixed Alpha;

                fixed3 Normal;
                fixed2 Specular;
                fixed3 Emission;
            };
 
            struct Input
            {
                float2 uv_MainTex;
            };
           
            sampler2D _MainTex, _SpecularTex, _BumpMap;
			float _SpecularIntensity;
            float _Cutoff;
               
            void surf (Input IN, inout SurfaceOutputHair o)
            {
                float4 albedo = tex2D(_MainTex, IN.uv_MainTex);
                clip(albedo.a - _Cutoff);
               
                o.Albedo = albedo.rgb;
                o.Alpha = albedo.a;

                o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
                o.Specular = tex2D(_SpecularTex, IN.uv_MainTex).rgb;
               
                // Stop DX11 complaining.
                o.Emission = fixed3(0.0,0.0,0.0);
            }
 
            inline fixed4 LightingExplorationSoftHair (SurfaceOutputHair s, fixed3 lightDir, fixed3 viewDir, fixed atten)
            {
                viewDir = normalize(viewDir);
                lightDir = normalize(lightDir);
                s.Normal = normalize(s.Normal);
                float NdotL = dot(s.Normal, lightDir);
                float3 h = normalize(lightDir + viewDir);
                float VdotH = dot( viewDir, h );
                             
                fixed4 c;
                // c.rgb = (s.Albedo * saturate(NdotL) * atten * _LightColor0.rgb);
				c.rgb = (s.Albedo * (1 + s.Specular.r * _SpecularIntensity))  * saturate(NdotL) * atten * _LightColor0.rgb;
                c.a = s.Alpha;
               
                return c;
            }
        ENDCG
 
        ZWrite Off
 
        CGPROGRAM
            #pragma surface surf ExplorationSoftHair fullforwardshadows exclude_path:prepass nolightmap nodirlightmap decal:blend
            #pragma target 3.0
 
            struct SurfaceOutputHair {
                fixed3 Albedo;
                fixed Alpha;
                //fixed3 AnisoDir;
                fixed3 Normal;
                fixed2 Specular;
                fixed3 Emission;
            };
 
            struct Input
            {
                float2 uv_MainTex;
            };
           
            sampler2D _MainTex, _SpecularTex, _BumpMap;
            float _Cutoff;
               
            void surf (Input IN, inout SurfaceOutputHair o)
            {
                float4 albedo = tex2D(_MainTex, IN.uv_MainTex);
                clip(-(albedo.a - _Cutoff));
               
                o.Albedo = albedo.rgb;
                o.Alpha = albedo.a;
                o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
                o.Specular = tex2D(_SpecularTex, IN.uv_MainTex).rgb * 40;
               
                // Stop DX11 complaining.
                o.Emission = fixed3(0.0,0.0,0.0);
            }
 
            inline fixed4 LightingExplorationSoftHair (SurfaceOutputHair s, fixed3 lightDir, fixed3 viewDir, fixed atten)
            {
                viewDir = normalize(viewDir);
                lightDir = normalize(lightDir);
                s.Normal = normalize(s.Normal);
                float NdotL = dot(s.Normal, lightDir);
                float3 h = normalize(lightDir + viewDir);
                float VdotH = dot( viewDir, h );

                fixed4 c;
                c.rgb = (s.Albedo * saturate(NdotL) * atten * _LightColor0.rgb);
                c.a = s.Alpha;
               
                return c;
            }
        ENDCG
    }
    FallBack "Transparent/Cutout/VertexLit"
}