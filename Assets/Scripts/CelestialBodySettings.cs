using UnityEngine;

[CreateAssetMenu()]
public class CelestialBodySettings : ScriptableObject {
    public float baseRadius = 0.5f;
    public float radiusChange = 0.001f;
    public float waterLevel = 0.25f;
    public float dx = 10, dy = 10;
    public int octaves = 5;
    public float baseRoughness = 1;
    public float roughness = 1.5f;
    public float persistance = 0.5f;
    public bool useColor = true;
    public int bodyTextureSize = 128;
    public int landTones = 5;
    public int waterTones = 5;
    public bool reducedTones = true;
    public float toneFalloff = 2;
    public int layers = 4;
    public int seed = 1243;
    public Gradient landGradient, waterGradient;
    public int cloudTextureSize = 128;
    public float cloudRadiusStart = 0.56f;
    public float cloudRadiusChange = 0.001f;
    public int cloudCentroids = 0;
    public int cloundCount = 0;
    public float cloudDensity = 0.05f;
    public float shadowStrength = 1f;
    public bool allowRotate = true;
}
