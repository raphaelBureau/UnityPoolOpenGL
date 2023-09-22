using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] CameraManager CM;
    [SerializeField] GameObject BallGameObj;
    List<GameObject> ballList; //si ca bouge pus cest le temps de controller la boulle
    [SerializeField] GameObject mainBall;

    bool playing = false;
    void Start()
    {
        //initialisation du jeu
        int nbBoules = BallGameObj.transform.childCount;
        ballList = new List<GameObject>();
        for(int i = 0; i<nbBoules;i++)
        {
            ballList.Add(BallGameObj.transform.GetChild(i).gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //gerer si le camera manager gere la camera
        if(!CM.Active)
        {
            if(playing)
            {

            }
            else //deplacer la camera vers la ballle
            {
                Vector3 camDest = mainBall.transform.position + new Vector3(-5, 3, 0);
                if (CM.Cam.transform.position == camDest)
                {
                    playing = true;
                }
                else
                {
                   CM.Cam.transform.position = CM.Cam.transform.position + (camDest - CM.transform.position) * Time.deltaTime;
                }
            }
        }
    }
    private void FixedUpdate() //check la velocite donc fixedupdate
    {
        if(CM.Active)
        {
            bool Moved = false;
            foreach(var ball in ballList)
            {
                if(ball.GetComponent<Rigidbody>().velocity.magnitude > 0.01) //minimum acceptable movement
                {
                    Moved = true;
                }
            }
            if(!Moved)
            {
                CM.Active = false;
            }
        }
    }
}
