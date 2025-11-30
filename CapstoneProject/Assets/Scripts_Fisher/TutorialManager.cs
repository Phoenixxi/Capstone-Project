using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.Video;



public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    public Image characterIcon;
    public TextMeshProUGUI charactername;
    public TextMeshProUGUI dialogueArea;

    private Queue<TutorialLine> lines;

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

        lines = new Queue<TutorialLine>();
    }

    private void Start()
    {

        player = GameObject.Find("Player");

        gameObject.SetActive(false);
        animator.Play("hide1");


        //dialogueVideo.time = 0;
        //dialogueVideo.Play();

        if (playerInput == null)
            playerInput = GameObject.Find("Player").GetComponent<PlayerInput>();

        isDialogueActive = false;


        //dialogueVideo.Pause();


    }

    public void StartTutorial(Tutorial tutorial)
    {
        isDialogueActive = true;
        gameObject.SetActive(true);

        // Disable Player Movement
        player.GetComponent<PlayerController>().SetCanMove(false);

        animator.Play("show1");

        lines.Clear();

        //dialogueVideo.Play();

        foreach (TutorialLine tutorialLine in tutorial.tutorialLines)
        {
            lines.Enqueue(tutorialLine);
        }

        DisplayNextTutorialLine();
    }

    public void DisplayNextTutorialLine()
    {
        if (lines.Count == 0)
        {
            EndTutorial();
            return;
        }

        TutorialLine currentLine = lines.Dequeue();
        characterIcon.sprite = currentLine.character.icon;
        charactername.text = currentLine.character.name;
        dialogueArea.color = currentLine.character.textColor;

        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentLine));
    }

    IEnumerator TypeSentence(TutorialLine tutorialLine)
    {
        dialogueArea.text = "";
        foreach (char letter in tutorialLine.line.ToCharArray())
        {
            dialogueArea.text += letter;
            yield return null;
        }
    }

    void EndTutorial()
    {
        isDialogueActive = false;
        gameObject.SetActive(false);

        player.GetComponent<PlayerController>().SetCanMove(true);

        //dialogueVideo.Stop();
        //dialogueVideo.time = 0;



        animator.Play("hide1");
    }
}
