using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[System.Serializable]
public class DialougeCharacter
{
    public string name;
    public Sprite icon;
    public Color textColor = Color.white;
    public VideoClip videoClip;

    public bool useTutorialBackground = false;
    public bool useTutorialText = false;
}

[System.Serializable]
public class DialogueLine
{
    public DialougeCharacter character;
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
        DialogueManager.Instance.StartDialogue(dialogue);
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
