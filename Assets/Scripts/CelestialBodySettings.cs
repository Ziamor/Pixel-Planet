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
    public int size = 512;
    public int landTones = 5;
    public int waterTones = 5;
    public bool reducedTones = true;
    public float toneFalloff = 2;
    public int layers = 4;
    public int seed = 1243;
    public Gradient landGradient, waterGradient;
    public int cloudCount = 10;
}
