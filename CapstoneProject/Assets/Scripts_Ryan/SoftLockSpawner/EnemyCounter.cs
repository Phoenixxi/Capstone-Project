using TMPro;
using UnityEngine;

public class EnemyCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmp;
    [SerializeField] private TextMeshProUGUI EnemyRemainingTMP;
    private int count;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        tmp.enabled = false;
        EnemyRemainingTMP.enabled = false;
    }

    public void initializeCount(int count)
    {
        this.count = count;
        tmp.text = count.ToString();
        tmp.enabled = true;
        EnemyRemainingTMP.enabled = true;
    }

    public void decreaseCount()
    {
        count--;
        tmp.text = count.ToString();
    }

    public void disableText()
    {
        tmp.enabled = false;
        EnemyRemainingTMP.enabled = false;
    }
    
}
