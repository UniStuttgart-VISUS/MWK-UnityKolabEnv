Shader "Custom/CameraStyleShader" {
  Properties {
    _MainTex("Texture", 2D) = "white" {}
  }

  SubShader {
    Pass {
      CGPROGRAM
      #pragma vertex vert_img
      #pragma fragment frag
      #include "UnityCG.cginc" // required for v2f_img
       
      // Properties
      sampler2D _MainTex;

      float4 frag(v2f_img input) : COLOR {
        half4 texcol = tex2D (_MainTex, input.uv);
        texcol.rgb = dot(texcol.rgb, float3(0.3, 0.59, 0.11));
        return texcol;
      }
      ENDCG
}}}