using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class CelestialBodyGenerator : MonoBehaviour {
    public CelestialBodySettings celestialBodySettings;
    public GameObject planetLayerPrefab;
    public GameObject cloudPrefab;
    public Texture2D[] cloudTextures;

    [HideInInspector]
    public bool planetSettingsFoldout;

    void Start() {
        if (Application.isPlaying)
            GeneratePlanet();
    }

    // Update is called once per frame
    void Update() {

    }

    public void GeneratePlanet() {
        Clean();
        GenerateSurface();
        GenerateClouds();
    }

    private void GenerateSurface() {
        int width = celestialBodySettings.textureSize;
        int height = celestialBodySettings.textureSize;

        NoiseMapData noiseMapData = GenerateNoiseMap(width, height, celestialBodySettings.noiseSettings);

        CelestialBodyColorMap celestialBodyData = GenerateCelestialBodyColorMap(width, height, celestialBodySettings.colorSettings, noiseMapData);

        Texture2D nightGlowMask = new Texture2D(width, height, TextureFormat.RGBA32, false);
        nightGlowMask.filterMode = FilterMode.Point;
        nightGlowMask.SetPixels(celestialBodyData.nightGlowColorMap);
        nightGlowMask.Apply();

        for (int i = 0; i < celestialBodyData.layerColorMaps.GetLength(0); i++) {
            AddCelestialBodyLayer(width, height, i, celestialBodyData.layerColorMaps[i], nightGlowMask, celestialBodySettings);
        }
    }

    private void AddCelestialBodyLayer(int width, int height, int layerIndex, Color[] colorMap, Texture2D nightGlowMask, CelestialBodySettings celestialBodySettings) {
        GameObject planetLayer = Instantiate(planetLayerPrefab, transform);
        planetLayer.name = "Celestial Body Layer " + layerIndex;

        Texture2D planetLayerTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        planetLayerTexture.filterMode = FilterMode.Point;
        planetLayerTexture.SetPixels(colorMap);
        planetLayerTexture.Apply();

        CelestialBodyLayer celestialBodyLayer = planetLayer.GetComponent<CelestialBodyLayer>();
        celestialBodyLayer.Configure(layerIndex, planetLayerTexture, nightGlowMask, celestialBodySettings.shapeSettings, celestialBodySettings.colorSettings);


        planetLayer.transform.localPosition = new Vector3(0, 0.01f * layerIndex, 0);
    }

    private CelestialBodyColorMap GenerateCelestialBodyColorMap(int width, int height, ColorSettings colorSettings, NoiseMapData noiseMapData) {

        Color[][] layerColorMaps = new Color[colorSettings.layers][];
        Color[] nightGlowColorMap = new Color[width * height];

        for (int i = 0; i < layerColorMaps.Length; i++) {
            layerColorMaps[i] = new Color[width * height];
        }

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                float noiseValue = Mathf.InverseLerp(noiseMapData.minValue, noiseMapData.maxValue, noiseMapData.noiseValues[x, y]);

                if (colorSettings.useColor) {
                    Color color;
                    int index = 0;
                    if (noiseValue >= colorSettings.waterLevel) {
                        float normalisedValue = Mathf.InverseLerp(colorSettings.waterLevel == 0 ? 0 : 1 - colorSettings.waterLevel, 1, noiseValue);
                        if (colorSettings.reducedTones)
                            normalisedValue = Mathf.Round(normalisedValue * colorSettings.landTones) / colorSettings.landTones;
                        color = colorSettings.landGradient.Evaluate(normalisedValue);
                        index = (int)Mathf.Round(normalisedValue * (layerColorMaps.GetLength(0) - 1));
                    } else {
                        float normalisedValue = Mathf.InverseLerp(0, colorSettings.waterLevel, noiseValue);
                        if (colorSettings.reducedTones)
                            normalisedValue = Mathf.Round((Mathf.Pow(normalisedValue, colorSettings.toneFalloff)) * colorSettings.waterTones) / colorSettings.waterTones;
                        color = colorSettings.waterGradient.Evaluate(normalisedValue);
                        nightGlowColorMap[x + y * width] = Color.white;
                    }

                    for (int i = 0; i < 1 + index; i++) {
                        layerColorMaps[i][x + y * width] = color;
                    }
                } else {
                    //colors[x + y * width] = new Color(noiseValue, noiseValue, noiseValue);
                }
            }
        }

        CelestialBodyColorMap celestialBodyData = new CelestialBodyColorMap();
        celestialBodyData.layerColorMaps = layerColorMaps;
        celestialBodyData.nightGlowColorMap = nightGlowColorMap;

        return celestialBodyData;
    }

    private NoiseMapData GenerateNoiseMap(int width, int height, NoiseSettings noiseSettings) {
        float[,] noiseValues = new float[width, height];

        float maxValue = float.MinValue;
        float minValue = float.MaxValue;

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                Vector3 p = new Vector3();
                p.x = i / (float)width;
                p.y = j / (float)height;
                float noiseValue = Evaluate(p, noiseSettings) / 2 + 0.5f;
                noiseValues[i, j] = noiseValue;

                maxValue = Mathf.Max(maxValue, noiseValue);
                minValue = Mathf.Min(minValue, noiseValue);
            }
        }

        NoiseMapData noiseMapData = new NoiseMapData();
        noiseMapData.noiseValues = noiseValues;
        noiseMapData.minValue = minValue;
        noiseMapData.maxValue = maxValue;

        return noiseMapData;
    }

    private void Clean() {
        int childs = transform.childCount;
        for (int i = childs - 1; i >= 0; i--) {
            if (Application.isPlaying)
                Destroy(transform.GetChild(i).gameObject);
            else {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }
    }
    public void GenerateClouds() {
        /*int cloudPadding = 3;
        if (cloudPrefab != null && old_celestialBodySettings.cloudCentroids > 0 && old_celestialBodySettings.cloundCount > 0) {

            Vector2[] cloudCentroids = new Vector2[old_celestialBodySettings.cloudCentroids];
            Vector2[] clouds = new Vector2[old_celestialBodySettings.cloundCount];
            for (int i = 0; i < cloudCentroids.Length; i++) {
                cloudCentroids[i] = new Vector2(Random.value, Random.value);
            }

            for (int i = 0; i < old_celestialBodySettings.cloundCount; i++) {
                int index = Random.Range(0, cloudCentroids.Length - 1);
                clouds[i] = cloudCentroids[index] + (Random.insideUnitCircle + Vector2.one) / 2 * old_celestialBodySettings.cloudDensity;
            }

            int width = old_celestialBodySettings.cloudTextureSize;
            int height = old_celestialBodySettings.cloudTextureSize;

            Texture2D waterMaskTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            waterMaskTexture.filterMode = FilterMode.Point;

            waterMaskTexture.SetPixels(new Color[width * height]);
            waterMaskTexture.Apply();

            for (int k = 0; k < cloudTextures.Length * cloudPadding; k++) {
                Texture2D cloudLayerTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
                cloudLayerTexture.filterMode = FilterMode.Point;

                Color[] cloudColors = cloudTextures[k / cloudPadding].GetPixels();
                Color[] colors = new Color[width * height];
                for (int i = 0; i < clouds.Length; i++) {
                    for (int j = 0; j < cloudColors.Length; j++) {
                        if (cloudColors[j].a == 0) continue;
                        int localX = j % cloudTextures[k / cloudPadding].width;
                        int localY = j / cloudTextures[k / cloudPadding].width;

                        int x = ((int)((clouds[i].x * width) % width) + localX) % width;
                        int y = ((int)((clouds[i].y * height) % height) + localY) % height;

                        int index = x * width + y;
                        colors[index] = cloudColors[j] * old_celestialBodySettings.cloudTint.Evaluate(k / (float)(cloudTextures.Length * cloudPadding));
                    }
                }
                cloudLayerTexture.SetPixels(colors);
                cloudLayerTexture.Apply();

                CelestialBodyLayer cloud = Instantiate(planetLayerPrefab, transform).GetComponent<CelestialBodyLayer>();
                cloud.gameObject.name = "Cloud Layer " + k;

                cloud.Configure(k, cloudLayerTexture, waterMaskTexture, old_celestialBodySettings);

                cloud.radius = old_celestialBodySettings.cloudRadiusStart + old_celestialBodySettings.cloudRadiusChange * k;
                cloud.allowRotate = true;
                cloud.transform.localPosition = new Vector3(0, 0.01f * k + 0.01f * old_celestialBodySettings.layers, 0);
                cloud.scroll = true;
                cloud.rotateSpeed = 0.002f;
                cloud.tint = old_celestialBodySettings.cloudTint.Evaluate(k / (cloudTextures.Length * cloudPadding));
                cloud.shadowColor = new Color(0.05288889f, 0.04848149f, 0.119f);
                //cloud.roateSpeedVariance = Random.Range(0.8f, 1.2f);
            }
        }*/
    }

    public float Evaluate(Vector3 point, NoiseSettings noiseSettings) {
        float noiseValue = 0;
        float frequency = noiseSettings.baseRoughness;
        float amplitude = 1;

        for (int i = 0; i < noiseSettings.octaves; i++) {
            float v = SimplexNoise.SeamlessNoise(point.x, point.y, frequency * noiseSettings.dx, frequency * noiseSettings.dy, noiseSettings.seed);
            noiseValue += (v + 1) * 0.5f * amplitude;
            frequency *= noiseSettings.roughness;
            amplitude *= noiseSettings.persistance;
        }

        return noiseValue;
    }
}
[CustomEditor(typeof(CelestialBodyGenerator))]
public class PlanetEditor : Editor {
    CelestialBodyGenerator planetGenerator;

