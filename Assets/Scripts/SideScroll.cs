using UnityEngine;

public class SideScroll : MonoBehaviour {
    public GameObject[] planets;

    public float scrollSpeed = 1f;
    public float smoothTime = 0.3F;

    Vector2 lastPos;
    Vector3 scrollVelocity = Vector3.zero;

    Vector3 startPos;

    int index = 0;

    float swipeTime = 0;

    // Start is called before the first frame update
    void Start() {
        lastPos = Input.mousePosition;
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update() {
        Vector2 currentPosition = Input.mousePosition;


        if (Time.time - swipeTime >= smoothTime && Input.GetMouseButton(0)) {
            swipeTime = Time.time;
            Vector3 swipe = (currentPosition - lastPos);
            float magnitude = swipe.magnitude;
            if (swipe.x != 0 && magnitude > 0.01f) {
                int newIndex = index;
                if (swipe.x < 0) {
                    newIndex++;
                } else {
                    newIndex--;
                }

                if (IsValidIndex(newIndex)) {
                    index = newIndex;
                }
            }
        }
        if (IsValidIndex(index)) {
            Vector3 target = planets[index].transform.position;
            target.z = startPos.z;
            transform.position = Vector3.SmoothDamp(transform.position, target, ref scrollVelocity, smoothTime);
        }

        lastPos = currentPosition;
    }

    bool IsValidIndex(int i) {
        return i >= 0 && i <= planets.Length - 1 && planets[i] != null;
    }
}
