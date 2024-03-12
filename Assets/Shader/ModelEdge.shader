Shader "Triniti/Model/ModelEdge" {
Properties {
 _MainTex ("Texture (RGB)", 2D) = "black" {}
 _Color ("Color", Color) = (1,1,1,1)
 _AtmoColor ("Atmosphere Color", Color) = (0.5,0.5,1,1)
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