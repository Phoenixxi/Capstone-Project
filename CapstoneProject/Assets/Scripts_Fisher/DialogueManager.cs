using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI Elements")]
    public Image characterIcon;
    public TextMeshProUGUI characterName;

    public RawImage dialogueBackground;     // Default dialogue background
    public RawImage tutorialBackground;     // tutorial background
    public TextMeshProUGUI dialogueText;    // Default dialogue text
    public TextMeshProUGUI tutorialText;    // tutorial text

    [Header("Dialogue Settings")]
    public float typingSpeed = 0.01f;

    [HideInInspector] public bool isDialogueActive = false;

    private Queue<DialogueLine> lines;
    private PlayerInput playerInput;
    private GameObject player;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        lines = new Queue<DialogueLine>();
    }

    private void Start()
    {
        player = GameObject.Find("Player");
        playerInput = player.GetComponent<PlayerInput>();

        gameObject.SetActive(false);
        isDialogueActive = false;
    }

    public void StartDialogue(Dialogue dialogue)
    {
        if (player == null) player = GameObject.Find("Player");

        isDialogueActive = true;
        gameObject.SetActive(true);

        // Disable player movement
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

        // Update icon, name, text color
        characterIcon.sprite = currentLine.character.icon;
        characterName.text = currentLine.character.name;

        
        TextMeshProUGUI activeText;

        if (currentLine.character.useAlternateBackground)
        {
            dialogueBackground.gameObject.SetActive(false);
            tutorialBackground.gameObject.SetActive(true);
        }
        else
        {
            dialogueBackground.gameObject.SetActive(true);
            tutorialBackground.gameObject.SetActive(false);
        }

        if (currentLine.character.useAlternateText)
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

        // Start typing coroutine
        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentLine, activeText));
    }

    private IEnumerator TypeSentence(DialogueLine dialogueLine, TextMeshProUGUI activeText)
    {
        Debug.Log("Typing to: " + activeText.name + " | Line: " + dialogueLine.line);in

        activeText.text = "";
        foreach (char letter in dialogueLine.line.ToCharArray())
        {
            activeText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    private void EndDialogue()
    {
        isDialogueActive = false;
        gameObject.SetActive(false);

        player.GetComponent<PlayerController>().SetCanMove(true);

        // Reset backgrounds and text 
        dialogueBackground.gameObject.SetActive(true);
        tutorialBackground.gameObject.SetActive(false);
        dialogueText.gameObject.SetActive(true);
        tutorialText.gameObject.SetActive(false);
    }
}
