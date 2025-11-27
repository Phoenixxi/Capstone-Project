using UnityEngine;

public class TriggerBox : MonoBehaviour
{
    public DialogueTrigger dialogueTrigger; // assign in Inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("TriggerBox activated!");

            if (dialogueTrigger != null)
            {
                dialogueTrigger.TriggerDialogue();
            }
        }
    }
}
