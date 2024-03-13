Shader "Triniti/Model/ModelEdge" {
  Properties {
   _MainTex ("Texture (RGB)", 2D) = "black" {}
   _Color ("Color", Color) = (1,1,1,1)
   _AtmoColor ("Atmosphere Color", Color) = (0.5,0.5,1,1)
  }
  SubShader { 
   Pass {
    BindChannels {
     Bind "vertex", Vertex
     Bind "normal", Normal
     Bind "texcoord", TexCoord0
    }
    CGPROGRAM
    #pragma vertex vert
    #pragma fragment frag
  
    struct appdata_t
    {
      float4 vertex : POSITION;
      float3 normal : NORMAL;
      float4 texcoord0 : TEXCOORD0;
    };
  
    struct v2f
    {
      float2 texcoord0 : TEXCOORD0;
      float texcoord1 : TEXCOORD1;
      float4 vertex : POSITION;
    };
    
    float4 _Color, _AtmoColor, _MainTex_ST;
  
    sampler2D _MainTex;
  
    v2f vert(appdata_t v)
    {
      v2f o;
      float3 coord = normalize((mul(UNITY_MATRIX_MV, float4(normalize(v.normal), 0.0))).xyzw);
      o.vertex = UnityObjectToClipPos(v.vertex);
      o.texcoord0 = v.texcoord0 * _MainTex_ST.xy + _MainTex_ST.zw;
      o.texcoord1 = clamp((coord.x * coord.x) + (coord.y * coord.y) - (coord.z * coord.z), 0.0, 1.0);
      return o;
    }
  
    float4 frag(v2f i) : SV_TARGET
    {
      return lerp(tex2D(_MainTex, i.texcoord0) * _Color, _AtmoColor, i.texcoord1.xxxx);
    }
    ENDCG
   }
  }
  }