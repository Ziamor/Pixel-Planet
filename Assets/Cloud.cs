﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    public float rotateSpeed = 1;
    public float roateSpeedVariance = 1f;

    Material mat;
    Renderer cloudRenderer;
    MaterialPropertyBlock propBlock;

    Vector2 lastPos;
    Vector2 pos;
    // Start is called before the first frame update
    void Start()
    {
        Init();
        pos = Random.insideUnitCircle;
        lastPos = Input.mousePosition;
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
        lastPos = currentPosition;
        cloudRenderer.SetPropertyBlock(propBlock);
    }

    void Init() {
        propBlock = new MaterialPropertyBlock();
        cloudRenderer = GetComponent<MeshRenderer>();
        mat = cloudRenderer.sharedMaterial;
    }
}