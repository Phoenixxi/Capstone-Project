using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Ensure we can access the namespace where ElementType etc. might be defined, just in case.
using lilGuysNamespace;

public class AcidZone : MonoBehaviour
{
    // ==========================================
    // Configuration
    // ==========================================

    [Header("Damage Settings")]
    [Tooltip("Amount of damage dealt to the player per interval.")]
    public float damageAmount = 5.0f; // Set to 5 as requested

    [Tooltip("Time in seconds between damage ticks.")]
    public float damageInterval = 1.0f;

    [Header("Visual Effects")]
    [Tooltip("The particle effect prefab to spawn when player enters acid.")]
    public GameObject bubblePrefab;

    [Tooltip("The color to tint the player when inside acid.")]
    public Color acidColor = new Color(0.2f, 1f, 0.2f, 1f); // Light Green

    // ==========================================
    // Internal State
    // ==========================================

    // Tracks the next time damage should be dealt for each specific player object
    private Dictionary<GameObject, float> nextDamageTimes = new Dictionary<GameObject, float>();

    // Stores the original color of the player to restore it later
    private Dictionary<GameObject, Color> originalColors = new Dictionary<GameObject, Color>();

    // ==========================================
    // Unity Events
    // ==========================================

    // Using OnTriggerStay to handle character swapping (Q/E) inside the pool correctly
    private void OnTriggerStay(Collider other)
    {
        // Only interact with objects tagged as "Player"
        if (other.CompareTag("Player"))
        {
            GameObject playerObj = other.gameObject;

            // --- 1. Visual: Handle Color Change ---
            // Ensures the player turns green even if they swapped characters inside the pool
            HandleColorChange(playerObj);

            // --- 2. Logic: Handle Damage ---
            HandleDamage(playerObj);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject playerObj = other.gameObject;

            // Restore the player's original color
            RestoreColor(playerObj);

            // Destroy bubble effects (looking for clones created by this script)
            var bubbles = playerObj.GetComponentsInChildren<ParticleSystem>();
            foreach (var b in bubbles)
            {
                if (b.name.Contains("Clone")) Destroy(b.gameObject);
            }

            // Clean up damage timer
            if (nextDamageTimes.ContainsKey(playerObj))
            {
                nextDamageTimes.Remove(playerObj);
            }
        }
    }

    // ==========================================
    // Core Logic Methods
    // ==========================================

    private void HandleDamage(GameObject playerObj)
    {
        // Initialize timer if not present
        if (!nextDamageTimes.ContainsKey(playerObj))
        {
            nextDamageTimes[playerObj] = Time.time;
        }

        // Check if it is time to deal damage
        if (Time.time >= nextDamageTimes[playerObj])
        {
            // CRITICAL FIX: Look for EntityManager on the object OR its parent
            // This fixes issues where the Collider is on a child object
            EntityManager entity = playerObj.GetComponent<EntityManager>();
            if (entity == null)
            {
                entity = playerObj.GetComponentInParent<EntityManager>();
            }

            if (entity != null)
            {
                // Call the specific TakeDamage method from your provided EntityManager script
                entity.TakeDamage(damageAmount);

                // Debug log to confirm it's working (remove later if too noisy)
                Debug.Log($"Acid dealt {damageAmount} damage to {entity.name}");
            }
            else
            {
                // If this logs, the player object is missing the EntityManager script
                Debug.LogError($"AcidZone found a Player tag on {playerObj.name}, but could not find 'EntityManager' script on it or its parent!");
            }

            // Reset the timer for the next tick
            nextDamageTimes[playerObj] = Time.time + damageInterval;
        }
    }

    private void HandleColorChange(GameObject playerObj)
    {
        Renderer[] renderers = playerObj.GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            // Ignore particle system renderers
            if (r != null && !(r is ParticleSystemRenderer))
            {
                // If the color hasn't been changed to acid color yet
                if (r.material.color != acidColor)
                {
                    // Store original color if not already stored
                    if (!originalColors.ContainsKey(playerObj))
                        originalColors[playerObj] = r.material.color;

                    // Apply acid color
                    r.material.color = acidColor;

                    // Spawn bubbles if not present
                    if (playerObj.GetComponentInChildren<ParticleSystem>() == null && bubblePrefab != null)
                    {
                        GameObject bubble = Instantiate(bubblePrefab, playerObj.transform.position, Quaternion.identity);
                        bubble.transform.SetParent(playerObj.transform);
                        bubble.transform.localPosition = new Vector3(0, 1.0f, 0);
                    }
                }
            }
        }
    }

    private void RestoreColor(GameObject playerObj)
    {
        if (originalColors.ContainsKey(playerObj))
        {
            Renderer[] renderers = playerObj.GetComponentsInChildren<Renderer>();
            foreach (var r in renderers)
            {
                if (r != null && !(r is ParticleSystemRenderer))
                    r.material.color = originalColors[playerObj];
            }
            originalColors.Remove(playerObj);
        }
    }
}