using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameManager {
    public static int currentScene = 0;

    public static void NextLevel() {
        Debug.Log("Next level");
        currentScene++;
        if (currentScene > 2) {
            currentScene = 0;
        }
        if (currentScene == 2) {
            AddPersonality(Personality.SOCIALIZE);
        }
        SceneManager.LoadScene(currentScene);
    }

    public static void StartDialogue() {
        Debug.Log("Add Dialogue");
        if (currentScene == 0) {
            DialogueManager.AddDialogue(Personality.FIND, new string[] { "I must get to the gold.. ", "I have to hurry.." });
        }
        else if(currentScene == 1) {
            DialogueManager.AddDialogue(Personality.FIND, new string[] { "I must get to the gold.. ", "I have to hurry.." });
            DialogueManager.AddDialogue(Personality.ATTACK, new string[] { "No one will harm us..", "I will keep us safe.." });
        }
        else if(currentScene == 2) {
            DialogueManager.AddDialogue(Personality.SOCIALIZE, new string[] { "Are these my friends..?", "They look nice!" });
        }
    }

    private static List<Personality> personalities = new List<Personality>() {
        Personality.FIND,
        Personality.ATTACK,
    };

    public static List<Personality> Personalities {
        get {
            return personalities;
        }
    }

    public static void AddPersonality(Personality p) {
        personalities.Add(p);
    }

    public static void RemovePersonality(Personality p) {
        foreach (Personality personality in personalities) {
            if(personality == p) {
                personalities.Remove(personality);
            }
        }
    }
}
