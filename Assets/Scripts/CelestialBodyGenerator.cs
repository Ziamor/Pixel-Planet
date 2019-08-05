using UnityEditor;
using UnityEngine;

public class CelestialBodyGenerator : MonoBehaviour {
    public CelestialBodySettings celestialBodySettings;
    public GameObject planetLayerPrefab;
    public GameObject cloudPrefab;

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

        int width = celestialBodySettings.size;
        int height = celestialBodySettings.size;

        Color[][] colors = new Color[celestialBodySettings.layers][];
        for (int i = 0; i < colors.Length; i++) {
            colors[i] = new Color[width * height];
        }
        float[,] noiseValues = new float[width, height];

        float maxValue = float.MinValue;
        float minValue = float.MaxValue;

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                Vector3 p = new Vector3();
                p.x = i / (float)width;
                p.y = j / (float)height;
                float noiseValue = Evaluate(p) / 2 + 0.5f;
                noiseValues[i, j] = noiseValue;

                maxValue = Mathf.Max(maxValue, noiseValue);
                minValue = Mathf.Min(minValue, noiseValue);
            }
        }
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                float noiseValue = Mathf.InverseLerp(minValue, maxValue, noiseValues[x, y]);

                if (celestialBodySettings.useColor) {
                    Color color;
                    int index = 0;
                    if (noiseValue >= celestialBodySettings.waterLevel) {
                        float normalisedValue = Mathf.InverseLerp(celestialBodySettings.waterLevel == 0 ? 0 : 1 - celestialBodySettings.waterLevel, 1, noiseValue);
                        if (celestialBodySettings.reducedTones)
                            normalisedValue = Mathf.Round(normalisedValue * celestialBodySettings.landTones) / celestialBodySettings.landTones;
                        color = celestialBodySettings.landGradient.Evaluate(normalisedValue);
                        index = (int)Mathf.Round(normalisedValue * (colors.GetLength(0) - 1));
                    } else {
                        float normalisedValue = Mathf.InverseLerp(0, celestialBodySettings.waterLevel, noiseValue);
                        if (celestialBodySettings.reducedTones)
                            normalisedValue = Mathf.Round((Mathf.Pow(normalisedValue, celestialBodySettings.toneFalloff)) * celestialBodySettings.waterTones) / celestialBodySettings.waterTones;
                        color = celestialBodySettings.waterGradient.Evaluate(normalisedValue);
                    }

                    for (int i = 0; i < 1 + index; i++) {
                        colors[i][x + y * width] = color;
                    }
                } else {
                    //colors[x + y * width] = new Color(noiseValue, noiseValue, noiseValue);
                }
            }
        }

        for (int i = 0; i < colors.GetLength(0); i++) {
            GameObject planetLayer = Instantiate(planetLayerPrefab, transform);
            planetLayer.name = "Plant Layer " + 1;
            Texture2D planetLayerTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            planetLayerTexture.filterMode = FilterMode.Point;

            planetLayerTexture.SetPixels(colors[i]);
            planetLayerTexture.Apply();

            CelestialBody celestialBodyLayer = planetLayer.GetComponent<CelestialBody>();
            celestialBodyLayer.SetTexture(planetLayerTexture);
            celestialBodyLayer.radius = celestialBodySettings.baseRadius + celestialBodySettings.radiusChange * i;
            celestialBodyLayer.shadowStrength = celestialBodySettings.shadowStrength;
            celestialBodyLayer.allowRotate = celestialBodySettings.allowRotate;

            planetLayer.transform.localPosition = new Vector3(0, 0.01f * i, 0);
        }

        GenerateClouds();
    }

    public void Clean() {
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
        if (cloudPrefab != null && celestialBodySettings.cloudCount > 0) {
            Vector2[] clouds = new Vector2[celestialBodySettings.cloudCount];
            for (int i = 0; i < clouds.Length; i++) {
                clouds[i] = new Vector2(Random.value, Random.value);
            }

            for (int i = 0; i < celestialBodySettings.cloudChunks; i++) {
                int index = Random.Range(0, clouds.Length - 1);
                Cloud cloud = Instantiate(cloudPrefab, transform).GetComponent<Cloud>();
                //cloud.roateSpeedVariance = Random.Range(0.8f, 1.2f);
                cloud.Init(clouds[index], celestialBodySettings.cloudDensity);
            }


        }
    }

    public float Evaluate(Vector3 point) {
        float noiseValue = 0;
        float frequency = celestialBodySettings.baseRoughness;
        float amplitude = 1;

        for (int i = 0; i < celestialBodySettings.octaves; i++) {
            float v = SimplexNoise.SeamlessNoise(point.x, point.y, frequency, frequency, celestialBodySettings.seed);
            noiseValue += (v + 1) * 0.5f * amplitude;
            frequency *= celestialBodySettings.roughness;
            amplitude *= celestialBodySettings.persistance;
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