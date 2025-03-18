Shader "Custom/WaterLikeOpaqueSphere"
{
    Properties
    {
        _Color("Base Color", Color) = (0, 0.5, 1, 1)
        _Metallic("Metallic", Range(0,1)) = 0.9
        _Smoothness("Smoothness", Range(0,1)) = 1
        _FresnelPower("Fresnel Intensity", Range(0, 5)) = 2
        _WaveSpeed("Wave Speed", Range(0, 5)) = 1
        _WaveStrength("Wave Strength", Range(0, 0.2)) = 0.05

    }
        SubShader
    {
        Tags { "Queue" = "Geometry" "RenderType" = "Opaque" }
        LOD 300

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        struct Input
        {
            float3 worldPos;    // Мировая позиция для волн
            float3 worldNormal;
            float3 viewDir;
            INTERNAL_DATA
        };

        fixed4 _Color;
        half _Metallic;
        half _Smoothness;
        half _FresnelPower;
        half _WaveSpeed;
        half _WaveStrength;

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float3 normal = WorldNormalVector(IN, float3(0, 0, 1));

            // Френель (оставляем как было)
            float fresnel = pow(1.0 - saturate(dot(normal, IN.viewDir)), _FresnelPower);

            // Анимированные волны (смешение нормалей)
            float wave = sin(_WaveSpeed * _Time.y + IN.worldPos.x * 2.0) *
                         cos(_WaveSpeed * _Time.y + IN.worldPos.z * 2.0) * _WaveStrength;
            normal += wave;

            // Основной цвет + Френель
            o.Albedo = _Color.rgb + _Color.rgb * fresnel;

            // Делаем сферу блестящей
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;
            o.Normal = normalize(normal);
        }
        ENDCG
    }
        FallBack "Standard"
}
