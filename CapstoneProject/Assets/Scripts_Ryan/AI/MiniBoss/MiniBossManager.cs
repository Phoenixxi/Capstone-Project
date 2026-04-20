using UnityEngine;

public class MiniBossManager : MonoBehaviour
{
    [SerializeField] private ArrowDirection arrowDirection;
    [SerializeField] private GameObject MiniBoss;
    [SerializeField] private Collider FrontCollider;
    [SerializeField] private Collider TriggerCollider;
    [SerializeField] private Collider ExitCollider;
    private EnemyControllerMiniBoss enemyControllerMiniBoss;
    private EntityManager MiniBossEntityManager;
    private FlashingArrow arrow;
    void Awake()
    {
        if(MiniBoss == null)
        {
            Debug.LogError("Mini boss not found");
        }

        enemyControllerMiniBoss = MiniBoss.GetComponentInChildren<EnemyControllerMiniBoss>(true);
        MiniBossEntityManager = MiniBoss.GetComponentInChildren<EntityManager>();

        Setup();
    }

    void Start()
    {
        MiniBossEntityManager.OnEntityKilledEvent += OnMiniBossDeath;

        switch(arrowDirection)
        {
            case ArrowDirection.Right:
                arrow = GameObject.Find("RightArrow").GetComponent<FlashingArrow>();
                break;
            case ArrowDirection.Up:
                arrow = GameObject.Find("UpArrow").GetComponent<FlashingArrow>();
                break;
            case ArrowDirection.Down:
                arrow = GameObject.Find("DownArrow").GetComponent<FlashingArrow>();
                break;
            case ArrowDirection.None:
                arrow = null;
                break;
        }
    }

    private void Setup()
    {
        enemyControllerMiniBoss.enabled = false;
        FrontCollider.enabled = false;
        TriggerCollider.enabled = true;
        ExitCollider.enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            FrontCollider.enabled = true;
            TriggerCollider.enabled = false;
            enemyControllerMiniBoss.enabled = true;
        }
    }

    private void OnMiniBossDeath()
    {
        ExitCollider.enabled = false;
        arrow.StartFlash();
    }
}
