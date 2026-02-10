using TMPro;
using UnityEngine;

public class EnemyCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmp;
    private int count;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        tmp.enabled = false;
    }

    public void initializeCount(int count)
    {
        this.count = count;
        tmp.text = count.ToString();
        tmp.enabled = true;
    }

    public void decreaseCount()
    {
        count--;
        tmp.text = count.ToString();
    }

    public void disableText()
    {
        tmp.enabled = false;
    }
    
}
