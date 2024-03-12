Shader "Triniti/Particle/Dissolve_AA" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _Color ("Color", Color) = (1,1,1,1)
 _SecondTex ("Texture", 2D) = "white" {}
 _ColorS ("Color", Color) = (1,1,1,1)
 _AlphaTex ("AlphaTex", 2D) = "alpha" {}
 _AlphaValue ("AlphaValue", Range(-0.1,1)) = 0
}
	//DummyShaderTextExporter
	
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard fullforwardshadows
#pragma target 3.0
		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
		}
		ENDCG
	}
}