using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BG : MonoBehaviour
{
    public float distance = 100f;
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Camera.main.transform.forward * distance + Camera.main.transform.position;
    }
}
