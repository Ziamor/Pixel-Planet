using UnityEngine;

[ExecuteInEditMode]
public class CelestialBody : MonoBehaviour {
    public float rotateSpeed = 1;
    public float radius = 1;
    public float shadowStrength = 1f;
    public bool allowRotate = true;
    public bool scroll = false;

    public Color tint = Color.white;
    public Color shadowColor = new Color(0.06652723f, 0.06652723f, 0.1226415f);

    Material mat;
    MaterialPropertyBlock propBlock;
    MeshRenderer meshRenderer;

    Vector2 rot;
    Vector2 lastPos;

    Texture texture = null;
    Texture waterMask = null;

    Transform sun;

    float nightGlow;
    // Start is called before the first frame update
    void Start() {
        Init();
        lastPos = Input.mousePosition;
    }

    // Update is called once per frame
    void Update() {
        if (mat == null || propBlock == null)
            Init();

        if (texture == null || waterMask == null) return;

        meshRenderer.GetPropertyBlock(propBlock);
        Vector2 currentPosition = Input.mousePosition;

        if (allowRotate && Input.GetMouseButton(0)) {
            Vector2 dir = (currentPosition - lastPos).normalized;
            rot += dir * rotateSpeed;
            propBlock.SetVector("_Offset", rot);
        }

        if (scroll) {
            rot += Vector2.right * rotateSpeed;
            propBlock.SetVector("_Offset", rot);
        }
        lastPos = currentPosition;

        if (sun != null) {
            Vector3 lightDir = (sun.position - transform.position).normalized;
            propBlock.SetVector("_LightDir", lightDir);
        }

        propBlock.SetTexture("_PlanetTexture", texture);
        propBlock.SetTexture("_WaterMask", waterMask);
        propBlock.SetColor("_Tint", tint);
        propBlock.SetFloat("_Radius", radius);
        propBlock.SetFloat("_ShadowStrength", shadowStrength);
        propBlock.SetColor("_ShadowColor", shadowColor);
        propBlock.SetFloat("_NightGlow", nightGlow);
        meshRenderer.SetPropertyBlock(propBlock);
    }

    void Init() {
        propBlock = new MaterialPropertyBlock();
        meshRenderer = GetComponent<MeshRenderer>();
        mat = meshRenderer.sharedMaterial;
        GameObject sunGameObject = GameObject.Find("Sun");
        if (sunGameObject != null)
            sun = sunGameObject.transform;
    }

    public void SetTexture(Texture tex) {
        if (mat == null)
            Init();

        texture = tex;
    }

    public void SetWaterMask(Texture tex) {
        if (mat == null)
            Init();

        waterMask = tex;
    }

    public void EnableNightGlow() {
        if (mat == null)
            Init();
        nightGlow = 1;
        //mat.EnableKeyword("NIGHT_GLOW_ON");
    }
}
