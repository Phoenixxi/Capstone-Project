using UnityEngine;

public class RevivePack : MonoBehaviour
{
    // Serialized so that designers can adjust in the inspector
    [SerializeField] public float healAmount = 10f; // how much health to restore

    //Note this will likely be removed in the future, just here so there can be some kind of visual indicator of using the station
    [SerializeField] private GameObject healthSign;
    [SerializeField] private GameObject healedText;
    private bool isUsed = false;

    private void OnTriggerEnter(Collider other)  
    {
       PlayerController player = other.GetComponentInParent<PlayerController>();

        if (player != null && !isUsed)
        {
            player.HealAllCharacters(healAmount);
            //Destroy(gameObject); // remove the health pack after healing
            healthSign.SetActive(false);
            healedText.SetActive(true);
            isUsed = true;
        }
    }
}
