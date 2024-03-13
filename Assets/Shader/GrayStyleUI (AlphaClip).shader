Shader "Triniti/Extra/GrayStyleUI (AlphaClip)" {
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
    float2 texcoord1 : TEXCOORD1;
  };

  sampler2D _MainTex;

  float _GrayFactor, _ColorF;

  float4 _MainTex_ST;

  v2f vert(appdata_t v)
  {
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.texcoord0 = v.texcoord0.xy;
    o.texcoord1 = v.vertex.xy * _MainTex_ST.xy + _MainTex_ST.zw;
    return o;
  }

  float4 frag(v2f i) : SV_TARGET
  {
    float4 col = tex2D(_MainTex, i.texcoord0);
    float gray = col.x + col.y + col.z * 0.333 * _ColorF;
    float2 ab = abs(i.texcoord1);
    if (1.0 - max(ab.x, ab.y) < 0.0)
    {
      col.w = 0.0;
    }
    return float4(lerp(col.x, gray, _GrayFactor), lerp(col.y, gray, _GrayFactor), lerp(col.z, gray, _GrayFactor), col.w);
  }
  ENDCG
 }
}
}