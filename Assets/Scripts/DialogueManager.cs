using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DialoguePersonality {
    public string dialogue;
    public Personality personality;

    public DialoguePersonality(string d, Personality p) {
        dialogue = d;
        personality = p;
    }
}

public class DialogueManager : MonoBehaviour {
    private static DialogueManager thisDialogueManager;

    public GameObject dialoguePanel;
    public Text dialogueText;

    public float timePerDialogue = 3.0f;

    public GameObject standardModel;
    public GameObject attackModel;
    public GameObject socializeModel;

    public Queue<DialoguePersonality> dialoguePersonalities;
    public Queue<DialoguePersonality> DialoguePersonalities {
        get {
            return dialoguePersonalities;
        }
        set {
            dialoguePersonalities = value;
        }
    }

    public Personality currentPersonality;

    private float timer = 0;
    private bool active;

	// Use this for initialization
	void Start () {
        thisDialogueManager = this;
        dialoguePersonalities = new Queue<DialoguePersonality>();
        standardModel.SetActive(false);
        attackModel.SetActive(false);
        socializeModel.SetActive(false);
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }
	
	// Update is called once per frame
	void Update () {
        if (active) {
            timer -= Time.deltaTime;

            if (timer < 0 || Input.GetKeyDown(KeyCode.Space)) {
                timer = timePerDialogue;
                if (currentPersonality != Personality.NONE) {
                    ExitDialogue(currentPersonality);
                }
                if (DialoguePersonalities.Count > 0) {
                    DisplayDialogue(DialoguePersonalities.Dequeue());
                }
            }
        }
	}

    public static void AddDialogue(Personality personality, string[] dialogue) {
        for (int i = 0; i < dialogue.Length; i++) {
            thisDialogueManager.DialoguePersonalities.Enqueue(new DialoguePersonality(dialogue[i], personality));
        }
        thisDialogueManager.active = true;
        PersonalityController.PauseGame = true;
    }

    private static void DisplayDialogue(DialoguePersonality dialoguePersonality) {
        thisDialogueManager.currentPersonality = dialoguePersonality.personality;

        switch (dialoguePersonality.personality) {
            case Personality.FIND:
                thisDialogueManager.standardModel.SetActive(true);
                break;
            case Personality.ATTACK:
                thisDialogueManager.attackModel.SetActive(true);
                break;
            case Personality.SOCIALIZE:
                thisDialogueManager.socializeModel.SetActive(true);
                break;
        }

        thisDialogueManager.dialoguePanel.SetActive(true);
        thisDialogueManager.dialogueText.text = dialoguePersonality.dialogue;
    }

    private void ExitDialogue(Personality personality) {
        switch (personality) {
            case Personality.FIND:
                standardModel.SetActive(false);
                break;
            case Personality.ATTACK:
                attackModel.SetActive(false);
                break;
            case Personality.SOCIALIZE:
                socializeModel.SetActive(false);
                break;
            
        }

        dialoguePanel.SetActive(false);
        dialogueText.text = "";

        if (DialoguePersonalities.Count < 1) {
            timer = 0;
            PersonalityController.PauseGame = false;
            active = false;
        }
    }
}
