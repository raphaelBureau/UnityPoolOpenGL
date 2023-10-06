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
        if (!GM.Multiplayer || GM.sendPackets)
        {
            if (transform.position.y < -2) //tombé
            {
                GM.BallFell(BallNo, SolidColor);
                gameObject.SetActive(false);

                if(GM.Multiplayer)
                {
                    GM.Net.SendBallData(new BallData(ballNo, gameObject.activeSelf, false));
                }
            }
        }
    }
    [System.Serializable]
    public class BallData
    {
       public int id { get; private set; }
       public bool active { get; private set; }
        public bool collisions { get; private set; }
        public BallData(int id, bool active,bool collisions)
        {
            this.id = id; //mainball id = 16
            this.active = active;
            this.collisions = collisions;
        }
    }
}

