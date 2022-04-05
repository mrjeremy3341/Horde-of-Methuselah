using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource source;
    public AudioClip[] songs;
    public AudioClip ambience;

    Queue<AudioClip> songQueue = new Queue<AudioClip>();
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        foreach(AudioClip song in songs)
        {
            songQueue.Enqueue(song);
        }

        if (!source.isPlaying)
        {
            songQueue.Enqueue(source.clip);
            source.clip = songQueue.Dequeue();
            source.Play();
        }

    }

    public void StartMusic()
    {
        if(source.clip == ambience || source.clip == null)
        {
            source.clip = songQueue.Dequeue();
            source.Play();
            source.loop = false;
        }
        else
        {
            source.Play();
        }
    }

    public void StopMusic()
    {
        source.Pause();
    }

    public void StartAmbience()
    {
        if (source.clip != null)
        {
            songQueue.Enqueue(source.clip);
        }

        source.clip = ambience;
        source.Play();
        source.loop = true;
    }
}
