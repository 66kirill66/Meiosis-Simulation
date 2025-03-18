Shader "Custom/WaterWithWaveShapes"
{
    Properties
    {
        _Color("Base Color", Color) = (0, 0.5, 1, 1)
        _Metallic("Metallic", Range(0,1)) = 0.9
        _Smoothness("Smoothness", Range(0,1)) = 1
        _FresnelPower("Fresnel Intensity", Range(0, 5)) = 2
        _WaveSpeed("Wave Speed", Range(0, 5)) = 1
        _WaveStrength("Wave Strength", Range(0, 0.2)) = 0.05
        _WaveType("Wave Type", Range(0, 3)) = 0 // 0 - синус, 1 - шум, 2 - радиальные, 3 - смесь
    }
        SubShader
    {
        Tags { "Queue" = "Geometry" "RenderType" = "Opaque" }
        LOD 300

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        struct Input
        {
            float3 worldPos;
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
        half _WaveType;

        // Функция псевдо-шумовых волн (Perlin-like)
        float random(float2 uv)
        {
            return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float3 normal = WorldNormalVector(IN, float3(0, 0, 1));

            float wave = 0;

            // Выбираем тип волн в зависимости от параметра _WaveType
            if (_WaveType < 1)
            {
                // 🔹 СИНУСОИДАЛЬНЫЕ ВОЛНЫ (сетчатый паттерн)
                wave = sin(_WaveSpeed * _Time.y + IN.worldPos.x * 2.0) *
                       cos(_WaveSpeed * _Time.y + IN.worldPos.z * 2.0) * _WaveStrength;
            }
            else if (_WaveType < 2)
            {
                // 🔹 ПЕРЛИН-ШУМ (более случайные формы)
                wave = random(IN.worldPos.xz + _Time.y * _WaveSpeed) * _WaveStrength;
            }
            else if (_WaveType < 3)
            {
                // 🔹 РАДИАЛЬНЫЕ ВОЛНЫ (расходящиеся круги)
                float distance = length(IN.worldPos.xz) * 2.0;
                wave = sin(distance - _Time.y * _WaveSpeed) * _WaveStrength;
            }
            else
            {
                // 🔹 СМЕШАННЫЕ ВОЛНЫ (разные паттерны)
                wave = (sin(_WaveSpeed * _Time.y + IN.worldPos.x * 2.0) +
                        random(IN.worldPos.xz + _Time.y * _WaveSpeed) +
                        sin(length(IN.worldPos.xz) * 2.0 - _Time.y * _WaveSpeed)) * _WaveStrength / 3.0;
            }

            normal += wave;

            // Френель
            float fresnel = pow(1.0 - saturate(dot(normal, IN.viewDir)), _FresnelPower);

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
