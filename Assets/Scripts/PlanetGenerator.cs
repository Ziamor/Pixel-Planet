using UnityEditor;
using UnityEngine;

public class PlanetGenerator : MonoBehaviour {
    public NoiseSettings noiseSettings;
    public float baseRadius = 0.5f;
    public float radiusChange = 0.001f;
    public float waterLevel = 0.25f;
    public float dx = 10, dy = 10, scale = 10;
    public int octaves = 5;
    public float baseRoughness = 1;
    public float roughness = 1.5f;
    public float persistance = 0.5f;
    public bool useColor = true;
    public int size = 512;
    public int landTones = 5;
    public int waterTones = 5;
    public bool reducedTones = true;
    public float toneFalloff = 2;
    public int layers = 4;

    public Gradient landGradient, waterGradient;

    public GameObject planetLayerPrefab;

    public GameObject cloudPrefab;
    public int cloudCount = 10;

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

        int width = size;
        int height = size;

        Color[][] colors = new Color[layers][];
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

                if (useColor) {
                    Color color;
                    int index = 0;
                    if (noiseValue > waterLevel) {
                        float normalisedValue = Mathf.InverseLerp(1 - waterLevel, 1, noiseValue);
                        if (reducedTones)
                            normalisedValue = Mathf.Round(normalisedValue * landTones) / landTones;
                        color = landGradient.Evaluate(normalisedValue);
                        index = (int)Mathf.Round(normalisedValue * (colors.GetLength(0) - 1));
                    } else {
                        float normalisedValue = Mathf.InverseLerp(0, waterLevel, noiseValue);
                        if (reducedTones)
                            normalisedValue = Mathf.Round((Mathf.Pow(normalisedValue, toneFalloff)) * waterTones) / waterTones;
                        color = waterGradient.Evaluate(normalisedValue);
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
            Texture2D planetLayerTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            planetLayerTexture.filterMode = FilterMode.Point;

            planetLayerTexture.SetPixels(colors[i]);
            planetLayerTexture.Apply();
            planetLayer.GetComponent<Planet>().SetTexture(planetLayerTexture);
            planetLayer.GetComponent<Planet>().radius = baseRadius + radiusChange * i;
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
        for (int i = childs - 1; i > 0; i--) {
            if (Application.isPlaying)
                Destroy(transform.GetChild(i).gameObject);
            else {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }
    }
    public void GenerateClouds() {
        clouds = new GameObject[cloudCount];
        if (cloudPrefab != null) {
            for (int i = 0; i < clouds.Length; i++) {
                clouds[i] = Instantiate(cloudPrefab, transform);
                clouds[i].GetComponent<Cloud>().roateSpeedVariance = Random.Range(0.8f, 1.2f);
            }
        }
    }

    public float Evaluate(Vector3 point) {
        float noiseValue = 0;
        float frequency = baseRoughness;
        float amplitude = 1;

        for (int i = 0; i < octaves; i++) {
            float v = SimplexNoise.SeamlessNoise(point.x, point.y, frequency, frequency, noiseSettings.seed);
            noiseValue += (v + 1) * 0.5f * amplitude;
            frequency *= roughness;
            amplitude *= persistance;
        }

        return noiseValue;
    }
}
[CustomEditor(typeof(PlanetGenerator))]
public class PlanetEditor : Editor {
    PlanetGenerator planetGenerator;

    public override void OnInspectorGUI() {
        planetGenerator = (PlanetGenerator)target;

        base.OnInspectorGUI();
        if (GUILayout.Button("Generate")) {
            planetGenerator.GeneratePlanet();
        }
    }
}