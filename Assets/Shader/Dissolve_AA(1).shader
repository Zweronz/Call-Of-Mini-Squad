Shader "Triniti/Particle/Dissolve_AA" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _Color ("Color", Color) = (1,1,1,1)
 _SecondTex ("Texture", 2D) = "white" {}
 _ColorS ("Color", Color) = (1,1,1,1)
 _AlphaTex ("AlphaTex", 2D) = "alpha" {}
 _AlphaValue ("AlphaValue", Range(-0.1,1)) = 0
}
SubShader { 
 LOD 200
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  Cull Off
  Blend SrcAlpha One
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag

  struct appdata_t
  {
    float4 vertex : POSITION;
    float4 texcoord0 : TEXCOORD0;
    float4 texcoord1 : TEXCOORD1;
  };

  struct v2f
  {
    float4 vertex : POSITION;
    float2 texcoord0 : TEXCOORD0;
    float2 texcoord1 : TEXCOORD1;
  };

  sampler2D _MainTex, _SecondTex, _AlphaTex;

  float4 _MainTex_ST, _Color, _ColorS;

  float _AlphaValue;

  v2f vert(appdata_t v)
  {
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.texcoord0 = v.texcoord0 * _MainTex_ST.xy + _MainTex_ST.zw;
    o.texcoord1 = v.texcoord1;
    return o;
  }

  float4 frag(v2f i) : SV_TARGET
  {
    float4 col = tex2D(_MainTex, i.texcoord0) * _Color;
    float4 scol = tex2D(_SecondTex, i.texcoord0) * _ColorS;
    float acol = tex2D(_AlphaTex, i.texcoord1).x - _AlphaValue;

    if (acol >= 0.1)
    {
      return col;
    }

    if (acol <= 0.0)
    {
      return scol;
    }

    return col * acol * 10.0 + (scol * (1.0 - (acol * 10.0)));
  }
  ENDCG
}
}
Fallback "Diffuse"
}
