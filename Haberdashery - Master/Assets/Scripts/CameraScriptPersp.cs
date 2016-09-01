using UnityEngine;
using System.Collections;

public class CameraScriptPersp : MonoBehaviour
{
    public Transform Player1,Player2;
    public float dampTime = 0.2f;
    public float screenEdgeBuffer = 5f;
    RaycastHit hitInfo1, hitInfo2;
    Vector3 camDir1, camDir2;
    GameObject disabledObject1, disabledObject2;
    private Camera mainCam;
    private float desiredSizeFOV, zoomSpeed;
    private float minSizeFOV = 30f, maxSizeFOV = 50f;

    void Awake()
    {
        mainCam = GetComponentInChildren<Camera>();
        SetStartPositionAndSize();
    }

    void FixedUpdate()
    {
        FindCameraPosition();

        camDir1 = mainCam.transform.position - Player1.transform.position;
        camDir2 = mainCam.transform.position - Player2.transform.position;
        if (Physics.Raycast(Player1.transform.position, camDir1, out hitInfo1))  // TODO: fix so that the player's shield is ignored
        {
            GameObject obstacle = hitInfo1.collider.gameObject;
            Color color = obstacle.GetComponent<Renderer>().material.color;
            obstacle.GetComponent<Renderer>().material.color = new Color(color.r, color.g, color.b, 0.5f);
            disabledObject1 = obstacle;
        }
        else
        {
            //reenable all object renderers that have been disabled
            if (disabledObject1 != null)
            {
                Color color1 = disabledObject1.GetComponent<Renderer>().material.color;
                disabledObject1.GetComponent<Renderer>().material.color = new Color(color1.r, color1.g, color1.b, 1f);
            }
        }
        if (Physics.Raycast(Player2.transform.position, camDir2, out hitInfo2))  // TODO: fix so that the player's shield is ignored
        {
            GameObject obstacle = hitInfo2.collider.gameObject;
            Color color = obstacle.GetComponent<Renderer>().material.color;
            obstacle.GetComponent<Renderer>().material.color = new Color(color.r, color.g, color.b, 0.5f);
            disabledObject2 = obstacle;
        }
        else
        {
            //reenable all object renderers that have been disabled
            if (disabledObject2 != null)
            {
                Color color2 = disabledObject2.GetComponent<Renderer>().material.color;
                disabledObject2.GetComponent<Renderer>().material.color = new Color(color2.r, color2.g, color2.b, 1f);
            }
        }
    }

    void FindCameraPosition()
    {
        Vector3 midPoint = new Vector3((Player1.position.x + Player2.position.x) * 0.5f, 0.0f, (Player1.position.z + Player2.position.z) * 0.5f);
        Vector3 seperation = Player1.position - Player2.position;

        desiredSizeFOV = seperation.magnitude * 2.3f;
        if (desiredSizeFOV <= minSizeFOV)
        {
            desiredSizeFOV = minSizeFOV;
        }
        else if (desiredSizeFOV >= maxSizeFOV)
        {
            desiredSizeFOV = maxSizeFOV;
        }
        mainCam.fieldOfView = Mathf.SmoothDamp(mainCam.fieldOfView, desiredSizeFOV + screenEdgeBuffer, ref zoomSpeed, dampTime);
        mainCam.transform.position = new Vector3(midPoint.x, mainCam.transform.position.y, midPoint.z - 15f);

        mainCam.transform.LookAt(midPoint);
    }

    public void SetStartPositionAndSize()
    {
        mainCam.transform.LookAt(Vector3.zero);
        mainCam.fieldOfView = 40f;
    }

}
