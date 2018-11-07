using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonalityControllerStart : PersonalityController {
    private int countStates = 0;
    private bool firstAttack = true;
    
    protected override void ChangeState(Personality newPersonality) {
        if (GameManager.currentScene == 0) {
            base.ChangeState(newPersonality);
            if (newPersonality == Personality.FIND) {
                countStates++;
                if (countStates == 2) {
                    DialogueManager.AddDialogue(Personality.FIND, new string[] { "Oh no..", "I can't let the other personalities take over.. ", "I must continue.." });
                }
            }
            else if (newPersonality == Personality.ATTACK) {
                if (firstAttack) {
                    DialogueManager.AddDialogue(Personality.ATTACK, new string[] { "I will protect us..", "If anyone comes close to us..", "I will kill them!" });
                    firstAttack = false;
                }
            }
        }
        else {
            base.ChangeState(newPersonality);
        }
    }
}
