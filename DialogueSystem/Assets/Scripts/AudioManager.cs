using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : Singleton<DialogueManager>
{
    // Start is called before the first frame update
    void Start()
    {
        EvtSystem.EventDispatcher.AddListener<PlayAudio>(PlayDialogue);
    }

    private void PlayDialogue(PlayAudio eventData)
    {
        AudioSource dialogueAudio = gameObject.AddComponent<AudioSource>();
        dialogueAudio.clip = eventData.clipToPlay;

        // play audio message
        dialogueAudio.Play();
    }
}
