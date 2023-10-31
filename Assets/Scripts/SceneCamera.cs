using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneCamera : MonoBehaviour
{
    [SerializeField] SceneCamera point1 = null;
    [SerializeField] SceneCamera point2 = null;
    [SerializeField] SceneCamera point3 = null;

    [SerializeField] GameObject mainBall;

    List<GameObject> balls;

    [SerializeField] float velocityFactor = 1f; //determine si une balle qui bouge vaut plus que deux balles qui bougent moins
    [SerializeField] float yVelocityFactor = 5f; //favorise une balle qui tombe
    [SerializeField] float viewDistanceFactor = 5f; //favorise une position de camera plus proche de laction

    int nbBalles;

    public float Priority
    {
        get { return TotalVelocity; }
    }
    public float TotalVelocity
    {
        get
        {
            float totalVelocity = 0;
            foreach (GameObject ball in balls)
            {
                if (ball.activeSelf)
                {
                    Rigidbody rb = ball.GetComponent<Rigidbody>();
                    totalVelocity += rb.velocity.magnitude + rb.angularVelocity.magnitude + Mathf.Abs(rb.velocity.y) * yVelocityFactor;
                }
            }
            return totalVelocity;
        }
    }
    public GameObject BestBall
    {
        get {
            GameObject bestBall = mainBall;
            float bestVelocity = -999f;
            foreach (GameObject ball in balls)
            {
                if (ball.activeSelf)
                {
                    Rigidbody rb = ball.GetComponent<Rigidbody>();
                    float vel = rb.velocity.magnitude + rb.angularVelocity.magnitude + Mathf.Abs(rb.velocity.y) * yVelocityFactor;
                    if (bestVelocity < vel)
                    {
                        bestVelocity = vel;
                        bestBall = ball;
                    }
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
        points = new List<SceneCamera>();
        nbBalles = 0;
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
     public (Vector3 pos,float priority) GetBestCamera()
      {
        float priority = -999;
        Vector3 pos = transform.position + Vector3.up*4;
        foreach(var el in points)
        {
            if (priority<el.Priority)
            {
                priority = Priority;
            }
        }
        return (pos, priority);
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
