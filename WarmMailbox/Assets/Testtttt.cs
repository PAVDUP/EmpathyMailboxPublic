using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using UnityEngine;

public class Testtttt : MonoBehaviour
{
    float timee = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timee += Time.deltaTime;
        if (timee <= 1)
            GetComponent<Rigidbody>().velocity = new Vector3(0, 1, 1);
        else
            GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}
