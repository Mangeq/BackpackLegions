Shader "Blackpine/Stroke" {
 SubShader {
  Tags { "Queue"="Transparent" "RenderType"="Transparent" }
  Pass {
   Cull Off ZWrite Off Blend SrcAlpha OneMinusSrcAlpha
   CGPROGRAM
   #pragma vertex vert
   #pragma fragment frag
   #include "UnityCG.cginc"
   struct vertex { float4 pos:POSITION; fixed4 color:COLOR; };
   struct output { float4 pos:SV_POSITION; fixed4 color:COLOR; };
   output vert(vertex v){ output o; o.pos=UnityObjectToClipPos(v.pos); o.color=v.color; return o; }
   fixed4 frag(output i):SV_Target { return i.color; }
   ENDCG
  }
 }
}
