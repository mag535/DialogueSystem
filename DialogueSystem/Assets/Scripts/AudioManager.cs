using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : Singleton<DialogueManager>
{
    public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        EvtSystem.EventDispatcher.AddListener<PlayAudio>(PlayAudioClip);
    }

    private void PlayAudioClip(PlayAudio eventData)
    {
        audioSource.Stop();
        if (audioSource != null)
        {
            audioSource.PlayOneShot(eventData.clipToPlay);
        }
    }
}
