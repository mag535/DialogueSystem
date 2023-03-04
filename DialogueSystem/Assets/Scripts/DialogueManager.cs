using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : Singleton<DialogueManager>
{
    #region SingletonStuff
    #endregion

    public DialogueDatabase database;

    private float dialogueWaitTime = 0.0f;
    private const float kDefaultWaitTime = 2.0f;
    private DialogueLineData currentDialogue = null;
    private float currentTime = 0.0f;

    // Dictionary for dialogue
    Dictionary<string, DialogueLineData> dialogueDatabase = new Dictionary<string, DialogueLineData>();

    public void Start()
    {
        LoadDatabase();
    }

    public void LoadDatabase()
    {
        //ITERATE THROUGH DIALOUGE DATABASE AND ADD IT TO THE DICTIONARY
        foreach (DialogueLineData line in database.data)
        {
            dialogueDatabase.Add(line.name, line);
        }
    }

    //STARTS DIALOUGE
    public void StartDialogue(string dialogueName)
    {
        DialogueLineData data = null;

        if(dialogueDatabase.TryGetValue(dialogueName, out data))
        {
            // only start dialogue if new line of dialogue
            if (data != currentDialogue)
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
            //RESET VALUES OF DIALOGUE AND TIME
            dialogueWaitTime = kDefaultWaitTime;
        }
        currentDialogue = lineToStart;
        currentTime = 0.0f;
    }

    private void PlayResponseLine(int currentResponseIndex)
    {
        // disable UI before playing next response
        EvtSystem.EventDispatcher.Raise<DisableUI>(new DisableUI());
        
        if (currentDialogue.responses.Length > currentResponseIndex)
        {
            DialogueLineData line = currentDialogue.responses[currentResponseIndex];
            StartDialogue(line);
        }
    }

    private void CreateResponseMessage()
    {
        //HOW MANY RESPONSES YOU HAVE
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
                responseMessage.responses[index].buttonAction = () => { this.PlayResponseLine(currentIndex);  };
                index++;
            }
            

            EvtSystem.EventDispatcher.Raise<ShowResponses>(responseMessage);
        }
        else
        {
            // disable UI bc at the end of dialogue tree
            EvtSystem.EventDispatcher.Raise<DisableUI>(new DisableUI());
            currentDialogue = null;
        }
    }

    public void Update()
    {
        //CHECK IF CURRENT DIALOGUE IS PLAYING
        if (currentDialogue != null && dialogueWaitTime > 0.0f)
        {
            //IF IT IS, ADD TIME TO TIMER
            currentTime += Time.deltaTime;
            //IF TIMER REACHES DESIGNATED WAIT TIME, PLAY AUDIO
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
