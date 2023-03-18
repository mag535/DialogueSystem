using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;


class StringReveal
{
    string textToReveal = null;

    float currentTime;
    float secondsPerChar;
    int currentStringIndex = 0;

    public void StartReveal(string text, float duration)
    {
        secondsPerChar = duration / text.Length;
        textToReveal = text;

        currentStringIndex = 0;
        currentTime = 0.0f;
    }

    public bool isDone()
    {
        return (textToReveal == null || currentStringIndex == (textToReveal.Length - 1));
    }

    public void ForceFinish()
    {
        currentStringIndex = (textToReveal.Length - 1);
        currentTime = 0.0f;
    }


    public string GetCurrentRevealedText()
    {
        currentTime += Time.deltaTime;

        if (currentTime >= secondsPerChar && currentStringIndex < (textToReveal.Length - 1))
        {
            currentStringIndex++;
            currentTime = 0.0f;
        }

        return textToReveal.AsSpan(0, currentStringIndex).ToString();
    }
}



public class UISystem : Singleton<UISystem>
{
    public TMPro.TextMeshProUGUI dialogueText;
    public GameObject buttonContainer;
    public GameObject buttonPrefab;
    public GameObject UIRoot;

    private Queue<GameObject> buttonPool;
    private List<GameObject> activeButtons;

    private StringReveal typewriter = new StringReveal();

    private void Start()
    {
        // create 4 buttons and add to queue
        buttonPool = new Queue<GameObject>();
        for (int i = 0; i<4; i++)
        {
            var button = GameObject.Instantiate(buttonPrefab, buttonContainer.transform);
            button.SetActive(false);
            buttonPool.Enqueue(button);
        }

        // Create a list of buttons to hold responses
        activeButtons = new List<GameObject>();
        // Disable UI until start of dialogue (by pressing E)
        //UIRoot.SetActive(false);

        EvtSystem.EventDispatcher.AddListener<ShowDialogueText>(ShowUI);
        EvtSystem.EventDispatcher.AddListener<ShowResponses>(ShowResponseButtons);
        EvtSystem.EventDispatcher.AddListener<DisableUI>(HideUI);
    }

    private void OnDisable()
    {
        EvtSystem.EventDispatcher.RemoveListener<ShowDialogueText>(ShowUI);
    }

    private void ShowUI(ShowDialogueText eventData)
    {
        UIRoot.SetActive(true);
        typewriter.StartReveal(eventData.text, eventData.duration);
    }

    private void HideUI(DisableUI data)
    {
        // deactivate buttons
        foreach (GameObject button in activeButtons)
        {
            button.SetActive(false);
            buttonPool.Enqueue(button);
        }
        // clear button list
        activeButtons.Clear();
        // disable UI
        UIRoot.SetActive(false);
    }

    private void ShowResponseButtons(ShowResponses eventData)
    {
        //Force the dialogue line to display fully.
        if (!typewriter.isDone())
        {
            typewriter.ForceFinish();
            dialogueText.text = typewriter.GetCurrentRevealedText();
        }

        // iterate through possible responses
        foreach (ResponseData response in eventData.responses)
        {
            // check button queue for button
            GameObject button = null;
            if (!buttonPool.TryDequeue(out button))
            {
                button = Instantiate(buttonPrefab, buttonContainer.transform);
            }
            button.SetActive(true);

            TMPro.TextMeshProUGUI label = button.GetComponentInChildren<TMPro.TextMeshProUGUI>();

            if (label != null)
            {
                label.text = response.text;
            }

            Button uiButton = button.GetComponent<Button>();
            // change button's color depending on karma score
            //  magenta == bad(-), yellow == good (+), gray == neutral (0)
            var uiButtonColors = uiButton.colors;
            if (response.karmaScore < 0)
            {
                uiButtonColors.normalColor = Color.magenta;
                uiButtonColors.highlightedColor = new Color(0.97f, 0.47f, 0.97f, 1.0f);
            }
            else if (response.karmaScore > 0)
            {
                uiButtonColors.normalColor = Color.yellow;
                uiButtonColors.highlightedColor = new Color(0.98f, 0.93f, 0.39f, 1.0f);
            }
            else
            {
                uiButtonColors.normalColor = Color.gray;
                uiButtonColors.highlightedColor = new Color(0.72f, 0.72f, 0.72f, 1.0f);
            }
            uiButton.colors = uiButtonColors;

            // clear all listeners before reusing the button
            uiButton.onClick.RemoveAllListeners();
            // add fresh listener to button
            uiButton.onClick.AddListener(response.buttonAction);

            activeButtons.Add(button);
        }
    }

    private void Update()
    {
        if (UIRoot.activeSelf && !typewriter.isDone())
            dialogueText.text = typewriter.GetCurrentRevealedText();
    }
}
