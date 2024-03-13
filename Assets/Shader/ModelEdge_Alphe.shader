Shader "Triniti/Model/ModelEdge_Alphe" {
  Properties {
      _MainTex ("Texture (RGBA)", 2D) = "white" {}
      _BlendTex ("Blend Texture (RGBA)", 2D) = "black" {}
      _Color ("Color", Color) = (1, 1, 1, 1)
      _AtmoColor ("Atmosphere Color", Color) = (0.5, 0.5, 1, 1)
      _Pow ("Edge Length", Range(0.5, 8)) = 1
  }
  
  SubShader {
      Tags { "QUEUE"="Transparent" }
      
      Pass {
          Name "PLANETBASE"
          Tags { "QUEUE"="Transparent" }
          
          BindChannels {
              Bind "vertex", Vertex
              Bind "normal", Normal
              Bind "texcoord", TexCoord0
          }
          
          Blend SrcAlpha OneMinusSrcAlpha
          CGPROGRAM
          #pragma vertex vert
          #pragma fragment frag

          #include "UnityCG.cginc"

          struct appdata_t
          {
              float4 vertex : POSITION;
              float3 normal : NORMAL;
              float2 texcoord0 : TEXCOORD0;
              float2 texcoord1 : TEXCOORD1;
              float2 texcoord2 : TEXCOORD2;
              float2 texcoord3 : TEXCOORD3;
          };

    struct v2f {
              float4 vertex : SV_POSITION;
      float2 texcoord0 : TEXCOORD0;
              float2 texcoord1 : TEXCOORD1;
              float2 texcoord2 : TEXCOORD2;
              float2 texcoord3 : TEXCOORD3;
              UNITY_FOG_COORDS(1)
              UNITY_VERTEX_OUTPUT_STEREO
          };

          sampler2D _MainTex;
          sampler2D _BlendTex;
          float4 _Color;
          float4 _AtmoColor;
          float _Pow;
    float4 _MainTex_ST;

          void vert (inout appdata_t v)
          {
              float3 coord = normalize((mul(UNITY_MATRIX_MV, float4(normalize(v.normal), 0.0))).xyzw);
              v.vertex = UnityObjectToClipPos(v.vertex);
              v.texcoord0 = coord.xy;
              v.texcoord2 = ((v.texcoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
              v.texcoord1 = pow(clamp((((coord.x * coord.x) + (coord.y * coord.y)) - ((coord.z * coord.z) * 0.5)), 0.0, 1.0), _Pow);
          }

    void frag (v2f i, out half4 fragColor : SV_Target)
    {
        fragColor = lerp(tex2D(_MainTex, i.texcoord2) * _Color, _AtmoColor, half4(i.texcoord3, i.texcoord3));
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
              Bind "texcoord", TexCoord0
              Bind "texcoord1", TexCoord1
          }
          Color [_Color]
          Blend SrcAlpha OneMinusSrcAlpha
          SetTexture [_MainTex] { combine texture * primary }
          SetTexture [_MainTex2] { ConstantColor [_AtmoColor] combine previous * constant }
      }
  }
}
