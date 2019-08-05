using UnityEngine;

public class FaceCamera : MonoBehaviour {
    public bool invertX = false;
    public bool invertY = false;
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void FixedUpdate() {
        Vector3 targetDir = Camera.main.transform.eulerAngles;
        targetDir.x *= (invertX ? -1f : 1f);
        targetDir.y *= (invertY ? -1f : 1f);
        transform.rotation = Quaternion.Euler(targetDir.x + 90, targetDir.y + 180, 0);
    }
}
