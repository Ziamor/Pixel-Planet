using UnityEngine;

public class OrbitCamera : MonoBehaviour {
    public Transform target;

    public float rotateSpeed = 1;
    public float radius = 1;
    public bool invert = false;

    Vector2 rot;
    Vector2 lastPos;

    // Start is called before the first frame update
    void Start() {
        lastPos = Input.mousePosition;
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (target == null) return;

        Vector2 currentPosition = Input.mousePosition;

        if (Input.GetMouseButton(0)) {
            Vector2 dir = (currentPosition - lastPos).normalized;
            rot += dir * rotateSpeed * (invert ? -1f : 1f);
        }

        lastPos = currentPosition;
        rot.y = 0;
        Quaternion rotation = Quaternion.Euler(-rot.y, rot.x, 0);

        Vector3 position = target.position + Quaternion.Euler(-rot.y, rot.x, 0f) * (radius * Vector3.back);

        transform.rotation = rotation;
        transform.position = position;
    }
}
