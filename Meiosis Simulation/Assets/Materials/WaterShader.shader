Shader "Custom/SimpleWater"
{
    Properties
    {
        _MainTex("Texture", 2D) = "blue" {} // Основная текстура воды
        _Speed("Wave Speed", Float) = 0.3    // Скорость анимации волн
        _WaveStrength("Wave Strength", Float) = 0.02 // Амплитуда волн (меньше для плавности)
        _WaveFrequency("Wave Frequency", Float) = 3.0 // Частота волн
        _Color("Water Color", Color) = (0.3, 0.5, 0.9, 1) // Цвет воды
    }
        SubShader
        {
            Tags { "RenderType" = "Transparent" }
            LOD 100
            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 3.0

                sampler2D _MainTex;
                float4 _Color;
                float _Speed;
                float _WaveStrength;
                float _WaveFrequency;

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                // Вершинный шейдер
                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }
                // Функция случайного смещения
                float Random(float2 uv)
                {
                    return frac(sin(dot(uv, float2(15.9898, 85.233))) * 43758.5453);
                }

                // Улучшенная генерация плавных волн без направленного движения
                float Wave(float2 uv, float time)
                {
                    // Круговое движение волн
                    float angle = time * _Speed;
                    float2 direction = float2(cos(angle), sin(angle));

                    float wave1 = sin(dot(uv, direction * _WaveFrequency) + time * _Speed);
                    float wave2 = cos(dot(uv, direction.yx * _WaveFrequency * 0.5) + time * _Speed * 0.5);
                    float wave3 = sin(dot(uv, direction * 0.3 + uv.yx * 0.4) + time * _Speed * 0.5);

                    // Добавляем более мягкое случайное смещение
                    float randomOffset = (Random(uv + time * 0.1) - 0.5) * 0.02;

                    return (wave1 * 0.4 + wave2 * 0.3 + wave3 * 0.3) * _WaveStrength + randomOffset;
                }


                // Фрагментный шейдер
                float4 frag(v2f i) : SV_Target
                {
                    float2 uv = i.uv;

                    // Генерация мягких волн
                    float time = _Time.y;
                    float waveOffset = Wave(uv, time);
                    uv += clamp(waveOffset, 0, 0.02);
                    uv = clamp(uv, 0.01, 0.5);
                    // Убираем резкие переходы
                    uv = frac(uv); // Ограничиваем UV в пределах [0,1]

                    // Получаем цвет текстуры и смешиваем с цветом воды
                    float4 texColor = tex2D(_MainTex, uv) * _Color;

                    // Добавляем эффект блеска воды
                    float reflection = smoothstep(0.4, 0.8, waveOffset);
                    texColor.rgb += reflection * 0.1;

                    return float4(texColor.rgb, texColor.a);
                }
                ENDCG
            }
        }
}
