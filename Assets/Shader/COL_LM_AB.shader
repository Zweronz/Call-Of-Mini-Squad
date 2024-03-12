Shader "Triniti/Scene/COL_LM_AB" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _MainTex ("MainTex(RGB)", 2D) = "" {}
 _LightMap ("Lightmap (RGB)", 2D) = "white" {}
}
SubShader { 
 Tags { "QUEUE"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" }
  BindChannels {
   Bind "vertex", Vertex
   Bind "texcoord", TexCoord0
   Bind "texcoord1", TexCoord1
  }
  Color [_Color]
  Fog { Mode Off }
  Blend SrcAlpha OneMinusSrcAlpha
  SetTexture [_MainTex] { combine texture * primary }
  SetTexture [_LightMap] { combine texture * previous }
 }
}
}