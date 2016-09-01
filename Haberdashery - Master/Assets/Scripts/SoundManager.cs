using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

    public AudioSource efxSource;
    public static SoundManager instance = null;
    public float lowPitch   = 0.9f;
    public float highPitch  = 1.1f;

    void Awake()
    {
        if (instance == null)
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a SoundManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySingle(float vol, AudioClip clip)
    {
        float randomPitch = Random.Range(lowPitch, highPitch);

        efxSource.clip = clip;
        efxSource.pitch = randomPitch;
        efxSource.volume = vol;
        efxSource.Play();
    }

    public AudioSource RandomizeSfx(float vol, params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitch, highPitch);

        efxSource.pitch = randomPitch;
        efxSource.clip = clips[randomIndex];
        efxSource.volume = vol;
        return efxSource;
    }
}
