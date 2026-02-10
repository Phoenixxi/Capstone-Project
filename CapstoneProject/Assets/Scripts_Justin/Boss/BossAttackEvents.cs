using System;
using UnityEngine;

/// <summary>
/// Simple helper class used to activate certain code-based features during specific points in the boss's attack animation. Should be placed on
/// whatever object in the attack hierarchy is animated
/// </summary>
public class BossAttackEvents : MonoBehaviour
{
    public Action BecomeDamagingEvent; //Intended to be used to activate hurtboxes and things along those lines
    public Action AttackEndedEvent; //Use when the animation finishes

    private void BecomeDamaging()
    {
        BecomeDamagingEvent?.Invoke();
    }

    private void AttackEnded()
    {
        AttackEndedEvent?.Invoke();
    }
}
