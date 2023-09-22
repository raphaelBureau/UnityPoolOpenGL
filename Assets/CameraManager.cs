using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // Start is called before the first frame update
    SceneCamera[] cams;
    [SerializeField] Camera cam;

    public Camera Cam
    {
        get { return cam; }
    }

    public bool Active = true; //daddy gameManager controle ca
    void Start()
    {
        cams = transform.GetComponentsInChildren<SceneCamera>();
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
            positions.Sort((el1, el2) => el1.priority.CompareTo(el2.priority));

            cam.transform.position = cam.transform.position + ((positions[0].movement.pos + new Vector3(0, 6, 0)) - cam.transform.position) * Time.deltaTime;

            var targetRotation = Quaternion.LookRotation(positions[0].movement.look.transform.position - cam.transform.position);
            cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, targetRotation, 2 * Time.deltaTime);

            //cam.transform.LookAt(positions[0].movement.look.transform.position);
        }
    }
}
