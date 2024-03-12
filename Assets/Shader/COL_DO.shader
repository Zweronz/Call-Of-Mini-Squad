Shader "Triniti/Character/COL_DO" {
Properties {
 _Color ("Main Color", Color) = (0.5,0.5,0.5,1)
 _MainTex ("MainTex(RGB)", 2D) = "" {}
}
SubShader { 
 Tags { "QUEUE"="Geometry" }
 Pass {
  Tags { "QUEUE"="Geometry" }
  SetTexture [_MainTex] { ConstantColor [_Color] combine texture * constant double }
 }
}
}