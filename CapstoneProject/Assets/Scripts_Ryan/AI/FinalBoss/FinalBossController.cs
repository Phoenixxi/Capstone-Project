using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Analytics;

public class FinalBossController : MonoBehaviour
{
    [SerializeField] private EntityManager entityManager;
    [SerializeField] private float AttackCoolDownMin = 5f;
    [SerializeField] private float AttackCoolDownMax = 10f;
    [SerializeField] public float DecreaseAttackCoolDownPercentage = 30f;

    private FinalBossAttacks[] finalBossAttacks;
    private float AttackCoolDown;
    private float timeSinceLastAttack = 0f;
    private GameObject playerGameObject;
    private FinalBossAttacks previousAttack;
    public Action PhaseTwo;
    private FinalBossHealthUI finalBossHealthUI;
    private bool initialized = false;

    void Awake()
    {
        finalBossAttacks = GetComponents<FinalBossAttacks>();
        playerGameObject = GameObject.FindGameObjectWithTag("Player"); //Find the player in the world
        AttackCoolDown = UnityEngine.Random.Range(AttackCoolDownMin, AttackCoolDownMax);
        PhaseTwo += cutAttackCoolDown;
    }

    void Start()
    {
        foreach(FinalBossAttacks fba in finalBossAttacks)
        {
            fba.enabled = false;
        }

        FinalBossManagerSingleton.Instance.InitializeFinalBoss += OnInitializeFinalBoss;
        entityManager = GetComponent<EntityManager>();
        entityManager.OnEntityHurtEvent += OnHit;
        finalBossHealthUI = GameObject.Find("FinalBossHealthBarRoot").GetComponentInChildren<FinalBossHealthUI>();
    }

    void Update()
    {
        if(!initialized) return;
        if(entityManager.currentHealth <= entityManager.maxHealth / 2) PhaseTwo?.Invoke();
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
        if(previousAttack != null && previousAttack.IsAttacking()) return;

        if(HasCooldownExpired())
        {
            float bestWeight = -Mathf.Infinity;
            FinalBossAttacks fba = null;
            foreach(FinalBossAttacks fbas in finalBossAttacks)
            {
                if(bestWeight < fbas.GetDynamicWeight() && fbas.HasCooldownExpired())
                {
                    bestWeight = fbas.GetDynamicWeight();
                    fba = fbas;
                }
            }

            if(fba == null) return;
            Attack(fba);
            previousAttack = fba;
        }
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

    private void OnInitializeFinalBoss()
    {
        foreach(FinalBossAttacks fba in finalBossAttacks)
        {
            fba.enabled = true;
        }

        initialized = true;
    }

    private void OnHit()
    {
        finalBossHealthUI.UpdateHealthBar(entityManager.currentHealth, entityManager.maxHealth);
    }
}
