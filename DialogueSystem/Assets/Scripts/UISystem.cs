using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISystem : Singleton<UISystem>
{
    public TMPro.TextMeshProUGUI dialogueText;
    public GameObject buttonContainer;
    public GameObject buttonPrefab;
    public GameObject UIRoot;

    private Queue<GameObject> buttonPool;
    private List<GameObject> activeButtons;

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
        dialogueText.text = eventData.text;
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
            // clear all listeners before reusing the button
            uiButton.onClick.RemoveAllListeners();
            // add fresh listener to button
            uiButton.onClick.AddListener(response.buttonAction);

            activeButtons.Add(button);
        }
    }
}
