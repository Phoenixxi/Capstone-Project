using UnityEngine;
using UnityEngine.UI;
using lilGuysNamespace;

public class PlayerUI : MonoBehaviour
{
    public EntityManager entityManager;
    public Text healthText;

    private void Start()
    {
        healthText.text = "Health: " + entityManager.currentHealth.ToString();
    }

    public void Update()
    {
        healthText.text =  "Health: " + ((int)entityManager.currentHealth).ToString();
    }

}
