using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KyleDulce.SocketIo;
using static ballScript;

public class Networking : MonoBehaviour
{
    // Start is called before the first frame update
    Socket socket;
    [SerializeField] GameManager GM;

    int frameCounter = 0;

    void Start()
    {
        frameCounter = 0;
        socket = SocketIo.establishSocketConnection("ws://127.0.0.1:3000");
        socket.connect();

        print("waiting for other player");
        socket.emit("joinGame", "pool"); //send connection request

        Time.timeScale = 0;//stop and wait for player 2

        socket.on("gameStart", (first) =>
        {
            print("connected");
            GM.EnableMultiplayer(bool.Parse(first));
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


        socket.on("positions", (res) => //array 6 * length , [x,y,z, velX, velY, velZ]  i+=6;
        {
         //print(res);
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
    }

    // Update is called once per frame
    void Update()
    {
    }
    private void FixedUpdate() //envoyer la physique
    {

        if (GM.sendPackets && socket.connected && frameCounter%20 ==0)
        {
            UpdateAllBalls();
        }
        frameCounter++;
        if(frameCounter > 10000)
        {
            frameCounter = 0;
        }
    }
    [System.Serializable]
    public class Position
    {
        public float[] data;
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
    public void UpdateAllBalls()
    {
        List<GameObject> balls = GM.BallList;

        var pos = new Position();

        pos.data = new float[balls.Count * 6];
        int ball = 0;
        for (int i = 0; i < pos.data.Length; i += 6)
        {
            pos.data[i] = balls[ball].transform.position.x;
            pos.data[i + 1] = balls[ball].transform.position.y;
            pos.data[i + 2] = balls[ball].transform.position.z;
            pos.data[i + 3] = balls[ball].GetComponent<Rigidbody>().velocity.x;
            pos.data[i + 4] = balls[ball].GetComponent<Rigidbody>().velocity.y;
            pos.data[i + 5] = balls[ball].GetComponent<Rigidbody>().velocity.z;

            ball++;
        }
        // print(JsonUtility.ToJson(pos));

        socket.emit("sendPos", JsonUtility.ToJson(pos));
        //    print("sent position: " + JsonUtility.ToJson(pos));
    }

}
