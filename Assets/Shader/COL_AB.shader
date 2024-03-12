Shader "Triniti/Character/COL_AB" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _MainTex ("MainTex(RGB)", 2D) = "" {}
}
SubShader { 
 Tags { "QUEUE"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" }
  Color [_Color]
  Blend SrcAlpha OneMinusSrcAlpha
  SetTexture [_MainTex] { combine texture * primary }
 }
}
}