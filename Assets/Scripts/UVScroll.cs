using UnityEngine;

public class UVScroll : MonoBehaviour {
    public float rotateSpeed = 1;
    public float radius = 1;
    public bool invert = false;

    Material mat;
    MaterialPropertyBlock propBlock;
    MeshRenderer meshRenderer;

    Vector2 rot;
    Vector2 lastPos;

    // Start is called before the first frame update
    void Start() {
        Init();
    }

    // Update is called once per frame
    void Update() {
        if (mat == null || propBlock == null)
            Init();

        meshRenderer.GetPropertyBlock(propBlock);
        Vector2 currentPosition = Input.mousePosition;

        if (Input.GetMouseButton(0)) {
            Vector2 dir = (currentPosition - lastPos).normalized;
            rot += dir * rotateSpeed * (invert ? -1f : 1f);
            propBlock.SetVector("_MainTex_ST", new Vector4(1, 1, rot.x, rot.y));
        }
        lastPos = currentPosition;

        meshRenderer.SetPropertyBlock(propBlock);
    }

    void Init() {
        lastPos = Input.mousePosition;
        propBlock = new MaterialPropertyBlock();
        meshRenderer = GetComponent<MeshRenderer>();
        mat = meshRenderer.sharedMaterial;
    }
}
