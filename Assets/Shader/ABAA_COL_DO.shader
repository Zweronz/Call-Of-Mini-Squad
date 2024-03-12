Shader "Triniti/Particle/ABAA_COL_DO" {
Properties {
 _Color ("Main Color", Color) = (0.5,0.5,0.5,0.5)
 _MainTex ("Particle Texture", 2D) = "white" {}
 _Color2 ("Main Color 2", Color) = (0.5,0.5,0.5,0.5)
 _MainTex2 ("Particle Texture 2", 2D) = "white" {}
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
  Cull Off
  Fog {
   Color (0,0,0,0)
  }
  Blend SrcAlpha OneMinusSrcAlpha
  SetTexture [_MainTex] { ConstantColor [_Color] combine constant * primary }
  SetTexture [_MainTex] { combine texture * previous double }
 }
 Pass {
  Tags { "QUEUE"="Transparent" }
  BindChannels {
   Bind "vertex", Vertex
   Bind "color", Color
   Bind "texcoord", TexCoord
  }
  ZWrite Off
  Cull Off
  Fog {
   Color (0,0,0,0)
  }
  Blend SrcAlpha One
  SetTexture [_MainTex2] { ConstantColor [_Color2] combine constant * primary }
  SetTexture [_MainTex2] { combine texture * previous double }
 }
}
}