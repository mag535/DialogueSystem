using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterID
{
    PLAYER,
    NPC1,
    NPC2
}

[CreateAssetMenu(menuName = "Dialogue System / Line Data")]
public class DialogueLineData : ScriptableObject
{
    public string text;

    public DialogueLineData[] responses;
    public int karmaScore;

    public AudioClip dialogueAudio;
    public CharacterID character;
}
