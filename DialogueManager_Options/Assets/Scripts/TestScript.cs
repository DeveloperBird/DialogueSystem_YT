using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class TestScript : MonoBehaviour {

    public DialogueBase dialogue;

    public void TriggerDialogue()
    {
        DialogueManager.instance.EnqueueDialogue(dialogue);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            TriggerDialogue();
        }
    }


}
