using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ElementType = lilGuysNamespace.EntityData.ElementType;

/// <summary>
/// Behavior for buff zone placed down by the BuffZoneAbility
/// </summary>
public class BuffZone : MonoBehaviour
{
    public const float SPAWN_OFFSET = -0.522f;


    private float duration = 0f;
    private float currentTimer;
    private float damageRate;
    private int damage;
    private ElementType element = ElementType.Normal;
    private bool hasTimerStarted = false;
    private Dictionary<GameObject, IEnumerator> enemies;
    private EntityManager player;
    private float attackCooldownMultiplier;
    
    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    public void SetDamageRate(float damageRate)
    {
        this.damageRate = damageRate;
    }

    public void SetElement(ElementType element)
    {
        this.element = element;
    }

    public void SetRadius(float radius)
    {
        Vector3 newScale = new Vector3(radius, transform.localScale.y, radius);
        transform.localScale = newScale;
    }

    public void SetAttackCooldownMultiplier(float multiplier)
    {
        attackCooldownMultiplier = multiplier;
    }

    public void StartTimer(float duration)
    {
        this.duration = duration;
        hasTimerStarted = true;
    }


    private void Awake()
    {
        enemies = new Dictionary<GameObject, IEnumerator>();
    }

    void Update()
    {
        if(hasTimerStarted)
        {
            currentTimer += Time.deltaTime;
            if (currentTimer >= duration) Destroy(gameObject);
        }
    }

    private IEnumerator DamageEnemyCoroutine(EntityManager enemy)
    {
        Debug.Log("Damage coroutine started");
        while(enemy != null && enemies.ContainsKey(enemy.gameObject))
        {
            yield return new WaitForSeconds(damageRate);
            enemy.TakeDamage(damage, element);
        }
        Debug.Log("Damage coroutine ended");
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject entity = other.gameObject;
        if(entity.tag == "Enemy")
        {
            if(!enemies.ContainsKey(entity))
            {
                EntityManager enemy = entity.GetComponent<EntityManager>();
                IEnumerator damageCoroutine = DamageEnemyCoroutine(enemy);
                enemies.Add(entity, damageCoroutine);
                StartCoroutine(damageCoroutine);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject entity = other.gameObject;
        if(entity.tag == "Enemy")
        {
            if (enemies.ContainsKey(entity)) enemies.Remove(entity);
        } else if(entity.tag == "Player")
        {
            Debug.Log("Player leaving buff zone");
            player.ResetAttackRate();
            player = null;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        GameObject entity = other.gameObject;
        if(entity.tag == "Player")
        {
            EntityManager currentPlayer = entity.GetComponentInChildren<EntityManager>();
            if (player != currentPlayer) player?.ResetAttackRate();
            player = currentPlayer;
            player.ApplyAttackCooldownMutliplier(attackCooldownMultiplier);
        }
    }

    private void OnDestroy()
    {
        Debug.Log("Player leaving buff zone");
        player?.ResetAttackRate();
    }
}
