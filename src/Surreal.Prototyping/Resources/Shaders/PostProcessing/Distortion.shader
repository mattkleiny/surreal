Shader "Surreal/Prototyping/Post Processing/Distortion" {
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
