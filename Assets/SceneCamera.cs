using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneCamera : MonoBehaviour
{
    [SerializeField] SceneCamera point1 = null;
    [SerializeField] SceneCamera point2 = null;
    [SerializeField] SceneCamera point3 = null;

    List<GameObject> balls;

    float velocityFactor = 5f; //determine si une balle qui bouge vaut plus que deux balles qui bougent moins

    int nbBalles = 0;

    public float Priority
    {
        get { return nbBalles + TotalVelocity * velocityFactor; }
    }
    public float TotalVelocity
    {
        get
        {
            float totalVelocity = 0;
            foreach (GameObject ball in balls)
            {
                totalVelocity += ball.GetComponent<Rigidbody>().velocity.magnitude;
            }
            return totalVelocity;
        }
    }
    public GameObject BestBall
    {
        get {
            GameObject bestBall = null;
            float bestVelocity = 0f;
            foreach (GameObject ball in balls)
            {
                float vel = ball.GetComponent<Rigidbody>().velocity.magnitude;
                if (bestVelocity < vel)
                {
                    bestVelocity = vel;
                    bestBall = ball;
                }
            }
            return bestBall;
        } //bris dencapsulation mais la camera a besoin de ca
    }
    public Vector3 Position
    {
        get { return transform.position; }
    }

    List<SceneCamera> points;
    // Start is called before the first frame update
    void Start()
    {
        balls = new List<GameObject>();
        points = new();
        if (point1!=null)
        {
            points.Add(point1);
        }
        if (point2 != null)
        {
            points.Add(point2);
        }
        if (point3 != null)
        {
            points.Add(point3);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// retourne la meuilleure position de camera
    /// le int est le nombre de boules en focus
    /// </summary>
    /// <returns></returns>
     public ((Vector3 position,GameObject ball) pos,float priority) GetBestCamera()
      {
        float priority = -1;
        Vector3 pos = Vector3.zero;
        GameObject ball = transform.gameObject;
        if (BestBall != null)
        {
            ball = BestBall;
        }
        foreach(var el in points)
        {
            if(priority<el.Priority)
            {
                priority = el.Priority;
                pos = el.Position;
            }
        }
        return ((pos,ball), priority);
      }
    private void OnTriggerEnter(Collider other)
    {
        balls.Add(other.gameObject);
        nbBalles++;
    }
    private void OnTriggerExit(Collider other)
    {
        balls.Remove(other.gameObject);
        nbBalles--;
    }
}
