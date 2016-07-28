Shader "Custom/FresnelSurface" {
	Properties {
		_BaseColor("Base Color", Color) = (0, 0, 0, 0)
		_FresnelColor("Fresnel Color", Color) = (0, 0, 0, 0)
		_GlowSize("Glow Size", Float) = 1
		_SelfIllum("Self Illumination", Float) = 1
		_SelfIllumBias("Self Illumination Bias", Float) = 1
		_Metallic("Metallic", Range(0, 1)) = 0
		_Glossiness("Glossiness", Range(0, 1)) = 0

		_EdgeMap ("Edgemap", 2D) = "bump" {}
		_EdgeBias("Edge Bias", Range(0, 1)) = 0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM

		#pragma surface surf Standard fullforwardshadows

		struct Input {
			float3 viewDir;
		};

		void surf (Input IN, inout SurfaceOutputStandard o) {

			half fresnel = dot (normalize(IN.viewDir), o.Normal);
			o.Alpha = <...>
			o.Albedo = <...>
			o.Metallic = <...>
			o.Smoothness = <...>
			o.Alpha = <...>
		}
		ENDCG
	}
	FallBack "Diffuse"
}
