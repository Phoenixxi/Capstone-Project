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
    public GameObject leftClickAnim;
 
    private Queue<DialogueLine> lines; //queue of lines, characters, text color, video clip
 
    public float typingSpeed; //typing speed

    private PlayerInput playerInput;
    private GameObject dialogueTrigger;
 
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
 
        lines = new Queue<DialogueLine>();

        HideElements(true);
    }

    void Start()
    {
        GameObject Player = GameObject.Find("Player");
        playerInput = Player.GetComponent<PlayerInput>();
    }

    public void StartDialogue(Dialogue dialogue, GameObject trigger)
    {
        playerInput.actions.FindActionMap("Player").Disable();
        playerInput.actions.FindActionMap("UI").Disable();

        dialogueTrigger = trigger;

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

        if(currentLine.tutorial.isTutorial) 
            Tutorial(currentLine);
        else Dialogue(currentLine);
    }

    private void Dialogue(DialogueLine currentLine)
    {
        DialogueBG.SetActive(true);
        TutorialBG.SetActive(false);
        leftClickAnim.SetActive(true);

        characterIcon.sprite = currentLine.character.icon;
        characterName.text = currentLine.character.name;

        StartCoroutine(TypeSentence(currentLine, dialogueTextArea));
    }

    private void Tutorial(DialogueLine currentLine)
    {
        DialogueBG.SetActive(false);
        TutorialBG.SetActive(true);

        videoPlayer.clip = currentLine.tutorial.videoClip;

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
        videoPlayer.Stop();
        HideElements(true);
        Destroy(dialogueTrigger);
        playerInput.actions.FindActionMap("Player").Enable();
        playerInput.actions.FindActionMap("UI").Enable();
    }

    private void HideElements(bool hide)
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(!hide);
        }
    }
}