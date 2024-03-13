Shader "Triniti/Model/CartoonRendering" {
Properties {
 _Color ("Main Color", Color) = (0.5,0.5,0.5,1)
 _OutlineColor ("Outline Color", Color) = (0,0,0,1)
 _Outline ("Outline width", Range(0.002,10)) = 1
 _MainTex ("Base (RGB)", 2D) = "white" {}
}
SubShader { 
 Tags { "QUEUE"="Geometry-1" "RenderType"="Opaque-1" }
 Pass {
  Tags { "QUEUE"="Geometry-1" "RenderType"="Opaque-1" }
  ZWrite Off
  Cull Front
  Blend SrcAlpha OneMinusSrcAlpha
  ColorMask RGB
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag

  float4 _OutlineColor;
  float _Outline;

  struct appdata_t
  {
    float4 vertex : POSITION;
    float3 normal : NORMAL;
    float4 texcoord0 : TEXCOORD0;
  };

  struct v2f
  {
    float4 vertex : POSITION;
    float4 color : COLOR;
  };

  v2f vert(appdata_t v)
  {
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
		float3 norm = mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal);
		norm.x *= UNITY_MATRIX_P[0][0];
		norm.y *= UNITY_MATRIX_P[1][1];
		o.vertex.xy += norm.xy * o.vertex.z * (_Outline * 30);
		o.color = _OutlineColor;
    return o;
  }

  float4 frag(v2f i) : SV_TARGET
  {
    return i.color;
  }
  ENDCG
}
}
}