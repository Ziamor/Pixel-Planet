using UnityEngine;

public class SideScroll : MonoBehaviour {
    public float scrollSpeed = 1f;
    public float smoothTime = 0.3F;

    Vector2 lastPos;
    Vector3 scrollVelocity = Vector3.zero;

    // Start is called before the first frame update
    void Start() {
        lastPos = Input.mousePosition;
    }

    // Update is called once per frame
    void Update() {
        Vector2 currentPosition = Input.mousePosition;
        Vector3 target = transform.position;

        if (Input.GetMouseButton(0)) {
            Vector3 swipe = (currentPosition - lastPos);
            float magnitude = swipe.magnitude;
            target += Vector3.right * -Mathf.Sign(swipe.x) * scrollSpeed * Mathf.Clamp01(magnitude);
        }

        transform.position = Vector3.SmoothDamp(transform.position, target, ref scrollVelocity, smoothTime);

        lastPos = currentPosition;
    }
}
