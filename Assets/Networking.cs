using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using KyleDulce.SocketIo;
using TMPro;
using static ballScript;

public class Networking : MonoBehaviour
{
    // Start is called before the first frame update
    Socket socket;
    [SerializeField] GameManager GM;
    [SerializeField] TextMeshProUGUI Message;

    [DllImport("__Internal")] private static extern void sendMessage(string mess);

    [DllImport("__Internal")] private static extern void setName(string name);

    int frameCounter = 0;

    void Start()
    {
        WebGLInput.captureAllKeyboardInput = false;
        Time.timeScale = 0;//stop and wait for player 2
        frameCounter = 0;
        Message.text = "Connection au Serveur...";
        socket = SocketIo.establishSocketConnection("wss://bureau.blue:3000");
        socket.connect();
 
        Message.text = "En attente d'un autre joueur";
        socket.emit("joinGame", "pool"); //send connection request

        socket.on("connect_error", (arg) => {
            Message.text = "Erreur de connection";
        });

        socket.on("playerLeave", (arg) =>
        {
            Message.text = "l'adversaire a quitté";
            print("player left");
            Time.timeScale = 0;
        });

        socket.on("gameStart", (first) =>
        {
            Message.text = "";
            print("connected");
            GM.EnableMultiplayer(bool.Parse(first));
            setName(bool.Parse(first) ? "Joueur 1" : "Joueur 2");
            Time.timeScale = 1; //play game;
        });

        socket.on("control", (args) =>
        {
            GM.OtherPlayerPlayRequest();
        });

        socket.on("updateBalls", (args) =>
        {
            GM.UpdateBalls(JsonUtility.FromJson<BallData>(args));
        });

        socket.on("updateTrajectory", (arg) =>
        {

        });


        socket.on("positions", (res) => //array 6 * length , [x,y,z, velX, velY, velZ]  i+=6;
        {
         //print(res);

            if(GM.Trajectory.activeSelf)
            {
                GM.Trajectory.SetActive(false);
            }
 
            Position pos = JsonUtility.FromJson<Position>(res);
            
            List<GameObject> balls = GM.BallList;
           
            int ball = 0;
            for (int i = 0; i < pos.data.Length; i += 6)
            {
                balls[ball].transform.position = new Vector3(pos.data[i], pos.data[i + 1], pos.data[i + 2]);
                balls[ball].GetComponent<Rigidbody>().velocity = new Vector3(pos.data[i + 3], pos.data[i + 4], pos.data[i + 5]);
                ball++;
            }
        });

        socket.on("trajectory", (trajectory) =>
        {
            Trajectory traj = JsonUtility.FromJson<Trajectory>(trajectory);

            if(!GM.Trajectory.activeSelf)
            {
                GM.Trajectory.SetActive(true);
            }

            GM.Trajectory.transform.position = traj.position;
            GM.Trajectory.transform.localScale = traj.scale;
            GM.Trajectory.GetComponent<MeshRenderer>().material.mainTextureScale = traj.texture;
            GM.Trajectory.transform.rotation = traj.rotation;
        });

        socket.on("sendMessage", (message) =>
        {
            sendMessage(message);
        });
    }

    // Update is called once per frame
    void Update()
    {
    }
    private void FixedUpdate() //envoyer la physique
    {

        if (GM.sendPackets && socket.connected)
        {
            if (GM.Playing && frameCounter % 5 ==0)
            {
                UpdateTrajectory();
            }
            if (!GM.Playing && frameCounter % 30 == 0)
            {
                UpdateAllBalls();
            }
        }
        frameCounter++;
        if(frameCounter > 10000)
        {
            frameCounter = 0;
        }
    }

    public void GiveOtherPlayerControl()
    {
        socket.emit("giveControl", "wasd");
    }

    public void SendBallData(BallData data)
    {
        socket.emit("updateBalls", JsonUtility.ToJson(data));
     //   print("sent data: " + JsonUtility.ToJson(data));
    }

    public void EmitMessage(string message)
    {
        socket.emit("sendMessage", message);
    }
    public void UpdateAllBalls()
    {
        List<GameObject> balls = GM.BallList;

        var pos = new Position();

        pos.data = new float[balls.Count * 6];
        int ball = 0;
        for (int i = 0; i < pos.data.Length; i += 6)
        {
            Vector3 position = balls[ball].transform.position;
            Vector3 vel = balls[ball].GetComponent<Rigidbody>().velocity;
    
            pos.data[i] = position.x;
            pos.data[i + 1] = position.y;
            pos.data[i + 2] = position.z;
            pos.data[i + 3] = vel.x;
            pos.data[i + 4] = vel.y;
            pos.data[i + 5] = vel.z;

            ball++;
        }
        // print(JsonUtility.ToJson(pos));

        socket.emit("sendPos", JsonUtility.ToJson(pos));
        //    print("sent position: " + JsonUtility.ToJson(pos));
    }

    public void UpdateTrajectory()
    {
        var traj = new Trajectory();

        traj.position = GM.Trajectory.transform.position;
        traj.scale = GM.Trajectory.transform.localScale;
        traj.texture = GM.Trajectory.GetComponent<MeshRenderer>().material.mainTextureScale;
        traj.rotation = GM.Trajectory.transform.rotation;

        socket.emit("updateTrajectory", JsonUtility.ToJson(traj));
    }


    [System.Serializable]
    public class Position
    {
        public float[] data;
    }

    [System.Serializable]
    public class Trajectory
    {
       public Vector3 position;
        public Vector3 scale;
        public Vector2 texture;
        public Quaternion rotation;
    }

}
