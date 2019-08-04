using UnityEditor;
using UnityEngine;

public class PlanetGenerator : MonoBehaviour {
    public NoiseSettings noiseSettings;
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

    public Gradient landGradient, waterGradient;
    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {

    }

    public void GeneratePlanet() {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        Material mat = meshRenderer.sharedMaterial;

        int width = size;
        int height = size;

        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;

        Color[] colors = new Color[width * height];
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
                    if (noiseValue > waterLevel) {
                        float normalisedValue = Mathf.InverseLerp(1 - waterLevel, 1, noiseValue);
                        if (reducedTones)
                            normalisedValue = Mathf.Round(normalisedValue * landTones) / landTones;
                        colors[x + y * width] = landGradient.Evaluate(normalisedValue);
                    } else {
                        float normalisedValue = Mathf.InverseLerp(0, waterLevel, noiseValue);
                        if (reducedTones)
                            normalisedValue = Mathf.Round((Mathf.Pow(normalisedValue, toneFalloff)) * waterTones) / waterTones;
                        colors[x + y * width] = waterGradient.Evaluate(normalisedValue);
                    }
                } else {
                    colors[x + y * width] = new Color(noiseValue, noiseValue, noiseValue);
                }
            }
        }

        tex.SetPixels(colors);
        tex.Apply();
        /*byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/../SavedScreen.png", bytes);*/
        mat.mainTexture = tex;
    }

    private void OnValidate() {
        GeneratePlanet();
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