using System.IO;
using UnityEditor;
using UnityEngine;

public class PlanetGenerator : MonoBehaviour {
    public NoiseSettings noiseSettings;
    public float waterLevel = 0.25f;
    public Color water, land;
    Noise noise;
    public float dx = 10, dy = 10, scale = 10;
    public int octaves = 5;
    public float baseRoughness = 1;
    public float roughness = 1.5f;
    public float persistance = 0.5f;
    public bool useColor = true;

    // Start is called before the first frame update
    void Start() {
        noise = new Noise();
    }

    // Update is called once per frame
    void Update() {

    }

    public void GeneratePlanet() {
        /*Noise noise = new Noise();
        int size = 512;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGB24, false);
        Color[] colors = new Color[size * size];
        for (int x = 0; x < size; x++) {
            for (int y = 0; y < size; y++) {
                Vector2 p = new Vector2((x / (float)size) - 0.5f, (y / (float)size) - 0.5f) * 2f;
                float a = Mathf.Atan2(p.y, p.x) / Mathf.PI;
                float r = p.magnitude;
                if (r > 1) {
                    colors[x + y * size] = new Color(0, 0, 0);
                    continue;
                }
                Vector3 uv;
                uv.x = r / 2 + 0.5f;
                uv.y = a / 2 + 0.5f;
                uv.z = 1;

                float r = 2 * (x / (float)size) - 1;
                float a = 2 * (y / (float)size) - 1;
                Vector3 uv;
                uv.x = r * Mathf.Cos(a);
                uv.y = r * Mathf.Sin(a);
                uv.z = 1;
                float noiseValue = 0;
                float frequency = baseRoughness;
                float amplitude = 1;

                for (int i = 0; i < octaves; i++) {
                    float v = noise.Evaluate(uv * frequency);
                    noiseValue += (v + 1) * 0.5f * amplitude;
                    frequency *= roughness;
                    amplitude *= persistence;
                }

                //noiseValue = Mathf.Max(0, noiseValue - settings.minValue);
                colors[x + y * size] = new Color(noiseValue, noiseValue, noiseValue);
            }
        }

        tex.SetPixels(colors);
        tex.Apply();
        // Encode texture into PNG
        byte[] bytes = tex.EncodeToPNG();

        // For testing purposes, also write to a file in the project folder
        File.WriteAllBytes(Application.dataPath + "/../Assets/SavedScreen.png", bytes);*/
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        Material mat = meshRenderer.sharedMaterial;

        float c = noiseSettings.c, a = noiseSettings.a; // torus parameters (controlling size)
        //float ratio = c / a;
        int size = 512;
        //int width = (int)(size * ratio); // Tried ratio, but width should be twice as big as height, 1 half for each hemisphere
        int width = size;
        int height = size;

        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;

        Color[] colors = new Color[width * height];
        float[,] noiseValues = new float[width, height];//NoiseGenerator.GenerateNoiseMap(size * 2, size, noiseSettings, Vector2.zero);

        float maxValue = float.MinValue;
        float minValue = float.MaxValue;

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                Vector3 p = new Vector3();
                /*float x = i / ((float)width);
                float y = j / ((float)height);                
                p.x = (c + a * Mathf.Cos(2 * Mathf.PI * y)) * Mathf.Cos(2 * Mathf.PI * x);
                p.y = (c + a * Mathf.Cos(2 * Mathf.PI * y)) * Mathf.Sin(2 * Mathf.PI * x);
                p.z = a * Mathf.Sin(2 * Mathf.PI * y);*/
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
                    if (noiseValue > waterLevel)
                        colors[x + y * width] = land;
                    else
                        colors[x + y * width] = water;
                } else {
                    colors[x + y * width] = new Color(noiseValue, noiseValue, noiseValue);
                }
            }
        }

        tex.SetPixels(colors);
        tex.Apply();
        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/../SavedScreen.png", bytes);
        mat.mainTexture = tex;
    }

    private void OnValidate() {
        GeneratePlanet();
    }

    public float Evaluate(Vector3 point) {
        if (noise == null)
            noise = new Noise();
        float noiseValue = 0;
        float frequency = baseRoughness;
        float amplitude = 1;

        for (int i = 0; i < octaves; i++) {
            float v = SimplexNoise.SeamlessNoise(point.x, point.y, frequency, frequency, noiseSettings.seed); //noise.Evaluate(point * frequency);
            noiseValue += (v + 1) * 0.5f * amplitude;
            frequency *= roughness;
            amplitude *= persistance;
        }

        return noiseValue;
    }
    /*public float Evaluate(Vector3 point) {
        if (noise == null)
            noise = new Noise();
        float noiseValue = 0;
        float frequency = noiseSettings.baseRoughness;
        float amplitude = 1;

        for (int i = 0; i < noiseSettings.octaves; i++) {
            float v = SimplexNoise.SeamlessNoise(point.x, point.y, 10, 10, noiseSettings.octaves * noiseSettings.seed) * frequency; //noise.Evaluate(point * frequency);
            noiseValue += (v + 1) * 0.5f * amplitude;
            frequency *= noiseSettings.roughness;
            amplitude *= noiseSettings.persistance;
        }

        //noiseValue = Mathf.Max(0, noiseValue - settings.minValue);
        return noiseValue;
    }
    public float Evaluate(Vector3 point) {
        if (noise == null)
            noise = new Noise();
        float noiseValue = 0;
        for (int i = 0; i < octaves; i++) {
            noiseValue += SimplexNoise.SeamlessNoise(point.x, point.y, scale * i, scale * i, noiseSettings.seed);
        }
        
        return noiseValue;
    }*/
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

