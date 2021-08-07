Shader "Surreal/Jam/Post Processing/Fast Aberration" {
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