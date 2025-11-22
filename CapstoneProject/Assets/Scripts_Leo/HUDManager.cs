using UnityEngine;

public class HUDManager : MonoBehaviour
{
    [SerializeField] private SwappingManager swapper;
    [SerializeField] public Image[] cooldowns;

    void Start()
    {
        swapper.SwapCharacterEvent += OnCharacterSwap;
    }

    void Update()
    {
        
    }

    private void OnCharacterSwap(int charNum)
    {
        SwapAbilityIcon(charNum);
    }

    private void SwapAbilityIcon(int charNum)
    {
        for (int i = 0; i < 3; i++)
        {
            cooldowns[i].SetActive(i == charNum);
            //if (i == charNum) cooldowns[i].Enable();
        }
    }
}
