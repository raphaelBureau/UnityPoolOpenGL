using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballScript : MonoBehaviour
{
    [SerializeField] int ballNo = 8;
    [SerializeField] bool solidColor = true;

    GameManager GM;

    public bool SolidColor
    {
        get { return solidColor; }
    }

    public int BallNo
    {
        get { return ballNo; }
    }
    // Start is called before the first frame update
    void Start()
    {
        GM = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
            if (GM.sendPackets && transform.position.y < -2) //tombé
            {
            print("ball fell n:" + ballNo);
                GM.BallFell(BallNo, SolidColor);

                gameObject.SetActive(false);
         //   print("sending data");
            GM.Net.SendBallData(new BallData(ballNo, false, true)); //on laisse les collisions a true parceque lobjet est desactivé
        //    print("data sent");
            //  print("send ball data id: " + ballNo + " not active, collisions enabled");
        }
        
    }
    [System.Serializable]
    public class BallData
    {
        public int id;
        public bool active; //important de ne pas set de proprietes parceque unity supporte pas ca en serializable
        public bool collisions;
        public BallData(int id, bool active,bool collisions)
        {
            this.id = id; //mainball id = 16
            this.active = active;
            this.collisions = collisions;
        }
    }
}

