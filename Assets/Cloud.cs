using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    public float rotateSpeed = 1;
    public float roateSpeedVariance = 1f;
    public float radius = 1;

    Material mat;
    Renderer cloudRenderer;
    MaterialPropertyBlock propBlock;

    Vector2 lastPos;
    Vector2 pos;

    Texture texture = null;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (mat == null)
            Init();
        cloudRenderer.GetPropertyBlock(propBlock);

        Vector2 currentPosition = Input.mousePosition;
        if (Input.GetMouseButton(0)) {
            Vector2 dir = (currentPosition - lastPos).normalized;
            pos += dir * rotateSpeed * roateSpeedVariance;
            propBlock.SetVector("_Offset", pos);
        }

        if(texture != null)
            propBlock.SetTexture("_MainTex", texture);
        lastPos = currentPosition;
        propBlock.SetFloat("_Radius", radius);
        cloudRenderer.SetPropertyBlock(propBlock);
    }

    public void Init() {
        lastPos = Input.mousePosition;
        propBlock = new MaterialPropertyBlock();
        cloudRenderer = GetComponent<MeshRenderer>();
        mat = cloudRenderer.sharedMaterial;
    }

    public void SetTexture(Texture tex) {
        if (mat == null)
            Init();

        texture = tex;
    }
}
