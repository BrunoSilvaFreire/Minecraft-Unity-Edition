using UnityEngine;

namespace Minecraft.Scripts.Utility {
    public class PerlinNoise {
        public static float[,] Generate(int w, int h, int octaveCount, float amplitude, int seed) {
            var baseNoise = GenerateWhiteNoise(w, h, seed);
            var width = baseNoise.GetLength(0);
            var height = baseNoise.GetLength(1);

            //Um array de matrizes (geralmente usando 8, no final cada uma dessas matrizes será somada para gerar a imagem final
            var smoothNoise = new float[octaveCount, width, height];

            const float persistence = 0.50f;

            for (var i = 0; i < octaveCount; i++) {
                //cada loop do for gera uma nova matriz mais suave que a anteior, todas baseadas na primeira matriz de noise gerada no programa
                var smoothMatrix = GenerateSmoothNoise(baseNoise, i);
                for (var x = 0; x < smoothMatrix.GetLength(0); x++) {
                    for (var y = 0; y < smoothMatrix.GetLength(1); y++) {
                        smoothNoise[i, x, y] = smoothMatrix[x, y];
                    }
                }
            }

            var perlinNoise = new float[width, height];
            var totalAmplitude = 0.0f;

            //For feito para somar cada uma das matrizes em uma unica
            for (var octave = octaveCount - 1; octave >= 0; octave--) {
                amplitude *= persistence;
                totalAmplitude += amplitude;

                for (var x = 0; x < width; x++) {
                    for (var y = 0; y < height; y++) {
                        //Soma as matrizes em uma unica
                        perlinNoise[x, y] += smoothNoise[octave, x, y] * amplitude;
                    }
                }
            }

            //normalização (traz os valores de volta para o intervalo de 0 a 1
            for (var x = 0; x < width; x++) {
                for (var y = 0; y < height; y++) {
                    perlinNoise[x, y] /= totalAmplitude;
                }
            }

            return perlinNoise;
        }


        //Gera uma matriz com ruido aleatório (usando uma seed, uma mesma seed sempre gera o mesmo resultado)
        static float[,] GenerateWhiteNoise(int w, int h, int seed) {
            var rand = new System.Random(seed);
            var noise = new float[w, h];
            for (var y = 0; y < h; y++) {
                for (var x = 0; x < w; x++) {
                    noise[x, y] = (float) rand.NextDouble();
                }
            }

            return noise;
        }

        static float[,] GenerateSmoothNoise(float[,] baseNoise, int octave) {
            var width = baseNoise.GetLength(0);
            var height = baseNoise.GetLength(1);

            var smoothNoise = new float[width, height];

            var samplePeriod = 1 << octave; //"move" o "1" para a esquerda. ex: octave = 1:  1 << 1 -> 10(binário) = 2
            //ex2: octave = 2: 1 << 2 -> 100(binário) = 4
            //ex3: octave = 3: 1 << 3 -> 1000(binário) = 8
            var sampleFrequency = 1.0f / samplePeriod;

            //Sinceramente não faço ideia do que essa maluquice abaixo faz
            for (var i = 0; i < width; i++) {
                var sample_i0 = (i / samplePeriod) * samplePeriod;
                var sample_i1 = (sample_i0 + samplePeriod) % width;
                var horizontal_blend = (i - sample_i0) * sampleFrequency;

                for (var j = 0; j < height; j++) {
                    var sample_j0 = (j / samplePeriod) * samplePeriod;
                    var sample_j1 = (sample_j0 + samplePeriod) % height;
                    var vertical_blend = (j - sample_j0) * sampleFrequency;


                    var top = Mathf.Lerp(baseNoise[sample_i0, sample_j0], baseNoise[sample_i1, sample_j0], horizontal_blend);
                    var bottom = Mathf.Lerp(baseNoise[sample_i0, sample_j1], baseNoise[sample_i1, sample_j1], horizontal_blend);


                    smoothNoise[i, j] = Mathf.Lerp(top, bottom, vertical_blend);
                }
            }

            return smoothNoise;
        }
    }
}