using UnityEngine;

[ExecuteInEditMode]
public class CelestialBodyLayer : MonoBehaviour {
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

    Texture surfaceTexture = null;
    Texture nightGlowTexture = null;

    Transform sun;

    float nightGlow;

    Zoom zoom;
    // Start is called before the first frame update
    void Start() {
        Init();
        lastPos = Input.mousePosition;
    }

    // Update is called once per frame
    void Update() {
        if (mat == null || propBlock == null)
            Init();

        if (surfaceTexture == null || nightGlowTexture == null) return;

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

        propBlock.SetTexture("_SurfaceTexture", surfaceTexture);
        propBlock.SetTexture("_WaterMask", nightGlowTexture);
        propBlock.SetColor("_Tint", tint);
        propBlock.SetFloat("_Radius", radius * zoom.ZoomLevel);
        propBlock.SetFloat("_ShadowStrength", shadowStrength);
        propBlock.SetColor("_ShadowColor", shadowColor);
        propBlock.SetFloat("_NightGlow", nightGlow);
        meshRenderer.SetPropertyBlock(propBlock);
    }

    void Init() {
        zoom = FindObjectOfType<Zoom>();
        propBlock = new MaterialPropertyBlock();
        meshRenderer = GetComponent<MeshRenderer>();
        mat = meshRenderer.sharedMaterial;
        GameObject sunGameObject = GameObject.Find("Sun");
        if (sunGameObject != null)
            sun = sunGameObject.transform;
    }

    public void Configure(int layerIndex, Texture surfaceTexture, Texture nightGlowTexture, BodySettings shapeSettings, ColorSettings colorSettings) {
        this.surfaceTexture = surfaceTexture;
        this.nightGlowTexture = nightGlowTexture;
        this.radius = shapeSettings.baseRadius + shapeSettings.radiusChange * layerIndex;
        this.tint = colorSettings.tint;
        this.shadowStrength = colorSettings.shadowStrength;
        this.allowRotate = shapeSettings.allowRotate;
        this.scroll = true;
        this.rotateSpeed = 0.001f;
        this.nightGlow = shapeSettings.hasNightGlow ? 1 : 0;
    }
}
