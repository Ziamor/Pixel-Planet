using UnityEditor;
using UnityEngine;

[CreateAssetMenu()]
public class CelestialBodySettings : ScriptableObject {
    [HideInInspector]
    public bool noiseSettingsFoldout;

    public int textureSize = 128;

    public NoiseSettings noiseSettings;
    public BodySettings shapeSettings;
    public ColorSettings colorSettings;
    public CloudSettings cloudSettings;
}

[System.Serializable]
public class NoiseSettings {
    public int seed = 1243;
    public float dx = 1;
    public float dy = 1;
    public int octaves = 5;
    public float persistance = 0.5f;
    public float baseRoughness = 1;
    public float roughness = 1.5f;
}

[System.Serializable]
public class BodySettings {
    public float baseRadius = 0.5f;
    public float radiusChange = 0.001f;
    public bool allowRotate = true;
    public bool hasNightGlow = false;
}

[System.Serializable]
public class ColorSettings {
    public Gradient landGradient;
    public Gradient waterGradient;

    public bool useColor = true;
    public int landTones = 5;
    public int waterTones = 5;
    public bool reducedTones = true;
    public float toneFalloff = 2;

    public float waterLevel = 0.25f;

    public int layers = 4;
    public Color tint = Color.white;

    public float shadowStrength = 1f;   
}

[System.Serializable]
public class CloudSettings {
    public float cloudRadiusStart = 0.56f;
    public float cloudRadiusChange = 0.001f;
    public int cloudCentroids = 0;
    public int cloundCount = 0;
    public float cloudDensity = 0.05f;
    public Gradient cloudTint;
}