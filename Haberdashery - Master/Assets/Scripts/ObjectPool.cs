using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    public GameObject box, needle;
    public int boxAmount = 10, needleAmount = 20;
    public Transform[] boxPositions;
    List<GameObject> boxes, needles;

    void Awake()
    {
        boxes = new List<GameObject>();
        needles = new List<GameObject>();
    }

    void Start()
    {
        // create object pool for boxes
        for (int i = 0; i < boxAmount; i++)
        {
            GameObject obj = Instantiate(box);
            obj.SetActive(false);
            boxes.Add(obj);
        }

        // create object pool for needles
        for (int i = 0; i < needleAmount; i++)
        {
            GameObject obj = Instantiate(needle);
            obj.SetActive(false);
            needles.Add(obj);
        }

        BoxSpawner();
    }

    void BoxSpawner()
    {
        for (int i = 0; i < boxes.Count; i++)
        {
            if (!boxes[i].activeInHierarchy)
            {
                boxes[i].transform.position = boxPositions[i].position;
                boxes[i].transform.rotation = boxPositions[i].rotation;
                boxes[i].SetActive(true);
            }
        }
    }
}
