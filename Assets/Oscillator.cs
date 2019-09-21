using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movementVector = new Vector3(10f,10f,10f);
    [SerializeField] float period = 2f;

    //[Range(0,1)] [SerializeField] float movementFactor; // 0 for not move, 1 for fully moved
    float movementFactor;
    Vector3 startingPos; // must be stored for absolute movement
    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position; // get initial position of gameObject from Transform componenet
    }

    // Update is called once per frame
    void Update()
    {
        //protect against period becoming 0
        if (period <= Mathf.Epsilon) { return; }
        float cycles = Time.time / period;
        const float tau = Mathf.PI * 2f; // about 6.28
        float rawSinWave = Mathf.Sin(cycles * tau);

        movementFactor = rawSinWave / 2f + 0.5f;
        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPos + offset; 
        
    }
}
