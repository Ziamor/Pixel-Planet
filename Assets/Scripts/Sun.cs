using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    public float speed = 1f;
    public Transform target;
    public float radius = 1f;
    public float rotateSpeed = 1f;
    public Vector3 axis = Vector3.forward;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetDir = target.position - transform.position;

        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, Time.time * speed, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDir);

        /*transform.RotateAround(target.position, axis, rotateSpeed * Time.deltaTime);
        var desiredPosition = (transform.position - target.position).normalized * radius + target.position;
        transform.position = Vector3.MoveTowards(transform.position, desiredPosition, Time.deltaTime * rotateSpeed);*/
    }
}
