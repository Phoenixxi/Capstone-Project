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
    public TextMeshProUGUI charactername;
    public TextMeshProUGUI dialogueArea;

    private Queue<DialogueLine> lines;

    public bool isDialogueActive = false;

    public float typingSpeed = 0.01f;

    public Animator animator;

    //public VideoPlayer dialogueVideo;

    [SerializeField] private PlayerInput playerInput;

    [SerializeField] private GameObject player;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        lines = new Queue<DialogueLine>();
    }

    private void Start()
    {

        player = GameObject.Find("Player");

        gameObject.SetActive(false);
        animator.Play("hide");


        //dialogueVideo.time = 0;
        //dialogueVideo.Play();

        if (playerInput == null)
            playerInput = GameObject.Find("Player").GetComponent<PlayerInput>();

        isDialogueActive = false;


        //dialogueVideo.Pause();


    }

    public void StartDialogue(Dialogue dialogue)
    {
        isDialogueActive = true;
        gameObject.SetActive(true);

        // Disable Player Movement
        player.GetComponent<PlayerController>().SetCanMove(false);

        animator.Play("show");

        lines.Clear();

        //dialogueVideo.Play();

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
        charactername.text = currentLine.character.name;
        dialogueArea.color = currentLine.character.textColor;

        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentLine));
    }

    IEnumerator TypeSentence(DialogueLine dialogueLine)
    {
        dialogueArea.text = "";
        foreach (char letter in dialogueLine.line.ToCharArray())
        {
            dialogueArea.text += letter;
            yield return null;
        }
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        gameObject.SetActive(false);

        player.GetComponent<PlayerController>().SetCanMove(true);

        //dialogueVideo.Stop();
        //dialogueVideo.time = 0;



        animator.Play("hide");
    }
}


