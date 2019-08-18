using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Temp till I figure out how I want the bg to work
        Vector3 newPos = Camera.main.transform.position;
        newPos.z = 10;
        transform.position = newPos;
    }
}
