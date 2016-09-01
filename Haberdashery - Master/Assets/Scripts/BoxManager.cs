using UnityEngine;
using System.Collections;

public class BoxManager : MonoBehaviour
{
    [HideInInspector]
    public bool isDestroyed;
    public AudioClip[] boxDestroySound;
    public GameObject Heart, Helm, Shield;

    private GameObject PowerUp;
    private Vector3 itemSpawnPoint;

    private float currentIntegrity;
    private int typeOfPU;
    private bool isRunning = false;

    float waitTime = 5f,
        startingIntegrity = 100f,
        hazardSpeed = 800f;

    void OnEnable()
    {
        // And reset the box integrity and whether or not it's destroyed
        currentIntegrity = startingIntegrity;
        isDestroyed = false;
        itemSpawnPoint = new Vector3(transform.position.x, 2f, transform.position.z);
    }

    // cycle through boxes randomly
    void Update()
    {
        if (!isRunning)
            StartCoroutine(ChangeBoxContents(waitTime));
    }

    public void BoxDamage(float amount)
    {
        // Reduce current integrity by the amount of damage done
        currentIntegrity -= amount;
        // If the current integrity is at or below zero and it has not yet been registered, change destroyed flag
        if (currentIntegrity <= 0f && !isDestroyed)
        {
            SoundManager.instance.RandomizeSfx(1f, boxDestroySound).Play();
            isDestroyed = true;
            BoxDeath();
            gameObject.SetActive(false);
        }
    }

    public void BoxDeath()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject needleClone = GameManager.instance.GetPooledNeedle();
            if (needleClone == null) return;
            needleClone.transform.position = itemSpawnPoint;
            needleClone.transform.rotation = transform.rotation;
            needleClone.GetComponent<Rigidbody>().AddForce(hazardSpeed * needleClone.transform.forward);
            transform.Rotate(0, 90, 0, Space.Self);
        }

        if(PowerUp != null)
            Instantiate(PowerUp, itemSpawnPoint, transform.rotation);        
    }

    // Function to ignore collisions between box and shield colliders
    void OnCollisionEnter(Collision col)
    {
        Transform[] allChildren = col.gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child.name == "Shield" && child.gameObject.activeInHierarchy)
            {
                Physics.IgnoreCollision(child.GetComponentInChildren<Collider>(), gameObject.GetComponent<Collider>());
            }            
        }
    }

    IEnumerator ChangeBoxContents(float timeInSecs)
    {
        isRunning = true;
        typeOfPU = Random.Range(0, 3);
        // randomly assign hazard/power-up flag to currently selected box
        switch (typeOfPU)
        {
            case 0:
                PowerUp = Helm;
                break;
            case 1:
                PowerUp = Heart;
                break;
            case 2:
                PowerUp = Shield;
                break;
            default:
                PowerUp = null;
                break;
        }

        yield return new WaitForSeconds(timeInSecs);

        isRunning = false;
    }
}
