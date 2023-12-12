Shader "Custom/LambertianDiffuse" {
    Properties{
        _Color("Main Color", Color) = (0,.2,.8,1)
    }
        SubShader{
            Tags { "RenderType" = "Opaque" }
            LOD 100

            CGPROGRAM
            #pragma surface surf Lambert

            sampler2D _MainTex;

            struct Input {
                float2 uv_MainTex;
            };

            fixed4 _Color;

            void surf(Input IN, inout SurfaceOutput Lambert) {
                Lambert.Albedo = _Color.rgb;
            }
            ENDCG
    }
        FallBack "Diffuse"
}