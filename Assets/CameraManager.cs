using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // Start is called before the first frame update
    SceneCamera[] cams;
    [SerializeField] Camera cam;
    [SerializeField] float CamSpeed = 10f;
    [SerializeField] Transform skyView;
    [SerializeField] float skyPriorityMargin = 10f;
    [SerializeField] GameObject mainBall;


    //temp
    [SerializeField] GameObject BallGameObj;
    List<GameObject> ballList; //si ca bouge pus cest le temps de controller la boulle

    public Camera Cam
    {
        get { return cam; }
    }

    public bool Active; //daddy gameManager controle ca
    void Start()
    {
        Active = false;
        cams = transform.GetComponentsInChildren<SceneCamera>();


        //temp
        int nbBoules = BallGameObj.transform.childCount;
        ballList = new List<GameObject>();
        for (int i = 0; i < nbBoules; i++)
        {
            ballList.Add(BallGameObj.transform.GetChild(i).gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Active)
        {
            List<((Vector3 pos, GameObject look) movement, float priority)> positions = new();

            foreach (var camera in cams)
            {
                positions.Add(camera.GetBestCamera());
            }
            positions.Sort((el1, el2) => (el2.priority - Mathf.Abs((cam.transform.position - el2.movement.pos).magnitude)).CompareTo(el1.priority - Mathf.Abs((cam.transform.position - el1.movement.pos).magnitude)));
            bool sky = false;
            float prevPrio = 0;
            for(int i =0; i<positions.Count;i++)
            {
                if(i==0)
                {
                    prevPrio = positions[0].priority;
                }
                else
                {
                    if(Mathf.Abs(prevPrio-positions[i].priority)<skyPriorityMargin)
                    {
                        prevPrio = positions[i].priority;
                    }
                    else
                    {
                        break;
                    }
                    if(i==1)
                    {
                        sky = true;
                        break;
                    }
                }
            }

            int val = 0;
            foreach(var el in positions)
            {
                print("pos "+val+", priority: " + el.priority + ", pos: " + el.movement.pos); //debug
                val++;
            }

            if(sky)
            {
                cam.transform.position = Vector3.MoveTowards(cam.transform.position, skyView.position, Time.deltaTime * CamSpeed);
            }
            else
            {
                cam.transform.position = Vector3.MoveTowards(cam.transform.position, positions[0].movement.pos, Time.deltaTime * CamSpeed);
            }

        //    var bestBall = mainBall;
        //    var bestVelocity = 0f;
        //    foreach(var ball in ballList) {
        //        if(ball.GetComponent<Rigidbody>().velocity.magnitude > bestVelocity)
         //       {
         //           bestBall = ball;
        //        }
         //   }


            var targetRotation = Quaternion.LookRotation(positions[0].movement.look.transform.position - cam.transform.position);
            //var targetRotation = Quaternion.LookRotation(bestBall.transform.position - cam.transform.position);

            cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, targetRotation, 2 * Time.deltaTime);

            //cam.transform.LookAt(positions[0].movement.look.transform.position);
        }
    }
}
