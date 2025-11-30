using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.Video;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public Image characterIcon;
    public TextMeshProUGUI characterName;

    public RawImage dialogueBackground;
    public RawImage tutorialBackground;

    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI tutorialText;

    public VideoClip videoClip;
    public float typingSpeed = 0.01f;

    [HideInInspector] public bool isDialogueActive = false;

    private Queue<DialogueLine> lines;
    private PlayerInput playerInput;
    private GameObject player;

    [SerializeField] private VideoPlayer tutorialVideoPlayer;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        lines = new Queue<DialogueLine>();
    }

    private void Start()
    {
        player = GameObject.Find("Player");
        playerInput = player.GetComponent<PlayerInput>();
        gameObject.SetActive(false);
        isDialogueActive = false;

        tutorialVideoPlayer.GetComponent<RawImage>().enabled = false;
    }

    public void StartDialogue(Dialogue dialogue)
    {
        if (player == null)
            player = GameObject.Find("Player");

        isDialogueActive = true;
        gameObject.SetActive(true);

        player.GetComponent<PlayerController>().SetCanMove(false);

        lines.Clear();
        foreach (DialogueLine dialogueLine in dialogue.dialogueLines)
        {
            lines.Enqueue(dialogueLine);
        }

        DisplayNextDialogueLine();
    }

    public void DisplayNextDialogueLine()
    {
        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine currentLine = lines.Dequeue();

        characterIcon.sprite = currentLine.character.icon;
        characterName.text = currentLine.character.name;

        TextMeshProUGUI activeText;

        if (currentLine.character.useTutorialBackground)
        {
            dialogueBackground.gameObject.SetActive(false);
            tutorialBackground.gameObject.SetActive(true);
        }
        else
        {
            dialogueBackground.gameObject.SetActive(true);
            tutorialBackground.gameObject.SetActive(false);
        }

        if (currentLine.character.useTutorialText)
        {
            dialogueText.gameObject.SetActive(false);
            tutorialText.gameObject.SetActive(true);
            activeText = tutorialText;
        }
        else
        {
            dialogueText.gameObject.SetActive(true);
            tutorialText.gameObject.SetActive(false);
            activeText = dialogueText;
        }

        activeText.color = currentLine.character.textColor;

        if (currentLine.character.videoClip != null && currentLine.character.videoClip.name != "fix")
        {
            tutorialVideoPlayer.gameObject.SetActive(true);
            tutorialVideoPlayer.clip = currentLine.character.videoClip;
            tutorialVideoPlayer.Play();
        }
        else
        {
            tutorialVideoPlayer.Stop();
            tutorialVideoPlayer.gameObject.SetActive(false);
            tutorialVideoPlayer.GetComponent<RawImage>().enabled = false;
        }

        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentLine, activeText));
    }

    private IEnumerator TypeSentence(DialogueLine dialogueLine, TextMeshProUGUI activeText)
    {
        Debug.Log("Typing to: " + activeText.name + " | Line: " + dialogueLine.line);

        activeText.text = "";

        foreach (char letter in dialogueLine.line.ToCharArray())
        {
            activeText.text += letter;
            yield break;
        }
    }

    private void EndDialogue()
    {
        isDialogueActive = false;
        gameObject.SetActive(false);

        player.GetComponent<PlayerController>().SetCanMove(true);

        dialogueBackground.gameObject.SetActive(true);
        tutorialBackground.gameObject.SetActive(false);

        dialogueText.gameObject.SetActive(true);
        tutorialText.gameObject.SetActive(false);

        tutorialVideoPlayer.GetComponent<RawImage>().enabled = false;
    }
}
