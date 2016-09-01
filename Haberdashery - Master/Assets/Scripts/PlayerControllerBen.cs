using UnityEngine;
using UnityEngine.UI;

public class PlayerControllerBen : MonoBehaviour
{
    ArrowManager arrowManager;
    GameObject needleClone;
    Quaternion upright, originalHandRot, desiredHandRot;
    Rigidbody player;
    Vector3 lowerLegPos1, lowerLegPos2, originalHandShield, originalHandNeedle, hitDir, hitVel;

    private PlayerHealthAmmo playerHealthAmmo;

    public AudioClip throwClip, walkClip;
    private AudioSource throwSound, walkSound;
    public GameObject Body, Leg1, Leg2, HandNeedle, HandShield, Shield, Helm;
    public Transform needleSpawn;
    [HideInInspector]
    public bool shieldActive, helmActive;
    private bool fired, isHit;
    private string fireButton, shieldButton, nameXAxis, nameZAxis, PS4Hor, PS4Vert, PS4Fire, PS4Shield;
    [HideInInspector]
    public int playerNumber = 1;
    [HideInInspector]
    public float currShootForce, helmTimer, shieldTimer, powerUpTimer, speed, PS4MoveHor, PS4MoveVert;
    float motionLeg1, motionLeg2, motionHand1, motionHand2;
    float chargeSpeed = 100f,
          hitTime = 1f,
          minShootForce = 750f,
          maxShootForce = 1000f;

    void Awake()
    {
        player = GetComponent<Rigidbody>();
        // Get rest positions of player body
        lowerLegPos1 = Leg1.transform.localPosition;
        lowerLegPos2 = Leg2.transform.localPosition;
        originalHandShield = HandShield.transform.localPosition;
        originalHandNeedle = HandNeedle.transform.localPosition;
        originalHandRot = HandShield.transform.localRotation;
        upright = Body.transform.localRotation;

        throwSound = AddAudio(throwClip, false, false, 0.75f);
        walkSound = AddAudio(walkClip, true, false, 0.2f);
    }

    public AudioSource AddAudio(AudioClip clip, bool loop, bool playAwake, float vol)
    {
        AudioSource newAudio = gameObject.AddComponent<AudioSource>();
        newAudio.clip = clip;
        newAudio.loop = loop;
        newAudio.playOnAwake = playAwake;
        newAudio.volume = vol;
        return newAudio;
    }

    private void OnEnable()
    {
        // When the player is spawned, make sure it's not kinematic.
        player.isKinematic = false;
        // Set the power-up flags to inactive
        helmActive = false;
        shieldActive = false;
        // Set the collision flag to false
        isHit = false;

        playerHealthAmmo = GetComponent<PlayerHealthAmmo>();
    }

    private void OnDisable()
    {
        // When the player is turned off, set it to kinematic so it stops moving.
        player.isKinematic = true;
    }

    private void Start()
    {
        // Get up/down/left/right input axes based on the player number
        PS4Hor = "PS4Hor" + playerNumber;
        PS4Vert = "PS4Vert" + playerNumber;
        // Get fire/shield input axes based on the player number
        PS4Fire = "PS4Fire" + playerNumber;
        PS4Shield = "PS4Shield" + playerNumber;
    }

    void Update()
    {
        PlayerMover();
        PlayerWalker();
        PowerUpManager(powerUpTimer);
        ThrowManager();
        ShieldMover();
    }

