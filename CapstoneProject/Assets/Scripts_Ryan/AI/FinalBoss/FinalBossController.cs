using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;

public class FinalBossController : MonoBehaviour
{
    [SerializeField] private EntityManager entityManager;
    [SerializeField] private PriorityQueue<FinalBossAttacks> finalBossAttackQueue = new PriorityQueue<FinalBossAttacks>();
    [SerializeField] private float AttackCoolDownMin = 5f;
    [SerializeField] private float AttackCoolDownMax = 10f;
    [SerializeField] public float DecreaseAttackCoolDownPercentage = 30f;
    [SerializeField] private float PreventDoubleAttackChancePercentage = 50f;


    private float AttackCoolDown;
    private float timeSinceLastAttack = 0f;
    private GameObject playerGameObject;
    private FinalBossAttacks previousAttack;
    public Action PhaseTwo;

    void Awake()
    {
        playerGameObject = GameObject.FindGameObjectWithTag("Player"); //Find the player in the world
        AttackCoolDown = UnityEngine.Random.Range(AttackCoolDownMin, AttackCoolDownMax);
        PhaseTwo += cutAttackCoolDown;
    }

    void Update()
    {
        //REMEMBER TO REMOVE THIS LINE ONCE U HAVE ENTITYMANAGER IN
        //if(entityManager.currentHealth <= entityManager.maxHealth / 2) PhaseTwo?.Invoke();
        AttemptToAttack();
    }

    private void cutAttackCoolDown()
    {
        float mid = (AttackCoolDownMin + AttackCoolDownMax)/2;
        mid -= mid * (DecreaseAttackCoolDownPercentage/100f);
        AttackCoolDownMax -= mid;
        AttackCoolDownMin = Mathf.Clamp(AttackCoolDownMin - mid, 1f, AttackCoolDownMin);
    }

    private void AttemptToAttack()
    {
        if(HasCooldownExpired() && finalBossAttackQueue.Count() > 0)
        {
            float preventSameAttack = UnityEngine.Random.Range(0, 100);
            FinalBossAttacks fbaPeek = finalBossAttackQueue.Peek();
            FinalBossAttacks fba;
            if(fbaPeek == previousAttack && preventSameAttack <= PreventDoubleAttackChancePercentage)
            {
                fba = GetNextAttack();
            }
            else
            {
                fba = finalBossAttackQueue.Pop();
            }

            Attack(fba);
            previousAttack = fba;
        }
    }

    private FinalBossAttacks GetNextAttack()
    {
        List<FinalBossAttacks> skipped = new List<FinalBossAttacks>();
        FinalBossAttacks fba = null;

        while(finalBossAttackQueue.Count() > 0)
        {
            FinalBossAttacks attack = finalBossAttackQueue.Pop();

            if(attack != previousAttack)
            {
                fba = attack;
                break;
            }

            skipped.Add(attack);
        }

        if(fba == null && skipped.Count > 0)
        {
            fba = skipped[0];
            skipped.RemoveAt(0);
        }

        foreach(FinalBossAttacks attack in skipped)
        {
            AddToAttackQueue(attack);
        }

        return fba;
    }

    private bool HasCooldownExpired()
    {
        float currentTime = Time.time;
        return currentTime - timeSinceLastAttack >= AttackCoolDown;
    }

    private void Attack(FinalBossAttacks attack)
    {
        attack.Attack(playerGameObject.transform);
        AttackCoolDown = UnityEngine.Random.Range(AttackCoolDownMin, AttackCoolDownMax);
        timeSinceLastAttack = Time.time;
    }

    public void AddToAttackQueue(FinalBossAttacks finalBossAttacks)
    {
        finalBossAttackQueue.Enqueue(finalBossAttacks, (int)finalBossAttacks.priority);
    }
}