/*using System.IO;
using UnityEditor;
using UnityEngine;

public class PlanetGenerator : MonoBehaviour {
    public NoiseSettings noiseSettings;
    public float waterLevel = 0.25f;
    public Color water, land;
    Noise noise;
    // Start is called before the first frame update
    void Start() {
        noise = new Noise();
    }

    // Update is called once per frame
    void Update() {

    }

    public void GeneratePlanet() {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        Material mat = meshRenderer.sharedMaterial;

        float c = noiseSettings.c, a = noiseSettings.a; // torus parameters (controlling size)
        //float ratio = c / a;
        int size = 512;
        //int width = (int)(size * ratio); // Tried ratio, but width should be twice as big as height, 1 half for each hemisphere
        int width = size;
        int height = size;

        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;

        Color[] colors = new Color[width * height];
        float[,] noiseValues = new float[width, height];//NoiseGenerator.GenerateNoiseMap(size * 2, size, noiseSettings, Vector2.zero);

        float maxValue = float.MinValue;
        float minValue = float.MaxValue;

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                Vector3 p = new Vector3();
                float x = i / ((float)width);
                float y = j / ((float)height);             

                float longitude = 2 * ((x) - 0.5f) * 180 * Mathf.Deg2Rad;
                float latitude = 2 * ((y) - 0.5f) * 180 * Mathf.Deg2Rad;
                p.x = Mathf.Cos(latitude) * Mathf.Cos(longitude);
                p.y = Mathf.Cos(latitude) * Mathf.Sin(longitude);
                p.z = Mathf.Sin(latitude);
                p.Normalize();
                float noiseValue = Evaluate(p);
                maxValue = Mathf.Max(maxValue, noiseValue);
                minValue = Mathf.Min(minValue, noiseValue);
                noiseValues[i, j] = noiseValue;
            }
        }
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                float noiseValue = Mathf.InverseLerp(minValue, maxValue, noiseValues[x, y]);
                if (noiseValue > waterLevel)
                    colors[x + y * width] = land;
                else
                    colors[x + y * width] = water;

                colors[x + y * width] = new Color(noiseValue, noiseValue, noiseValue);
            }
        }

        tex.SetPixels(colors);
        tex.Apply();
        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/../SavedScreen.png", bytes);
        mat.mainTexture = tex;
    }

    private void OnValidate() {
        GeneratePlanet();
    }

    public float Evaluate(Vector3 point) {
        if (noise == null)
            noise = new Noise();
        float noiseValue = 0;
        float frequency = noiseSettings.baseRoughness;
        float amplitude = 1;

        for (int i = 0; i < noiseSettings.octaves; i++) {
            float v = noise.Evaluate(point * frequency);
            noiseValue += (v + 1) * 0.5f * amplitude;
            frequency *= noiseSettings.roughness;
            amplitude *= noiseSettings.persistance;
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
}*/
