using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.Video;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance; //Singleton
 
    public Image characterIcon; //Character image
    public TextMeshProUGUI characterName; //Character name
    public TextMeshProUGUI dialogueTextArea; //dialogue area
    public TextMeshProUGUI tutorialTextArea;
    public GameObject DialogueBG;
    public GameObject TutorialBG;
    public VideoPlayer videoPlayer;
    public Button continuteBtn;
 
    private Queue<DialogueLine> lines; //queue of lines, characters, text color, video clip
    
    public bool isDialogueActive = false; //self explanatory
 
    public float typingSpeed = 0.01f; //typing speed
 
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
 
        lines = new Queue<DialogueLine>();

        HideElements(true);
    }
 
    public void StartDialogue(Dialogue dialogue)
    {
        isDialogueActive = true;
 
        lines.Clear();
 
        foreach (DialogueLine dialogueLine in dialogue.dialogueLines)
        {
            lines.Enqueue(dialogueLine);
        }

        continuteBtn.gameObject.SetActive(true);
 
        DisplayNextDialogueLine();
    }
 
    public void DisplayNextDialogueLine()
    {
        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        StopAllCoroutines();
 
        DialogueLine currentLine = lines.Dequeue();

        if(currentLine.character.isTutorial) Tutorial(currentLine);
        else Dialogue(currentLine);
    }

    private void Dialogue(DialogueLine currentLine)
    {
        DialogueBG.SetActive(true);
        TutorialBG.SetActive(false);

        characterIcon.sprite = currentLine.character.icon;
        characterName.text = currentLine.character.name;

        StartCoroutine(TypeSentence(currentLine, dialogueTextArea));
    }

    private void Tutorial(DialogueLine currentLine)
    {
        DialogueBG.SetActive(false);
        TutorialBG.SetActive(true);

        videoPlayer.clip = currentLine.character.videoClip;

        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += OnVideoPrepared;

        StartCoroutine(TypeSentence(currentLine, tutorialTextArea));
    }

    private void OnVideoPrepared(VideoPlayer vp)
    {
        vp.prepareCompleted -= OnVideoPrepared;
        vp.Play();
    }
 
    IEnumerator TypeSentence(DialogueLine dialogueLine, TextMeshProUGUI dialogueArea)
    {
        dialogueArea.text = "";
        foreach (char letter in dialogueLine.line.ToCharArray())
        {
            dialogueArea.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
 
    void EndDialogue()
    {
        isDialogueActive = false;
        videoPlayer.Stop();
        HideElements(true);
    }

    private void HideElements(bool hide)
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(!hide);
        }
    }
}