using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TutorialCharacter
{
    public string name;
    public Sprite icon;
    public Color textColor = Color.white;
}

[System.Serializable]
public class TutorialLine
{
    public TutorialCharacter character;
    [TextArea(3, 10)]
    public string line;
}

[System.Serializable]
public class Tutorial
{
    public List<TutorialLine> tutorialLines = new List<TutorialLine>();
}

public class TutorialTrigger : MonoBehaviour
{

    public Tutorial tutorial;

    public void TriggerTutorial()
    {
        TutorialManager.Instance.StartTutorial(tutorial);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Collided with: " + collision.name);

            if (TutorialManager.Instance != null)
            {
                Debug.Log("TutorialManager instance found!");
                TriggerTutorial();
            }
            else
            {
                Debug.LogError("TutorialManager instance is null!");
            }
        }
    }
}