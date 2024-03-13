Shader "Triniti/Particle/AA_COL_DO_FADE_P2_Dissolve" {
Properties {
 _Color ("Main Color", Color) = (0.5,0.5,0.5,0.5)
 _MainTex ("Particle Texture", 2D) = "white" {}
 _FadeTex ("Fade Texture", 2D) = "white" {}
 _AlphaTex ("AlphaTex", 2D) = "alpha" {}
 _AlphaValue ("AlphaValue", Range(-0.1,1)) = 0
}
SubShader { 
 Tags { "QUEUE"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" }
  BindChannels {
   Bind "vertex", Vertex
   Bind "color", Color
   Bind "texcoord", TexCoord
  }
  ZWrite Off
  Cull Front
  Fog {
   Color (0,0,0,0)
  }
  Blend SrcAlpha One
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag

  struct appdata_t
  {
    float4 vertex : POSITION;
    float4 color : COLOR;
    float4 texcoord0 : TEXCOORD0;
    float4 texcoord1 : TEXCOORD1;
  };

  struct v2f
  {
    float2 texcoord0 : TEXCOORD0;
    float2 texcoord1 : TEXCOORD1;
    float4 color : COLOR;
    float4 vertex : POSITION;
  };

  float4 _Color, _MainTex_ST, _FadeTex_ST, _AlphaTex_ST;

  sampler2D _MainTex, _FadeTex, _AlphaTex;

  float _AlphaValue;

  v2f vert(appdata_t v)
  {
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.texcoord0 = (v.texcoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
    o.texcoord1 = (v.texcoord1.xy * _FadeTex_ST.xy) + _FadeTex_ST.zw;
    o.color = v.color;
    return o;
  }

  float4 frag(v2f i) : SV_TARGET
  {
    float4 outcol;
    float4 col = tex2D(_MainTex, i.texcoord0) * _Color * tex2D(_FadeTex, i.texcoord1);
    float acol = tex2D(_AlphaTex, i.texcoord1).x - _AlphaValue;

    if (acol >= 0.1)
    {
      outcol = col;
    }
    else
    {
      if (acol <= 0)
      {
        outcol = (0).xxxx;
      }
      else
      {
        outcol = col * acol * 10;
      }
    }
    
    return outcol * 2;
  }
  ENDCG
}
}
SubShader { 
 Tags { "QUEUE"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" }
  BindChannels {
   Bind "vertex", Vertex
   Bind "color", Color
   Bind "texcoord", TexCoord
  }
  ZWrite Off
  Cull Front
  Fog {
   Color (0,0,0,0)
  }
  Blend SrcAlpha One
  SetTexture [_MainTex] { ConstantColor [_Color] combine constant * primary }
  SetTexture [_MainTex] { combine texture * previous double }
  SetTexture [_FadeTex] { combine texture * previous }
 }
 Pass {
  Tags { "QUEUE"="Transparent" }
  BindChannels {
   Bind "vertex", Vertex
   Bind "color", Color
   Bind "texcoord", TexCoord
  }
  ZWrite Off
  Fog {
   Color (0,0,0,0)
  }
  Blend SrcAlpha One
  SetTexture [_MainTex] { ConstantColor [_Color] combine constant * primary }
  SetTexture [_MainTex] { combine texture * previous double }
  SetTexture [_FadeTex] { combine texture * previous }
 }
}
}