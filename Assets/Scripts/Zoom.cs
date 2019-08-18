using UnityEngine;

public class Zoom : MonoBehaviour {
    [SerializeField]
    private float zoomLevel = 1;
    public float zoomSpeed;

    public float ZoomLevel { get { return zoomLevel; } }

    // Update is called once per frame
    void Update() {
        zoomLevel = Mathf.Clamp01(zoomLevel + Input.mouseScrollDelta.y * zoomSpeed);
    }
}
