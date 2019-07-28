using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[ExecuteInEditMode]
public class Planet : MonoBehaviour
{
    public float rotateSpeed = 1;

    MeshRenderer meshRenderer;
    Material mat;

    Vector2 rot;

    Vector2 lastPos;
    // Start is called before the first frame update
    void Start()
    {
        Init();
        lastPos = Input.mousePosition;
    }

    // Update is called once per frame
    void Update()
    {
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
        meshRenderer = GetComponent<MeshRenderer>();
        mat = meshRenderer.sharedMaterial;
    }
}