    Editor planetSettingsEditor;

    public override void OnInspectorGUI() {
        using (var check = new EditorGUI.ChangeCheckScope()) {
            base.OnInspectorGUI();
            if (check.changed) {
                planetGenerator.GeneratePlanet();
            }
        }

        if (GUILayout.Button("Generate")) {
            planetGenerator.GeneratePlanet();
        }

        DrawSettingsEditor(planetGenerator.celestialBodySettings, planetGenerator.GeneratePlanet, ref planetGenerator.planetSettingsFoldout, ref planetSettingsEditor);
    }

    void DrawSettingsEditor(Object settings, System.Action onSettingsUpdated, ref bool foldout, ref Editor editor) {
        if (settings != null) {
            foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
            using (var check = new EditorGUI.ChangeCheckScope()) {
                if (foldout) {
                    CreateCachedEditor(settings, null, ref editor);
                    editor.OnInspectorGUI();

                    if (check.changed) {
                        if (onSettingsUpdated != null) {
                            onSettingsUpdated();
                        }
                    }
                }
            }
        }
    }

    private void OnEnable() {
        planetGenerator = (CelestialBodyGenerator)target;
    }
}

public struct NoiseMapData {
    public float minValue;
    public float maxValue;

    public float[,] noiseValues;
}

public struct CelestialBodyColorMap {
    public Color[][] layerColorMaps;
    public Color[] nightGlowColorMap;
}