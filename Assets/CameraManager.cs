using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    // Start is called before the first frame update
    SceneCamera[] cams;
    [SerializeField] Camera cam;
    [SerializeField] Camera topCam;
    [SerializeField] float CamSpeed = 10f;
    [SerializeField] Transform skyView;
    [SerializeField] float skyPriorityMargin = 20f;
    [SerializeField] float LazySkyMargin = 5f;
    [SerializeField] GameObject mainBall;

    //ui
    [SerializeField] Image CamControlBackground;
    

    bool inSky = false;
    public bool InUI = false;
    bool fixedCam = false; //false = dynamic, true = fixed
    //temp
    [SerializeField] GameObject BallGameObj;
    List<GameObject> ballList; //si ca bouge pus cest le temps de controller la boulle

    public Camera Cam
    {
        get { return cam; }
    }
    public Camera TopCam
    {
        get { return topCam; }
    }

    public bool Active; //daddy gameManager controle ca
    void Start()
    {
        cam.enabled = true;
        topCam.enabled = false;
        fixedCam = false;
        InUI = false;
        CamControlBackground.color = new Color(0.6f, 0.6f, 0.6f, 0.5f);

        Active = false;
        cams = transform.GetComponentsInChildren<SceneCamera>();
        inSky = false;

        //temp
        int nbBoules = BallGameObj.transform.childCount;
        ballList = new List<GameObject>();
        for (int i = 0; i < nbBoules; i++)
        {
            ballList.Add(BallGameObj.transform.GetChild(i).gameObject);
        }
    }

    Vector3 GetBestLookPos()
    {
        Vector3 res = Vector3.zero;
        List<(float vel, Vector3 pos)> fastBalls = new();
        float totalVel = 0;
        foreach(var ball in ballList)
        {
            if(ball.activeSelf) { 
            float vel = ball.GetComponent<Rigidbody>().velocity.magnitude;
            if(vel > 0)
            {
                fastBalls.Add((vel, ball.transform.position));
                totalVel += vel;
            }
            }
        }
        if (totalVel == 0)
        {
            return mainBall.transform.position;
        }
        foreach(var ball in fastBalls)
        {
            res += ball.pos*ball.vel;
           // print("pos: " + ball.pos + ", vel:" + ball.vel);
        }
        res = res / totalVel;
       // print("main: " + mainBall.transform.position + ", lookAvg:" + res + ", totalVel: " + totalVel);
        return res;
    }

    // Update is called once per frame
    void Update()
    {
        if (Active)
        {
            List<(Vector3 pos, float priority)> positions = new();

            foreach (var camera in cams)
            {
                positions.Add(camera.GetBestCamera());
            }
            positions.Sort((el1, el2) => (el2.priority).CompareTo(el1.priority));
            
            if(inSky)
            {
                if (positions[0].priority - positions[1].priority > skyPriorityMargin + LazySkyMargin)
                {
                    inSky = false;
                }
            }
            else
            {
                if(positions[0].priority - positions[1].priority < skyPriorityMargin)
                {
                    inSky = true;
                }
            }

    //        int val = 0;
    //        foreach(var el in positions)
     //       {
      //          print("pos "+val+", priority: " + el.priority + ", pos: " + el.pos); //debug
      //          val++;
      //      }

            if(inSky)
            {
                cam.transform.position = Vector3.MoveTowards(cam.transform.position, skyView.position, Time.deltaTime * CamSpeed);
            }
            else
            {
                cam.transform.position = Vector3.MoveTowards(cam.transform.position, positions[0].pos, Time.deltaTime * CamSpeed);
            }

        //    var bestBall = mainBall;
        //    var bestVelocity = 0f;
        //    foreach(var ball in ballList) {
        //        if(ball.GetComponent<Rigidbody>().velocity.magnitude > bestVelocity)
         //       {
         //           bestBall = ball;
        //        }
         //   }




            var targetRotation = Quaternion.LookRotation(GetBestLookPos() - cam.transform.position);
            //var targetRotation = Quaternion.LookRotation(bestBall.transform.position - cam.transform.position);

            cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, targetRotation, 5 * Time.deltaTime);

            //cam.transform.LookAt(positions[0].movement.look.transform.position);
        }
    }

    public void ToggleCam()
    {
        Color red = new Color(1, 0.1f, 0.1f, 0.5f);
        Color grey = new Color(0.6f, 0.6f, 0.6f, 0.5f);
        cam.enabled = topCam.enabled;
        topCam.enabled = !cam.enabled;
        if (!cam.enabled)
        {
            CamControlBackground.color = red;
            fixedCam = true;
        }
        else
        {
            CamControlBackground.color = grey;
            fixedCam = false;
        }
    }

    public void ToggleCamEnter()
    {
        Color solid = CamControlBackground.color;
        solid.a = 1f;
        CamControlBackground.color = solid;
        InUI = true;
    }
    public void ToggleCamExit()
    { //fulcrum come in
        Color faded = CamControlBackground.color;
        faded.a = 0.5f;
        CamControlBackground.color = faded;
        InUI = false;
    }
    public void LockTop()
    {
        CamControlBackground.gameObject.SetActive(false);
        if (!fixedCam)
        {
            cam.enabled = false;
            topCam.enabled = true;
        }
    }
    public void UnlockTop()
    {
        CamControlBackground.gameObject.SetActive(true);
        if(!fixedCam)
        {
            cam.enabled = true;
            topCam.enabled = false;
        }
    }
}
