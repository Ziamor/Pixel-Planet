using UnityEditor;
using UnityEngine;

public class PlanetGenerator : MonoBehaviour {
    public PlanetSettings planetSettings;
    public GameObject planetLayerPrefab;
    public GameObject cloudPrefab;

    [HideInInspector]
    public bool planetSettingsFoldout;

    GameObject[] clouds;

    void Start() {
        if (Application.isPlaying)
            GeneratePlanet();
    }

    // Update is called once per frame
    void Update() {

    }

    public void GeneratePlanet() {
        Clean();

        int width = planetSettings.size;
        int height = planetSettings.size;

        Color[][] colors = new Color[planetSettings.layers][];
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

                if (planetSettings.useColor) {
                    Color color;
                    int index = 0;
                    if (noiseValue > planetSettings.waterLevel) {
                        float normalisedValue = Mathf.InverseLerp(1 - planetSettings.waterLevel, 1, noiseValue);
                        if (planetSettings.reducedTones)
                            normalisedValue = Mathf.Round(normalisedValue * planetSettings.landTones) / planetSettings.landTones;
                        color = planetSettings.landGradient.Evaluate(normalisedValue);
                        index = (int)Mathf.Round(normalisedValue * (colors.GetLength(0) - 1));
                    } else {
                        float normalisedValue = Mathf.InverseLerp(0, planetSettings.waterLevel, noiseValue);
                        if (planetSettings.reducedTones)
                            normalisedValue = Mathf.Round((Mathf.Pow(normalisedValue, planetSettings.toneFalloff)) * planetSettings.waterTones) / planetSettings.waterTones;
                        color = planetSettings.waterGradient.Evaluate(normalisedValue);
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
            planetLayer.GetComponent<Planet>().SetTexture(planetLayerTexture);
            planetLayer.GetComponent<Planet>().radius = planetSettings.baseRadius + planetSettings.radiusChange * i;
            planetLayer.transform.localPosition = new Vector3(0, 0.01f * i, 0);
        }

        GenerateClouds();
    }

    /*private void OnValidate() {
        if (Application.isPlaying)
            GeneratePlanet();
        else {
            UnityEditor.EditorApplication.delayCall += () => {
                GeneratePlanet();
            };
        }
    }*/

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
        clouds = new GameObject[planetSettings.cloudCount];
        if (cloudPrefab != null) {
            for (int i = 0; i < clouds.Length; i++) {
                clouds[i] = Instantiate(cloudPrefab, transform);
                clouds[i].GetComponent<Cloud>().roateSpeedVariance = Random.Range(0.8f, 1.2f);
            }
        }
    }

    public float Evaluate(Vector3 point) {
        float noiseValue = 0;
        float frequency = planetSettings.baseRoughness;
        float amplitude = 1;

        for (int i = 0; i < planetSettings.octaves; i++) {
            float v = SimplexNoise.SeamlessNoise(point.x, point.y, frequency, frequency, planetSettings.seed);
            noiseValue += (v + 1) * 0.5f * amplitude;
            frequency *= planetSettings.roughness;
            amplitude *= planetSettings.persistance;
        }

        return noiseValue;
    }
}
[CustomEditor(typeof(PlanetGenerator))]
public class PlanetEditor : Editor {
    PlanetGenerator planetGenerator;

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

        DrawSettingsEditor(planetGenerator.planetSettings, planetGenerator.GeneratePlanet, ref planetGenerator.planetSettingsFoldout, ref planetSettingsEditor);
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
        planetGenerator = (PlanetGenerator)target;
    }
}