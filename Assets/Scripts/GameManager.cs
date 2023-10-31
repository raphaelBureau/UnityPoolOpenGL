using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static ballScript;
using System.Runtime.InteropServices;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] CameraManager CM;
    [SerializeField] GameObject BallGameObj;
    List<GameObject> ballList; //si ca bouge pus cest le temps de controller la boulle
    [SerializeField] GameObject mainBall;
    [SerializeField] GameObject trajectory;
    [SerializeField] Networking net;
    [SerializeField] TextMeshProUGUI Message;

    [DllImport("__Internal")] private static extern bool IsMobile();

    public GameObject Trajectory
    {
        get
        {
            return trajectory;
        }
    }

    public Networking Net
    {
        get { return net; }
    }

    public List<GameObject> BallList
    {
        get { return ballList; } //lencalplsulation
    }

    public bool sendPackets = false;

    public bool Multiplayer
    {
        get { return multiplayer; }
    }

    [SerializeField] GameObject player1Display;
    [SerializeField] GameObject player2Display;

    (bool left, bool right) mouseClick;
    (bool left, bool right) prevMouseClick;
    float hitStrenght = 30f;
    Vector3 mouseMove = new Vector3(0, 0, 0);
    Vector3 prevMouseMove = new Vector3(0,0,0);
    Resolution screen;

    bool playing;

    public bool Playing { get { return playing; } }

    bool multiplayer = false;

    string player1Name = "Joueur 1";
    string player2Name = "Joueur 2";

    bool placingMainBall = false;
    bool mainBallFell = false;

    bool playerTurn = false; //false = player 1, true = player 2;
    bool playAgain = false; //si un point = true

    public bool mainBallNoReplay = false;

    bool playerBallClick = false;

    bool firstBallFell = false;
    bool player1Solid;

    int player1Points = 0;
    int player2Points = 0;

    float playerSens = 1;

    void Start()
    {
        mainBallNoReplay = false;
        sendPackets = false;
        multiplayer = false;
        playerTurn = false;
        playAgain = false;
        player1Points = 0;
        player2Points = 0;
        firstBallFell = false;
        //initialisation du jeu
        mainBallFell = false;
        placingMainBall = false;
        mouseClick = (false, false);
        mouseMove = Vector3.zero;
        screen = Screen.currentResolution;
        playing = false; //doit etre false pour placer la camera au bon endroit
        int nbBoules = BallGameObj.transform.childCount;
        ballList = new List<GameObject>();
        for (int i = 0; i < nbBoules; i++)
        {
            ballList.Add(BallGameObj.transform.GetChild(i).gameObject);
            //  print("balllist id: " + i + " name: " + ballList[i].name);
        }

        UpdateProfiles();
    }

    // Update is called once per frame
    void Update()
    {
        if (placingMainBall && sendPackets)
        {
            PlaceMainBall();
        }
        else
        {
            //gerer si le camera manager gere la camera
            if (!CM.Active)
            {
                if (playing)
                { //donne le controlle de la camera au user et permet de frapper la boulle

                    if (HandleInput()) //retoune true si un input a change
                    {
                        UpdatePlayControls();
                    }

                }
                else //deplacer la camera vers la ballle
                {
                    MoveCamToBall();
                }
            }
        }
    }
    private void FixedUpdate() //check la velocite donc fixedupdate
    {
        if (CM.Active && sendPackets)
        {
            bool Moved = false;
            foreach (var ball in ballList)
            {
                if (ball.GetComponent<Rigidbody>().velocity.magnitude > 0 && ball.activeSelf) //minimum acceptable movement
                {
                    Moved = true;
                }
            }
            if (!Moved)
            {
                if (playAgain && !mainBallNoReplay)
                {
                    CM.Active = false;
                }
                else
                {
                    playerTurn = !playerTurn;
                    net.GiveOtherPlayerControl();
                    sendPackets = false;
                    UpdateProfiles();
                    mainBallNoReplay = false;
                }
            }
        }
    }
    bool HandleInput()
    {
        bool changed = false;
        Transform camTran = CM.Cam.transform;
        Vector3 ballPos = mainBall.transform.position;

        if (IsMobile())
        {//mobile
            switch(Input.touchCount)
            {
                case 0:
                    prevMouseMove = Vector3.zero;
                    break;
                case 1: //camera move
                    mouseMove = Input.GetTouch(0).position * playerSens;
                    if(prevMouseMove == Vector3.zero)
                    {
                        prevMouseMove = mouseMove;
                    }
                    if ((mouseMove - prevMouseMove).magnitude > 0)
                    {//user controls
                     //y rotation
                        float yR = (mouseMove.x - prevMouseMove.x) / screen.width * 100;
                        camTran.RotateAround(ballPos, camTran.up, yR);

                        //x rotation
                        float xR = (mouseMove.y - prevMouseMove.y) / screen.height * 100;
                        camTran.RotateAround(ballPos, camTran.right, -xR);


                        camTran.LookAt(ballPos);
                        changed = true;
                        prevMouseMove = mouseMove;
                    }
                    break;
                case 2: //fire
                    playerBallClick = true;
                    changed = true;
                    break;

            }
        }
        else
        { //desktop
            mouseClick.left = Input.GetMouseButton(0);
            mouseClick.right = Input.GetMouseButton(1);

            mouseMove = Input.mousePosition;
            if (prevMouseMove == Vector3.zero)
            {
                prevMouseMove = mouseMove;
            }
            if (mouseClick.right && (mouseMove - prevMouseMove).magnitude>0)
            {//user controls
             //y rotation
                float yR = (mouseMove.x - prevMouseMove.x) / screen.width * 100;
                camTran.RotateAround(ballPos, camTran.up, yR);

                //x rotation
                float xR = (mouseMove.y - prevMouseMove.y) / screen.height * 100;
                camTran.RotateAround(ballPos, camTran.right, -xR);


                camTran.LookAt(ballPos);
                prevMouseMove = mouseMove;
                changed = true;
            }
            else
            {
                prevMouseMove = mouseMove;
            }

            if (Mathf.Abs(Input.mouseScrollDelta.y) > 0)
            {
                hitStrenght += Input.mouseScrollDelta.y;
                changed = true;
            }
            if (prevMouseClick.left != mouseClick.left || prevMouseClick.right != mouseClick.right)
            {
                changed = true;
                prevMouseClick = mouseClick;
                if(mouseClick.left)
                {
                    playerBallClick = true;
                }
            }
        }
        return changed;
    }
    void UpdatePlayControls()
    {
        Vector3 ball = mainBall.transform.position;
        Vector3 camf = CM.Cam.transform.forward;
        trajectory.transform.position = new Vector3(ball.x, 0.6f, ball.z) + new Vector3(camf.x, 0, camf.z).normalized * hitStrenght / 2;
        trajectory.transform.localScale = new Vector3(0.02f, 1, hitStrenght / 10f);
        trajectory.transform.LookAt(new Vector3(ball.x, 0.6f, ball.z) + new Vector3(camf.x, 0, camf.z).normalized * hitStrenght);
        trajectory.GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(1, hitStrenght / 2);

        if (playerBallClick && !CM.InUI)
        {//firrre!!!
            mainBall.GetComponent<Rigidbody>().velocity = new Vector3(CM.Cam.transform.forward.x, 0, CM.Cam.transform.forward.z).normalized * hitStrenght;
            trajectory.SetActive(false);

            playAgain = false;
            playing = false;
            CM.Active = true;
            playerBallClick = false;
            Net.UpdateAllBalls();
        }
    }
    void PlaceMainBall()
    {
        net.SendBallData(new BallData(0, true, false)); //
                                                        // print("raycasting moment");
        RaycastHit hit;
        Ray ray = CM.TopCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 50f, LayerMask.GetMask("Surface")))
        {
            mainBall.transform.position = new Vector3(hit.point.x, 1.3f, hit.point.z);
            mainBall.GetComponent<Renderer>().material.color = new Color(1f, 0.5f, 0.5f);
            //  print(mainBall.GetComponent<mainBall>().GoodPlacement);
            if (mainBall.GetComponent<mainBall>().GoodPlacement)
            {
                mainBall.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f);
                if (Input.GetMouseButton(0))
                {

                    mainBall.layer = 8;
                    mainBall.GetComponent<Collider>().isTrigger = false;
                    mainBall.GetComponent<Rigidbody>().isKinematic = false;
                    CM.UnlockTop();
                    placingMainBall = false;

                    Net.UpdateAllBalls();
                    net.SendBallData(new BallData(0, true, true));
                    mainBall.GetComponent<mainBall>().placing = false;
                    mainBallFell = false;
                    mainBallNoReplay = false;

                }
            }
        }
    }
    void MoveCamToBall()
    {
        Transform camTran = CM.Cam.transform;
        Vector3 ballPos = mainBall.transform.position;

        Quaternion targetRotation = Quaternion.LookRotation(ballPos - camTran.position);
        if (Mathf.Abs((camTran.position - mainBall.transform.position + Vector3.up * 2).magnitude) > 7)
        {
            camTran.position = Vector3.MoveTowards(camTran.position, ballPos + Vector3.up * 2, 10 * Time.deltaTime);
        }

        camTran.rotation = Quaternion.Slerp(camTran.rotation, targetRotation, 3 * Time.deltaTime);

        if (Mathf.Abs((camTran.position - ballPos + Vector3.up * 2).magnitude) <= 7.1 && (Mathf.Abs(camTran.rotation.w - targetRotation.w + camTran.rotation.x - targetRotation.x) < 5))
        {
            playing = true;
            trajectory.SetActive(true);
            trajectory.transform.position = new Vector3(ballPos.x, 0.6f, ballPos.z) + new Vector3(camTran.forward.x, 0, camTran.forward.z).normalized * hitStrenght / 2;
            trajectory.transform.localScale = new Vector3(0.02f, 1, hitStrenght / 10f);
            trajectory.transform.LookAt(new Vector3(ballPos.x, 0.6f, ballPos.z) + new Vector3(camTran.forward.x, 0, camTran.forward.z).normalized * hitStrenght);
            trajectory.GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(1, hitStrenght / 2);
        }

    }
    void MainBallPlacementRequest()
    {
        CM.LockTop();
        placingMainBall = true;
        mainBall.layer = 7;
        mainBall.SetActive(true);
        mainBall.GetComponent<Collider>().isTrigger = true;
        mainBall.GetComponent<Collider>().enabled = true;
        mainBall.GetComponent<Rigidbody>().isKinematic = true;
        mainBall.GetComponent<mainBall>().placing = true;

        if (multiplayer)
        {
            net.SendBallData(new BallData(0, true, false));
        }
    }
    public void BallFell(int ballNo, bool solidColor)
    {
        if (ballNo == 8)
        {//game over
            Time.timeScale = 0;
            if (playerTurn)
            {
                if (player2Points == 7)
                {
                    //win
                    Message.text = player2Name + " a gagné la partie";
                }
                else
                {
                    //loose
                    Message.text = player1Name + " a gagné la partie, " + player2Name + " a fait tomber la boule 8 avant les boules 9-15";
                }
            }
            else
            {
                if (player1Points == 7)
                {
                    //win
                    Message.text = player1Name + " a gagné la partie";
                }
                else
                {
                    //loose
                    Message.text = player2Name + " a gagné la partie, " + player1Name + " a fait tomber la boule 8 avant les boules 1-7";
                }
            }
        }
        else
        {
            if (!firstBallFell)
            {
                firstBallFell = true;

                if (!playerTurn)
                {//player 1
                    player1Solid = solidColor;
                    player1Points++;

                }
                else
                {//player 2
                    player1Solid = !solidColor;
                    player2Points++;
                }
                playAgain = true;
                UpdateProfiles();
                return;
            }

            if (solidColor)
            {
                if (player1Solid)
                {
                    player1Points++;

                    if (!playerTurn)
                    {
                        playAgain = true;
                    }
                }
                else
                {
                    player2Points++;

                    if (playerTurn)
                    {
                        playAgain = true;
                    }
                }
            }
            else //pas solid
            {
                if (!player1Solid)
                {
                    player1Points++;

                    if (!playerTurn)
                    {
                        playAgain = true;
                    }
                }
                else
                {
                    player2Points++;

                    if (playerTurn)
                    {
                        playAgain = true;
                    }
                }
            }
        }
        UpdateProfiles();
    }
    public void UpdateBalls(BallData ball)
    {
        //print("update ball id:" + ball.id + " active: " + ball.active + " collisions: " + ball.collisions);
        if (ball.id == 0) //main ball
        {
            if (ball.collisions) //si collisions, la balle est placée, sinon, elle est en placement du joueur
            {
                mainBall.SetActive(ball.active);
                mainBall.GetComponent<Collider>().isTrigger = false;
                mainBall.GetComponent<Collider>().enabled = true;
                mainBall.GetComponent<Rigidbody>().isKinematic = false;
            }
            else
            {
                mainBall.SetActive(ball.active);
                mainBall.GetComponent<Collider>().isTrigger = true;
                mainBall.GetComponent<Collider>().enabled = true;
                mainBall.GetComponent<Rigidbody>().isKinematic = true;
            }
            if (!ball.active) //si le gameobject est disabled la balle est tombe dans un trou donc le joueur doit la placer au prochain tour
            {
                mainBallFell = true;
            }
        }
        else
        {//balle normalle 
            ballList[ball.id].SetActive(ball.active);
            BallFell(ball.id, ball.id <= 8);
        }
    }
    public void EnableMultiplayer(bool first)
    {
        multiplayer = true;
        sendPackets = first;
        CM.Active = !first;
        UpdateProfiles();
    }
    public void OtherPlayerPlayRequest() //resume la partie, assume que chaque balle a cessé de bouger
    {
        CM.Active = false;
        sendPackets = true;
        playerTurn = !playerTurn;
        UpdateProfiles();
        if (mainBallFell)
        {
            MainBallPlacementRequest();
        }
    }
    void UpdateProfiles()
    {
        if (!playerTurn)
        {

            player1Display.GetComponent<Image>().color = new Color(0.8f, 0.6f, 0.6f, 0.7f);
        }
        else
        {
            player1Display.GetComponent<Image>().color = new Color(0.6f, 0.6f, 0.6f, 0.5f);
        }
        TextMeshProUGUI[] text1 = player1Display.GetComponentsInChildren<TextMeshProUGUI>(); //lenght 3, 0= nom, 1= score, 2 = balltype

        text1[0].text = player1Name;

        text1[1].text = "Score: " + player1Points;

        if (firstBallFell)
        {
            text1[2].text = player1Solid ? "Solide" : "Rayé";
        }

        if (playerTurn)
        {
            player2Display.GetComponent<Image>().color = new Color(0.8f, 0.6f, 0.6f, 0.7f);

        }
        else
        {
            player2Display.GetComponent<Image>().color = new Color(0.6f, 0.6f, 0.6f, 0.5f);
        }
        TextMeshProUGUI[] text2 = player2Display.GetComponentsInChildren<TextMeshProUGUI>(); //lenght 3, 0= nom, 1= score, 2 = balltype

        text2[0].text = player2Name;

        text2[1].text = "Score: " + player2Points;

        if (firstBallFell)
        {
            text2[2].text = player1Solid ? "Rayé" : "Solide";
        }
    }
}
