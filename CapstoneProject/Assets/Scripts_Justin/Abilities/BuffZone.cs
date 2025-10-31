using System.Collections.Generic;
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
    //TODO Implement buffing feature
    
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

    public void StartTimer(float duration)
    {
        this.duration = duration;
        hasTimerStarted = true;
    }


    // Update is called once per frame
    void Update()
    {
        if(hasTimerStarted)
        {
            currentTimer += Time.deltaTime;
            if (currentTimer >= duration) Destroy(gameObject);
        }
    }
}
