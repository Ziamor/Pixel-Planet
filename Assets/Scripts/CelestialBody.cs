using UnityEngine;

[ExecuteInEditMode]
public class CelestialBody : MonoBehaviour {
    public float rotateSpeed = 1;
    public float radius = 1;
    public float shadowStrength = 1f;
    public bool allowRotate = true;
    public bool scroll = false;

    public Color shadowColor = new Color(0.06652723f, 0.06652723f, 0.1226415f);
    Material mat;
    MaterialPropertyBlock propBlock;
    MeshRenderer meshRenderer;

    Vector2 rot;
    Vector2 lastPos;

    Texture texture = null;

    Transform sun;

    // Start is called before the first frame update
    void Start() {
        Init();
        lastPos = Input.mousePosition;
    }

    // Update is called once per frame
    void Update() {
        if (mat == null || propBlock == null || sun == null)
            Init();

        if (texture == null) return;

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

        Vector3 lightDir = (sun.position - transform.position).normalized;

        propBlock.SetTexture("_PlanetTexture", texture);
        propBlock.SetFloat("_Radius", radius);
        propBlock.SetFloat("_ShadowStrength", shadowStrength);
        propBlock.SetColor("_ShadowColor", shadowColor);
        propBlock.SetVector("_LightDir", lightDir);
        meshRenderer.SetPropertyBlock(propBlock);
    }

    void Init() {
        propBlock = new MaterialPropertyBlock();
        meshRenderer = GetComponent<MeshRenderer>();
        mat = meshRenderer.sharedMaterial;
        sun = GameObject.Find("Sun").transform;
    }

    public void SetTexture(Texture tex) {
        if (mat == null)
            Init();

        texture = tex;
    }
}
