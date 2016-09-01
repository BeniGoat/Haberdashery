using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {

    public Canvas BackgroundCanvas,
                  ControlsCanvas,
                  MainCanvas,
                  ModesCanvas, 
                  OptionsCanvas;

    public Button ButtonSound;
    public Button ButtonControls;
    public Button ButtonStart;
    public Button ButtonBack;
    public Button ButtonDuel;

    public EventSystem eventSystem;

    public bool soundOn;
    
    public AudioClip musicClip, clickClip;
    private AudioSource musicSource, clickSource;

    void Awake()
    {
        ControlsCanvas.enabled = false;
        OptionsCanvas.enabled = false;
        ModesCanvas.enabled = false;
        AudioListener.volume = 1;
        musicSource = AddAudio(musicClip, true, true, 0.5f);
        clickSource = AddAudio(clickClip, false, false, 0.5f);
        musicSource.Play();
        soundOn = true;

        eventSystem.SetSelectedGameObject(ButtonStart.gameObject);
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

    public void StartOn()
    {
        clickSource.Play();
        ModesCanvas.enabled = true;
        MainCanvas.enabled = false;
        eventSystem.SetSelectedGameObject(ButtonDuel.gameObject);
    }

    public void OptionsOn()
    {
        clickSource.Play();
        OptionsCanvas.enabled = true;
        MainCanvas.enabled = false;
        eventSystem.SetSelectedGameObject(ButtonControls.gameObject);
    }

    public void ControlsOn()
    {
        clickSource.Play();
        OptionsCanvas.enabled = false;
        BackgroundCanvas.enabled = false;
        ControlsCanvas.enabled = true;
        eventSystem.SetSelectedGameObject(ButtonBack.gameObject);
    }

    public void SoundOn()
    {
        if (soundOn)
        {
            ButtonSound.GetComponent<Text>().text = "sound off";
            AudioListener.volume = 0;
            soundOn = false;
            PlayerCount.soundOn = false;
        }
        else
        {
            ButtonSound.GetComponent<Text>().text = "sound on";
            AudioListener.volume = 1;
            soundOn = true;
            PlayerCount.soundOn = true;
        }
    }

    public void BackOn()
    {
        clickSource.Play();
        BackgroundCanvas.enabled = true;
        ControlsCanvas.enabled = false;
        OptionsCanvas.enabled = false;
        MainCanvas.enabled = true;
        eventSystem.SetSelectedGameObject(ButtonStart.gameObject);
    }

    public void DuelOn()
    {
        PlayerCount.numberOfPlayers = 2;
        SceneManager.LoadScene(1);
    }

    public void TripleOn()
    {
        PlayerCount.numberOfPlayers = 3;
        SceneManager.LoadScene(1);
    }

    public void FourwayOn()
    {
        PlayerCount.numberOfPlayers = 4;
        SceneManager.LoadScene(1);
    }

    public void ExitOn()
    {
        clickSource.Play();
        Application.Quit();
    }
}
