using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ballScript;

public class mainBall : MonoBehaviour
{
    [SerializeField] GameManager GM;
    public bool GoodPlacement
    {
        get { return triggerEmpty; }
    }
    int count = 0;
    bool triggerEmpty = true;
    public bool placing = false;
    // Start is called before the first frame update
    void Start()
    {
        placing = false;
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
      //  print("trigger empty: " + triggerEmpty);
        triggerEmpty = true;
        if (!GM.Multiplayer || GM.sendPackets && !placing)
        {
            if (transform.position.y < -2)
            {
                
                gameObject.SetActive(false);
                transform.position = new Vector3(0, 2, 0);
                gameObject.GetComponent<Collider>().enabled = false;
                gameObject.GetComponent<Rigidbody>().isKinematic = true;

                if (GM.Multiplayer)
                {
                    GM.Net.SendBallData(new BallData(0, gameObject.activeSelf, false));
                }
                else
                {
                    GM.RequestMainBallPlacement();
                }
            }
        }
       
    }
    private void OnTriggerStay(Collider other)
    {
        triggerEmpty = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        count++;
        triggerEmpty = false; //peut etre un fix ??
       
    }
    private void OnTriggerExit(Collider other)
    {
        count--;
    }
}
