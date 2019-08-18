using UnityEngine;

public class Orbit : MonoBehaviour {
    public float radius = 1f;
    public float speed = 1f;
    public bool angled = false;
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        Vector3 pos = transform.position;

        pos.x = Mathf.Sin(Time.time * speed) * radius;
        if (angled)
            pos.y = Mathf.Sin(Time.time * speed) * radius;
        pos.z = Mathf.Cos(Time.time * speed) * radius;

        transform.position = pos;
    }
}
