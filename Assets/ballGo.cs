using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballGo : MonoBehaviour
{
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = new Vector3(60, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
     
    }
}
