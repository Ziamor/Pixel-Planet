using UnityEngine;

[ExecuteInEditMode]
public class Planet : MonoBehaviour {
    public float rotateSpeed = 1;
    public float radius = 1;

    Material mat;
    MaterialPropertyBlock propBlock;
    MeshRenderer meshRenderer;

    Vector2 rot;
    Vector2 lastPos;

    Texture texture = null;

    // Start is called before the first frame update
    void Start() {
        Init();
        lastPos = Input.mousePosition;
    }

    // Update is called once per frame
    void Update() {
        if (mat == null)
            Init();

        if (texture == null) return;

        meshRenderer.GetPropertyBlock(propBlock);
        Vector2 currentPosition = Input.mousePosition;
        if (Input.GetMouseButton(0)) {
            Vector2 dir = (currentPosition - lastPos).normalized;
            rot += dir * rotateSpeed;
            mat.SetVector("_Offset", rot);
        }
        lastPos = currentPosition;
        propBlock.SetTexture("_PlanetTexture", texture);
        propBlock.SetFloat("_Radius", radius);
        meshRenderer.SetPropertyBlock(propBlock);
    }

    void Init() {
        propBlock = new MaterialPropertyBlock();
        meshRenderer = GetComponent<MeshRenderer>();
        mat = meshRenderer.sharedMaterial;
    }

    public void SetTexture(Texture tex) {
        if (mat == null)
            Init();

        texture = tex;
    }
}
