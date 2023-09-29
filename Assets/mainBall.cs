using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainBall : MonoBehaviour
{
    [SerializeField] GameManager GM;
    public bool GoodPlacement
    {
        get { return triggerEmpty; }
    }
    int count = 0;
    bool triggerEmpty = true;
    // Start is called before the first frame update
    void Start()
    {
        count = 0;
        triggerEmpty = true;
    }

    // Update is called once per frame
    void Update()
    {
        //print(count);
    }
    private void FixedUpdate()
    {
        print("trigger empty: " + triggerEmpty);
        triggerEmpty = true;
        if(transform.position.y <-2)
        {
            GM.RequestMainBallPlacement();
            gameObject.SetActive(false);
            transform.position = new Vector3(0, 2, 0);
            gameObject.GetComponent<Collider>().enabled = false;
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }
       
    }
    private void OnTriggerStay(Collider other)
    {
        triggerEmpty = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        count++;
       
    }
    private void OnTriggerExit(Collider other)
    {
        count--;
    }
}
