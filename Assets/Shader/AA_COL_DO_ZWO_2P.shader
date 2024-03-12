Shader "Triniti/Particle/AA_COL_DO_ZWO_2P" {
Properties {
 _Color ("Main Color", Color) = (0.5,0.5,0.5,0.5)
 _MainTex ("Particle Texture", 2D) = "white" {}
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
  Fog {
   Color (0,0,0,0)
  }
  Blend SrcAlpha One
  ColorMask 0
  SetTexture [_MainTex] { combine texture * primary }
 }
 Pass {
  Tags { "QUEUE"="Transparent" }
  BindChannels {
   Bind "vertex", Vertex
   Bind "color", Color
   Bind "texcoord", TexCoord
  }
  Fog {
   Color (0,0,0,0)
  }
  Blend SrcAlpha One
  Offset -1, -1
  SetTexture [_MainTex] { ConstantColor [_Color] combine constant * primary }
  SetTexture [_MainTex] { combine texture * previous double }
 }
}
}