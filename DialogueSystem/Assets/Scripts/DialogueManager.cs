using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    #region SingletonStuff
    static DialogueManager _instance = null;

    public static DialogueManager Instance { get {return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (this == _instance)
        {
            _instance = null;
        }
    }
    #endregion

    public DialogueDatabase database;

    private float dialogueWaitTime = 0.0f;
    private const float kDefaultWaitTime = 2.0f;
    private DialogueLineData currentDialogue = null;
    private float currentTime = 0.0f;

    Dictionary<string, DialogueLineData> dialogueDatabase = new Dictionary<string, DialogueLineData>();

    public void Start()
    {
        LoadDatabase();
    }

    public void LoadDatabase()
    {
        foreach(DialogueLineData line in database.data)
        {
            dialogueDatabase.Add(line.name, line);
        }
    }

    public void StartDialogue(string dialogueName)
    {
        DialogueLineData data = null;

        if(dialogueDatabase.TryGetValue(dialogueName, out data))
        {
            StartDialogue(data);
        }
    }

    public void StartDialogue(DialogueLineData lineToStart)
    {
        //Show Text on Screen
        ShowDialogueText message = new ShowDialogueText();
        message.text = lineToStart.text;
        message.id = lineToStart.character;

        EvtSystem.EventDispatcher.Raise<ShowDialogueText>(message);

        //Play dialogue Audio
        if (lineToStart.dialogueAudio != null)
        {
            PlayAudio audioMessage = new PlayAudio();
            audioMessage.clipToPlay = lineToStart.dialogueAudio;

            EvtSystem.EventDispatcher.Raise<PlayAudio>(audioMessage);

            dialogueWaitTime = lineToStart.dialogueAudio.length;
        }
        else
        {
            dialogueWaitTime = kDefaultWaitTime;
        }
        currentDialogue = lineToStart;
        currentTime = 0.0f;
    }

    private void PlayResponseLine(int currentResponseIndex)
    {
        if (currentDialogue.responses.Length > currentResponseIndex)
        {
            DialogueLineData line = currentDialogue.responses[currentResponseIndex];
            StartDialogue(line);
        }
    }

    private void CreateResponseMessage()
    {
        int numResponses = currentDialogue.responses.Length;
        if (numResponses > 0)
        {
            ShowResponses responseMessage = new ShowResponses();

            responseMessage.responses = new ResponseData[numResponses];

            int index = 0;
            foreach(DialogueLineData response in currentDialogue.responses)
            {
                responseMessage.responses[index].text = response.text;
                responseMessage.responses[index].karmaScore = response.karmaScore;
                int currentIndex = index;
                responseMessage.responses[index].buttonAction = () => { this.PlayResponseLine(currentIndex)};
                index++;
            }
            

            EvtSystem.EventDispatcher.Raise<ShowResponses>(responseMessage);
        }
    }

    public void Update()
    {
        if (currentDialogue != null && dialogueWaitTime > 0.0f)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= dialogueWaitTime)
            {
                dialogueWaitTime = 0.0f;
                CreateResponseMessage();
            }
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            StartDialogue("DialogueOne");
        }
    }
}
