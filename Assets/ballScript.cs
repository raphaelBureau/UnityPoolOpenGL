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
        if(transform.position.y < -2) //tombé
        {
            GM.BallFell(BallNo, SolidColor);
            gameObject.SetActive(false);
        }
    }
}
