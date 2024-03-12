Shader "FX PACK 1/Particles/Trail Additive Turbulence" {
Properties {
 _MainTex ("Main_Texture", 2D) = "white" {}
 _Color01 ("Color", Color) = (1,1,1,1)
 _Blend_Texture ("Blend_Texture_01", 2D) = "white" {}
 _Color02 ("Color", Color) = (1,1,1,1)
 _Blend_Texture01 ("Blend_Texture_02", 2D) = "black" {}
 _Color03 ("Color", Color) = (1,1,1,1)
 _MaskTex ("Mask_Texture", 2D) = "black" {}
 _Speed00 ("Main_Texture_Speed", Float) = 0
 _Speed01 ("Blend_Texture_01_Speed", Float) = 1
 _Speed02 ("Blend_Texture_02_Speed", Float) = 1
 _LightenMain ("Brightness_Main", Float) = 1
 _Lighten ("Brightness_Blend", Float) = 1
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