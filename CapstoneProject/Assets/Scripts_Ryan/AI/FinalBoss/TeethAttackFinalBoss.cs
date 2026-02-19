using UnityEngine;

public class TeethAttackFinalBoss : FinalBossAttacks
{
    [SerializeField] private Transform TeethAttackTransform;
    private ToothAttack toothAttackScript;

    public override void Attack(Transform PlayerTransform)
    {
        if(!HasCooldownExpired()) return;

        toothAttackScript.Attack();

        UpdateTimeSinceLastAttack();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        if(toothAttackScript == null)
        {
            toothAttackScript = TeethAttackTransform.GetComponent<ToothAttack>();
        }
    }

    public bool IsAttacking()
    {
        return toothAttackScript.IsAttacking;
    }
}
