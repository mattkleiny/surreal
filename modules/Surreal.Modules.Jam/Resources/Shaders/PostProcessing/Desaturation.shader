Shader "Surreal/Jam/Post Processing/Desaturation" {
  SubShader {
    Pass {
      HLSLPROGRAM
      half4 Fragment()
      {
        return half4(1, 0, 1, 1);
      }
      ENDHLSL
    }
  }
}