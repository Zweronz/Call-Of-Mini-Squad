Shader "Triniti/Extra/GrayStyleUI" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _GrayFactor ("Gray Factor", Range(0,1)) = 1
 _ColorF ("Gray Color", Range(0,2)) = 1
}
SubShader { 
 LOD 200
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  ZWrite Off
  Cull Off
  Fog { Mode Off }
  Blend SrcAlpha OneMinusSrcAlpha
  Offset -1, -1
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag

  struct appdata_t
  {
    float4 vertex : POSITION;
    float4 texcoord0 : TEXCOORD0;
  };

  struct v2f
  {
    float4 vertex : POSITION;
    float2 texcoord0 : TEXCOORD0;
  };

  sampler2D _MainTex;

  float _GrayFactor, _ColorF;

  v2f vert(appdata_t v)
  {
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.texcoord0 = v.texcoord0.xy;
    return o;
  }

  float4 frag(v2f i) : SV_TARGET
  {
    float4 col = tex2D(_MainTex, i.texcoord0);
    float gray = (col.x * 0.1) + (col.y * 0.6) + (col.z * 0.3) * _ColorF;
    return float4(lerp(col.x, gray, _GrayFactor), lerp(col.y, gray, _GrayFactor), lerp(col.z, gray, _GrayFactor), col.w);
  }
  ENDCG
 }
}
}