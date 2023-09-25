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
    [SerializeField] GameObject trajectory;

    (bool left, bool right) mouseClick;
    (bool left, bool right) prevMouseClick;
    float hitStrenght = 30f;
    Vector3 mouseMove;
    Vector3 prevMouseMove;
    Resolution screen;

    bool playing;
    void Start()
    {
        //initialisation du jeu
        mouseClick = (false, false);
        mouseMove = Vector3.zero;
        screen = Screen.currentResolution;
        playing = false; //doit etre false pour placer la camera au bon endroit
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
        if (!CM.Active)
        {
            if (playing)
            { //donne le controlle de la camera au user et permet de frapper la boulle
                mouseClick.left = Input.GetMouseButton(0);
                mouseClick.right = Input.GetMouseButton(1);
                hitStrenght += Input.mouseScrollDelta.y;
                mouseMove = Input.mousePosition;
                if (mouseClick.right)
                {//user controls
                 //y rotation
                    float yR = (mouseMove.x - prevMouseMove.x) / screen.width * 100;
                    CM.Cam.transform.RotateAround(mainBall.transform.position, CM.Cam.transform.up, yR);

                    //x rotation
                    float xR = (mouseMove.y - prevMouseMove.y) / screen.height * 100;
                    CM.Cam.transform.RotateAround(mainBall.transform.position, CM.Cam.transform.right, -xR);


                    CM.Cam.transform.LookAt(mainBall.transform.position);

                    trajectory.transform.position = new Vector3(mainBall.transform.position.x, 0.6f, mainBall.transform.position.z) + new Vector3(CM.Cam.transform.forward.x, 0, CM.Cam.transform.forward.z).normalized * hitStrenght / 2;
                    trajectory.transform.localScale = new Vector3(0.02f, 1, hitStrenght / 10f);
                    trajectory.transform.LookAt(new Vector3(mainBall.transform.position.x, 0.6f, mainBall.transform.position.z) + new Vector3(CM.Cam.transform.forward.x, 0, CM.Cam.transform.forward.z).normalized * hitStrenght);
                    trajectory.GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(1,hitStrenght / 2);
                }

                if(mouseClick.left)
                {//firrre!!!
                    mainBall.GetComponent<Rigidbody>().velocity =  new Vector3(CM.Cam.transform.forward.x,0, CM.Cam.transform.forward.z).normalized * hitStrenght;
                    trajectory.GetComponent<MeshRenderer>().enabled = false;
                    playing = false;
                    CM.Active = true;
                }

                prevMouseClick = mouseClick;
                prevMouseMove = mouseMove;

            }
            else //deplacer la camera vers la ballle
            {
                var targetRotation = Quaternion.LookRotation(mainBall.transform.position - CM.Cam.transform.position);
                if (Mathf.Abs((CM.Cam.transform.position - mainBall.transform.position + Vector3.up*2).magnitude) <= 7.1 && (Mathf.Abs(CM.Cam.transform.rotation.w - targetRotation.w + CM.Cam.transform.rotation.x - targetRotation.x) <5))
                {
                    playing = true;
                    trajectory.GetComponent<MeshRenderer>().enabled = true;
                    trajectory.transform.position = new Vector3(mainBall.transform.position.x, 0.6f, mainBall.transform.position.z) + new Vector3(CM.Cam.transform.forward.x, 0, CM.Cam.transform.forward.z).normalized * hitStrenght / 2;
                    trajectory.transform.localScale = new Vector3(0.02f, 1, hitStrenght / 10f);
                    trajectory.transform.LookAt(new Vector3(mainBall.transform.position.x, 0.6f, mainBall.transform.position.z) + new Vector3(CM.Cam.transform.forward.x, 0, CM.Cam.transform.forward.z).normalized * hitStrenght);
                    trajectory.GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(1, hitStrenght / 2);
                }
                else
                {
                    if (Mathf.Abs((CM.Cam.transform.position - mainBall.transform.position +Vector3.up * 2).magnitude) > 7)
                    {
                        CM.Cam.transform.position = Vector3.MoveTowards(CM.Cam.transform.position, mainBall.transform.position + Vector3.up * 2, 10 * Time.deltaTime);
                    }

                    
                    CM.Cam.transform.rotation = Quaternion.Slerp(CM.Cam.transform.rotation, targetRotation, 3 * Time.deltaTime);
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
                if(ball.GetComponent<Rigidbody>().velocity.magnitude > 0.05 && ball.activeSelf) //minimum acceptable movement
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