    // If player has sustained a needle hit, gather ballistic information on needle
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Arrow")
        {
            arrowManager = other.GetComponent<ArrowManager>();

            if (!arrowManager.collectable)
            {
                isHit = true;
                hitDir = arrowManager.hitDir;
                hitVel = arrowManager.hitVel;
            }
        }
    }

    void PlayerMover()
    {
        // controller inputs
        PS4MoveHor = Input.GetAxis(PS4Hor);
        PS4MoveVert = Input.GetAxis(PS4Vert);
        Vector3 PS4movement = new Vector3(PS4MoveHor, 0.0f, PS4MoveVert);

        // If not at rest, player faces the direction of movement
        if (PS4movement.sqrMagnitude >= 0.05f)
            transform.forward = PS4movement;

        // Player moves in direction at speed
        if (PS4movement.magnitude > 1f)
            player.velocity = PS4movement.normalized * speed;
        else
            player.velocity = PS4movement * speed;

        // If player is hit by needle, moves in direction of impact
        if (isHit && hitTime > 0)
        {
            player.AddForce(hitDir * hitVel.magnitude * hitTime * 5f);
            hitTime -= Time.deltaTime;
        }
        else
        {
            isHit = false;
            hitTime = 1f;
        }

    }

    public void ThrowManager()
    {
        if (playerHealthAmmo.currentAmmo > 0)
        {
            // If the max force has been exceeded and the arrow hasn't yet been thrown..
            if (currShootForce >= maxShootForce && !fired)
            {
                // ...use the max force and shoot the arrow.
                currShootForce = maxShootForce;
                FireNeedle();
            }
            // Otherwise, if the fire button has just started being pressed...
            else if (Input.GetButtonDown(PS4Fire))
            {
                // ...reset the fired flag and reset the shoot force.
                fired = false;
                currShootForce = minShootForce;
            }
            // Otherwise, if the fire button is being held and the arrow hasn't been thrown yet...
            else if (Input.GetButton(PS4Fire) && !fired)
            {
                // Increment the shoot force
                currShootForce += chargeSpeed * Time.deltaTime;
            }
            // Otherwise, if the fire button is released and the arrow hasn't been thrown yet...
            else if (Input.GetButtonUp(PS4Fire) && !fired)
            {
                // ...throw the arrow
                FireNeedle();
            }
        }
    }

    void FireNeedle()
    {
        throwSound.Play();
        // Set the fired flag so ShootArrow is only called once.
        fired = true;
        // Get a needle from the needle object pool
        needleClone = GameManager.instance.GetPooledNeedle();
        if (needleClone == null) return;
        needleClone.transform.position = needleSpawn.position;
        needleClone.transform.rotation = needleSpawn.rotation;
        needleClone.SetActive(true);
        // Apply throwing force to the arrow in the player's forward direction
        needleClone.GetComponent<Rigidbody>().AddForce(currShootForce * transform.forward);
        // Register the player who threw the needle
        needleClone.GetComponent<ArrowManager>().shooter = gameObject;
        if (needleClone.GetComponent<ArrowManager>().shooter != null)
            needleClone.GetComponent<ArrowManager>().shooter = gameObject;
        // Reset the throw force
        currShootForce = minShootForce;
        // Reduce current ammo by 1
        playerHealthAmmo.currentAmmo--;
    }

    // Walking Movement
    void PlayerWalker()
    {
        motionLeg1 = lowerLegPos1.y + Mathf.PingPong(Time.time * 3f, 0.5f);
        motionLeg2 = (lowerLegPos2.y + 0.4f) - Mathf.PingPong(Time.time * 3f, 0.5f);
        motionHand1 = 0.5f - Mathf.PingPong(Time.time * 2, 1f);
        motionHand2 = Mathf.PingPong(Time.time * 2, 1f) - 0.5f;

        float t = Time.time % (2 * Mathf.PI);
        float oscSpeed = ((t + Time.deltaTime) % (2 * Mathf.PI)) * 5;

        Body.transform.localPosition = new Vector3(Body.transform.localPosition.x, 0.5f + (Mathf.Sin(oscSpeed) * 0.05f), Body.transform.localPosition.z);

        // If the player is moving...
        if (player.velocity != Vector3.zero)
        {
            // angle body towards motion
            Body.transform.localRotation = Quaternion.AngleAxis(10, Vector3.right);
            // move hands back and forth            
            HandNeedle.transform.localPosition = new Vector3(HandNeedle.transform.localPosition.x, HandNeedle.transform.localPosition.y, motionHand1);
            HandShield.transform.localPosition = new Vector3(HandShield.transform.localPosition.x, HandShield.transform.localPosition.y, motionHand2);
            // move legs up and down
            Leg1.transform.localPosition = new Vector3(Leg1.transform.localPosition.x, motionLeg1, Leg1.transform.localPosition.z);
            Leg2.transform.localPosition = new Vector3(Leg2.transform.localPosition.x, motionLeg2, Leg2.transform.localPosition.z);

            if (!walkSound.isPlaying)
                walkSound.Play();
            else
                walkSound.pitch = Random.Range(0.8f, 1.2f);
        }
        // if the player isn't moving...
        else
        {
            // return body and limbs to default positions
            Body.transform.localRotation = upright;
            HandNeedle.transform.localPosition = originalHandNeedle;
            Leg1.transform.localPosition = lowerLegPos1;
            Leg2.transform.localPosition = lowerLegPos2;

            walkSound.Pause();
        }
    }

    // Shield Movement
    void ShieldMover()
    {
        // If the shield power-up is active...
        if (shieldActive)
        {
            //...and the shield button is being pressed...
            if (Input.GetButton(PS4Shield))
            {
                //...scan the area around the player for colliders...
                foreach (Collider hit in Physics.OverlapSphere(transform.position, 5f))
                {
                    //...ignore everthing that isn't a needle...
                    if (hit.tag == "Arrow" && hit.transform.parent == null)
                    {
                        //...get the direction from the player to the collider...
                        Vector3 direction = (hit.transform.position - transform.position).normalized;
                        //...and move the shield towards the collider.
                        HandShield.transform.LookAt(hit.transform.position);
                        HandShield.transform.position = transform.position + direction;
                    }
                }
            }
            // If the shield button isn't being pressed but the shield is active...
            else if (HandShield.transform.localPosition != originalHandShield)
            {
                //...return the shield to it's original position
                HandShield.transform.localPosition = originalHandShield;
                HandShield.transform.localRotation = originalHandRot;
            }
        }
        // If the shield power-up is not active but the shield button is still being held down
        else if (Input.GetButton(PS4Shield))
        {
            //...return the shield to its original position
            HandShield.transform.localPosition = originalHandShield;
            HandShield.transform.localRotation = originalHandRot;
        }
    }

    // Power Up Manager
    public void PowerUpManager(float timer)
    {
        if (helmActive)
        {
            Helm.SetActive(true);
            helmTimer -= Time.deltaTime;

            if (helmTimer < 0)
            {
                helmActive = false;
                helmTimer = timer;
            }
        }
        else
            Helm.SetActive(false);

        if (shieldActive)
        {
            Shield.SetActive(true);
            shieldTimer -= Time.deltaTime;

            if (shieldTimer < 0)
            {
                shieldActive = false;
                shieldTimer = timer;
            }
        }
        else
            Shield.SetActive(false);
    }
}
