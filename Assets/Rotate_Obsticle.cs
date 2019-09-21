using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate_Obsticle : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] float rotationFactor = 1f;
    float currentRotationZ;
    void Start()
    {
        currentRotationZ = transform.rotation.z;
    }

    // Update is called once per frame
    void Update()
    {
        currentRotationZ -= rotationFactor * Time.deltaTime;
        Quaternion newRotation = Quaternion.Euler(0, 0, currentRotationZ);
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime);

    }
}
