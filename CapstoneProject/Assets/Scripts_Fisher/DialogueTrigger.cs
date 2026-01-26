using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

[System.Serializable]
public class DialougeCharacter
{
    [Header("The name of the character speaking")]
    public string name;
    [Header("The character sprite")]
    public Sprite icon;
}

[System.Serializable]
public class DialogueTutorial
{
    [Header("Control icon for tutorial")]
    public Sprite icon;
    [Header("Set true if dialogue is a tutorial")]
    public bool isTutorial;
}

// [System.Serializable]
// public class TutorialIcons
// {
//     [Header("The Icon to display")]
//     public Sprite icon;
// }

[System.Serializable]
public class DialogueLine
{
    [Header("Ignore this section if the dialogue is supposed to be a tutorial \nIn other words leave everything null if it is a tutorial")]
    public DialougeCharacter character;
    [Header("Ignore this section if the dialogue is not supposed to be a tutorial \nIn other words leave everything null if it is an actual dialogue")]
    public DialogueTutorial tutorial;

    [Header("The actual dialogue WHY NOT WORKING")]
    [TextArea(3, 10)]
    public string line;
}

[System.Serializable]
public class Dialogue
{
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();
}

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    public void TriggerDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogue, gameObject);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("hello");
            TriggerDialogue();
        }
    }
}
