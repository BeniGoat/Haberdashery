using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int numPlayers;
    public int numRoundsToWin = 3;                              // The number of rounds a single player has to win to win the game.
    public float startDelay = 3f;                               // The delay between the start of RoundStarting and RoundPlaying phases.
    public float endDelay = 3f;                                 // The delay between the end of RoundPlaying and RoundEnding phases.
    private PlayerManager roundWinner;                          // Reference to the winner of the current round.  Used to make an announcement of who won.
    private PlayerManager gameWinner;                           // Reference to the winner of the game.  Used to make an announcement of who won.
    private WaitForSeconds startWait;                           // Used to have a delay whilst the round starts.
    private WaitForSeconds endWait;                             // Used to have a delay whilst the round or game ends.
    public AudioClip musicClip, spawnClip, victoryClip;
    private AudioSource musicSource;
    public CameraScriptOrtho cameraControl;                     // Reference to the CameraScriptOrtho script for control during different phases.
    public GameObject boxPrefab, needlePrefab, playerPrefab;    // Reference to the prefab the players will control.
    public PlayerManager[] characters;                          // A collection of managers for enabling and disabling different aspects of the characters.
    public Text messageText, pausedText, continueText;          // Reference to the overlay Text to display winning text, etc.
    public Text[] scoresText;
    private Color[] scoreColour;
    public Transform[] boxPositions, spawnPoints;               // The position and direction objects will have when they spawn.
    [HideInInspector]
    public List<GameObject> boxPool, needlePool;
    public bool willGrow = false;
    private int roundNumber;                                    // Which round the game is currently on.
    public int boxAmount = 10, needleAmount = 20;
    public GameObject Exit;
    MenuManager menuManager;
    public bool isPaused = false, roundActive = false;

    void Awake()
    {
        if (instance == null)
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

        musicSource = AddAudio(musicClip, true, true, 0.4f);
        musicSource.Play();
    }

    private void Start()
    {
        // Create the delays so they only have to be made once.
        startWait = new WaitForSeconds(startDelay);
        endWait = new WaitForSeconds(endDelay);

        boxPool = new List<GameObject>();
        needlePool = new List<GameObject>();

        numPlayers = PlayerCount.numberOfPlayers;

        SpawnAllPlayers();

        ObjectPooler(boxPrefab, boxPool, boxAmount);
        ObjectPooler(needlePrefab, needlePool, needleAmount);
        SetCameraTargets();

        menuManager = GetComponent<MenuManager>();
        pausedText.enabled = false;
        continueText.enabled = false;
        Exit.SetActive(false);

        // Once the characters have been created, start the game.
        StartCoroutine(GameLoop());
    }

    private void Update()
    {
        PauseManager();
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

    private void PauseManager()
    {
        if (Input.GetButtonDown("PS4Pause"))
        {
            if (!isPaused)
            {
                isPaused = true;
                Time.timeScale = 0;

                AudioListener.volume = 0;
                DisablePlayerControl();

                Exit.SetActive(true);
                messageText.enabled = false;
                pausedText.enabled = true;
                continueText.enabled = true;
            }
            else
            {
                isPaused = false;
                Time.timeScale = 1;

                if (PlayerCount.soundOn)
                    AudioListener.volume = 1;

                if (roundActive && Time.timeScale == 1)
                    EnablePlayerControl();

                Exit.SetActive(false);
                messageText.enabled = true;
                pausedText.enabled = false;
                continueText.enabled = false;
            }
        }
    }

    public void ExitOn()
    {
        Application.Quit();
    }

    private void SpawnAllPlayers()
    {
        // For all the characters...
        for (int i = 0; i < numPlayers; i++)
        {
            // ... create them, set their player number and references needed for control.
            characters[i].instance = Instantiate(playerPrefab, spawnPoints[i].position, spawnPoints[i].rotation) as GameObject;
            characters[i].playerNumber = i + 1;
            characters[i].Setup();
        }
    }

    // This function is used to turn all the players back on and reset their positions and properties.
    private void ResetAllPlayers()
    {
        for (int i = 0; i < numPlayers; i++)
        {
            characters[i].Reset();
            characters[i].instance.transform.position = spawnPoints[i].position;
            characters[i].instance.transform.rotation = spawnPoints[i].rotation;
        }
    }

    private void EnablePlayerControl()
    {
        for (int i = 0; i < numPlayers; i++)
        {
            characters[i].EnableControl();
        }
    }

    private void DisablePlayerControl()
    {
        for (int i = 0; i < numPlayers; i++)
        {
            characters[i].DisableControl();
        }
    }

    private void ResetBoxes()
    {
        for (int i = 0; i < boxPool.Count; i++)
        {
            boxPool[i].transform.position = boxPositions[i].position;
            boxPool[i].transform.rotation = boxPositions[i].rotation;
            boxPool[i].SetActive(false);
            boxPool[i].SetActive(true);
            boxPool[i].GetComponent<Collider>().enabled = true;
        }
        if (GameObject.FindGameObjectsWithTag("PowerUp") != null)
        {
            foreach (GameObject powerUp in GameObject.FindGameObjectsWithTag("PowerUp"))
            {
                Destroy(powerUp);
            }
        }
    }

    private void ResetNeedles()
    {
        for (int i = 0; i < needlePool.Count; i++)
        {
            needlePool[i].SetActive(false);
            needlePool[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
            needlePool[i].GetComponent<Rigidbody>().isKinematic = false;
            needlePool[i].transform.parent = null;
        }
    }

    private void ObjectPooler(GameObject prefab, List<GameObject> objectPool, int amount)
    {
        // create object pool for objects
        for (int i = 0; i < amount; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            objectPool.Add(obj);
        }
    }

    public GameObject GetPooledNeedle()
    {
        for (int i = 0; i < needlePool.Count; i++)
        {
            if (!needlePool[i].activeInHierarchy)
            {
                needlePool[i].SetActive(true);
                return needlePool[i];
            }
        }
        if (willGrow)
        {
            GameObject obj = Instantiate(needlePrefab);
            obj.SetActive(true);
            needlePool.Add(obj);
            return obj;
        }
        return null;
    }

    private void SetCameraTargets()
    {
        // Create a collection of transforms the same size as the number of players.
        Transform[] targets = new Transform[numPlayers];

        // For each of these transforms...
        for (int i = 0; i < targets.Length; i++)
        {
            // ... set it to the appropriate player transform.
            targets[i] = characters[i].instance.transform;
        }

        // These are the targets the camera should follow.
        cameraControl.players = targets;
    }

    // This is called from start and will run each phase of the game one after another.
    private IEnumerator GameLoop()
    {
        // Start off by running the 'RoundStarting' coroutine but don't return until it's finished.
        yield return StartCoroutine(RoundStarting());

        // Once the 'RoundStarting' coroutine is finished, run the 'RoundPlaying' coroutine but don't return until it's finished.
        yield return StartCoroutine(RoundPlaying());

        // Once execution has returned here, run the 'RoundEnding' coroutine, again don't return until it's finished.
        yield return StartCoroutine(RoundEnding());

        // This code is not run until 'RoundEnding' has finished.  At which point, check if a game winner has been found.
        if (gameWinner != null)
        {
            // If there is a game winner, return to main menu.
            PlayerCount.soundOn = true;
            SceneManager.LoadScene(0);
            Destroy(gameObject);
        }
        else
        {
            // If there isn't a winner yet, restart this coroutine so the loop continues.
            // Note that this coroutine doesn't yield.  This means that the current version of the GameLoop will end.
            StartCoroutine(GameLoop());
        }
    }

    private IEnumerator RoundStarting()
    {
        // As soon as the round starts reset the players and make sure they can't move.
        ResetAllPlayers();
        DisablePlayerControl();
        ResetBoxes();
        ResetNeedles();
        ScoreCounter();

        musicSource.enabled = true;
        SoundManager.instance.PlaySingle(0.8f, spawnClip);

        // Snap the camera's zoom and position to something appropriate for the reset players.
        cameraControl.SetStartPositionAndSize();

        // Display pre-round text.
        messageText.text = "prepare to fight";

        // Wait for the specified length of time until yielding control back to the game loop.
        yield return startWait;
    }

    private IEnumerator RoundPlaying()
    {
        roundActive = true;
        // As soon as the round begins playing let the players control the players.
        EnablePlayerControl();

        // Clear the text from the screen.
        messageText.text = string.Empty;

        // While there is not one players left...
        while (!OnePlayerLeft())
        {
            // ... return on the next frame.
            yield return null;
        }
    }

    private IEnumerator RoundEnding()
    {
        musicSource.enabled = false;
        SoundManager.instance.PlaySingle(1f, victoryClip);
        roundActive = false;
        // Stop players from moving.
        DisablePlayerControl();

        // Clear the winner from the previous round.
        roundWinner = null;

        // See if there is a winner now the round is over.
        roundWinner = GetRoundWinner();

        // If there is a winner, increment their score.
        if (roundWinner != null)
            roundWinner.wins++;
        ScoreCounter();
        // Now the winner's score has been incremented, see if someone has one the game.
        gameWinner = GetGameWinner();

        // Get a message based on the scores and whether or not there is a game winner and display it.
        string message = EndMessage();
        messageText.text = message;

        // Wait for the specified length of time until yielding control back to the game loop.
        yield return endWait;
    }

    // This is used to check if there is one or fewer players remaining and thus the round should end.
    private bool OnePlayerLeft()
    {
        // Start the count of players left at zero.
        int numPlayersLeft = 0;

        // Go through all the players...
        for (int i = 0; i < numPlayers; i++)
        {
            // ... and if they are active, increment the counter.
            if (characters[i].instance.activeSelf)
                numPlayersLeft++;
        }
        // If there are one or fewer players remaining return true, otherwise return false.
        return numPlayersLeft <= 1;
    }

    // This function is to find out if there is a winner of the round.
    // This function is called with the assumption that 1 or fewer players are currently active.
    private PlayerManager GetRoundWinner()
    {
        // Go through all the players...
        for (int i = 0; i < characters.Length; i++)
        {
            // ... and if one of them is active, it is the winner so return it.
            if (characters[i].instance.activeSelf)
                return characters[i];
        }

        // If none of the players are active it is a draw so return null.
        return null;
    }

    // This function is to find out if there is a winner of the game.
    private PlayerManager GetGameWinner()
    {
        
        // Go through all the players...
        for (int i = 0; i < numPlayers; i++)
        {
            // ... and if one of them has enough rounds to win the game, return it.
            if (characters[i].wins == numRoundsToWin)
                return characters[i];
        }

        // If no players have enough rounds to win, return null.
        return null;
    }

    private void ScoreCounter()
    {
        for (int i = 0; i < numPlayers; i++)
        {
            scoresText[i].color = characters[i].playerColor;
            scoresText[i].text = characters[i].wins.ToString();
        }
    }

    // Returns a string message to display at the end of each round.
    private string EndMessage()
    {
        // By default when a round ends there are no winners so the default end message is a draw.
        string message = "draw!";

        // If there is a winner then change the message to reflect that.
        if (roundWinner != null)
            message = roundWinner.coloredPlayerText + " wins the round!";

        // Add some line breaks after the initial message.
        message += "\n\n";

        // If there is a game winner, change the entire message to reflect that.
        if (gameWinner != null)
            message = gameWinner.coloredPlayerText + " wins!";

        return message;
    }
}
