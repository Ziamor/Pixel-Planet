using UnityEngine;

public static class NoiseGenerator {

    public enum NormalizeMode { LOCAL, GLOBAL };

    public static float[,] GenerateNoiseMap(int mapWidth, int mapheight, NoiseSettings settings, Vector2 sampleCentre) {
        float[,] noiseMap = new float[mapWidth, mapheight];

        System.Random prng = new System.Random(settings.seed);
        Vector2[] octaveOffsets = new Vector2[settings.octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < settings.octaves; i++) {
            float offsetX = prng.Next(-100000, 100000) + settings.offset.x + sampleCentre.x;
            float offsety = prng.Next(-100000, 100000) - settings.offset.y - sampleCentre.y;
            octaveOffsets[i] = new Vector2(offsetX, offsety);

            maxPossibleHeight += amplitude;
            //maxPossibleHeight += amplitude * Mathf.Pow(persistance, i);
            amplitude *= settings.persistance;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapheight / 2f;
        for (int y = 0; y < mapheight; y++) {
            for (int x = 0; x < mapWidth; x++) {
                amplitude = 1;
                frequency = 1;

                float noiseHeight = 0;

                for (int i = 0; i < settings.octaves; i++) {
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / settings.scale * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[i].y) / settings.scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= settings.persistance;
                    frequency *= settings.lacunarity;
                }

                if (settings.useRidgeNoise) {
                    noiseHeight = 2 * (0.5f - Mathf.Abs(0.5f - noiseHeight));
                }

                if (noiseHeight > maxLocalNoiseHeight) {
                    maxLocalNoiseHeight = noiseHeight;
                }
                if (noiseHeight < minLocalNoiseHeight) {
                    minLocalNoiseHeight = noiseHeight;
                }

                noiseMap[x, y] = noiseHeight;
                if (settings.normalizeMode == NormalizeMode.GLOBAL) {
                    float normalizedHeight = (noiseMap[x, y] + 1f) / (2f * (maxPossibleHeight * settings.normalizedMaxHeightModifier));
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, float.MaxValue);
                }
            }
        }
        if (settings.normalizeMode == NormalizeMode.LOCAL) {
            for (int y = 0; y < mapheight; y++) {
                for (int x = 0; x < mapWidth; x++) {
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                }
            }
        }
        return noiseMap;
    }

    public static float[,] GenerateBlueNoiseMap(int mapWidth, int mapheight, NoiseSettings settings, Vector2 sampleCentre) {
        float[,] whiteNoiseMap = new float[mapWidth, mapheight];

        System.Random prng = new System.Random(settings.seed);
        Vector2[] octaveOffsets = new Vector2[settings.octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < settings.octaves; i++) {
            float offsetX = prng.Next(-100000, 100000) + settings.offset.x + sampleCentre.x;
            float offsety = prng.Next(-100000, 100000) - settings.offset.y - sampleCentre.y;
            octaveOffsets[i] = new Vector2(offsetX, offsety);

            maxPossibleHeight += amplitude;
            //maxPossibleHeight += amplitude * Mathf.Pow(persistance, i);
            amplitude *= settings.persistance;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = (mapWidth) / 2f;
        float halfHeight = (mapheight) / 2f;
        for (int y = 0; y < mapheight; y++) {
            for (int x = 0; x < mapWidth; x++) {
                amplitude = 1;
                frequency = 1;

                float noiseHeight = 0;

                for (int i = 0; i < settings.octaves; i++) {
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / settings.scale * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[i].y) / settings.scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= settings.persistance;
                    frequency *= settings.lacunarity;
                }

                if (settings.useRidgeNoise) {
                    noiseHeight = 2 * (0.5f - Mathf.Abs(0.5f - noiseHeight));
                }

                if (noiseHeight > maxLocalNoiseHeight) {
                    maxLocalNoiseHeight = noiseHeight;
                }
                if (noiseHeight < minLocalNoiseHeight) {
                    minLocalNoiseHeight = noiseHeight;
                }

                whiteNoiseMap[x, y] = noiseHeight;
                if (settings.normalizeMode == NormalizeMode.GLOBAL) {
                    float normalizedHeight = (whiteNoiseMap[x, y] + 1f) / (2f * (maxPossibleHeight * settings.normalizedMaxHeightModifier));
                    whiteNoiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, float.MaxValue);
                }
            }
        }

        if (settings.normalizeMode == NormalizeMode.LOCAL) {
            for (int y = 0; y < mapheight; y++) {
                for (int x = 0; x < mapWidth; x++) {
                    whiteNoiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, whiteNoiseMap[x, y]);
                }
            }
        }

        float[,] noiseMap = new float[mapWidth, mapheight];
        for (int y = 0; y < mapheight; y++) {
            for (int x = 0; x < mapWidth; x++) {
                float max = -1;
                for (int y2 = y - settings.r; y2 <= y + settings.r; y2++) {
                    for (int x2 = x - settings.r; x2 <= x + settings.r; x2++) {
                        if (x2 < 0 || y2 < 0 || x2 >= mapWidth || y2 >= mapheight)
                            continue;
                        float e = whiteNoiseMap[x2, y2];
                        if (e > max) {
                            max = e;
                        }
                    }
                }
                if (whiteNoiseMap[x, y] == max) {
                    noiseMap[x, y] = 1;
                }
            }
        }
        return noiseMap;
    }
}

[System.Serializable]
[CreateAssetMenu()]
public class NoiseSettings : ScriptableObject{
    public NoiseGenerator.NormalizeMode normalizeMode;

    public float normalizedMaxHeightModifier = 1f;

    public float scale = 40;

    [Range(0, 1)]
    public float persistance = 0.5f;
    public float lacunarity = 1f;

    public int octaves = 5;
    public int seed = 1234;

    public Vector2 offset;

    public bool useRidgeNoise = false;

    public int r = 1;
    public float strength = 1f;

    public float baseRoughness = 1f;
    public float roughness = 1.5f;

    public float c = 4;
    public float a = 1;

    public void ValidateValues() {
        scale = Mathf.Max(scale, 0.001f);
        normalizedMaxHeightModifier = Mathf.Max(normalizedMaxHeightModifier, 0.001f);
        octaves = Mathf.Max(octaves, 1);
        lacunarity = Mathf.Max(lacunarity, 1);
        Mathf.Clamp01(persistance);
    }
}