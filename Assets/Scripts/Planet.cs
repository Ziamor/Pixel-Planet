using UnityEngine;

public class Planet : MonoBehaviour {
    public float rotateSpeed = 1;
    public GameObject cloudPrefab;
    public int cloudCount = 10;

    Material mat;

    Vector2 rot;

    Vector2 lastPos;

    GameObject[] clouds;

    // Start is called before the first frame update
    void Start() {
        Init();
        lastPos = Input.mousePosition;
    }

    // Update is called once per frame
    void Update() {
        if (mat == null)
            Init();

        Vector2 currentPosition = Input.mousePosition;
        if (Input.GetMouseButton(0)) {
            Vector2 dir = (currentPosition - lastPos).normalized;
            rot += dir * rotateSpeed;
            mat.SetVector("_Offset", rot);
        }
        lastPos = currentPosition;
    }

    void Init() {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        mat = meshRenderer.sharedMaterial;

        if (clouds != null) {
            for (int i = 0; i < clouds.Length; i++) {
                if (clouds[i] == null) continue;
                Destroy(clouds[i]);
            }
        }
        
        clouds = new GameObject[cloudCount];
        if (cloudPrefab != null) {
            for (int i = 0; i < clouds.Length; i++) {
                clouds[i] = Instantiate(cloudPrefab);
                clouds[i].GetComponent<Cloud>().roateSpeedVariance = Random.Range(0.8f, 1.2f);
            }
        }
    }
}
