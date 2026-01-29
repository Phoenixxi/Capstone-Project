using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 确保引用你的命名空间
using lilGuysNamespace;
using System;

public class AcidZone : MonoBehaviour
{
    // ==========================================
    // 配置区域
    // ==========================================

    [Header("Damage Settings")]
    public float damageAmount = 5.0f;
    public float damageInterval = 1.0f;

    [Header("Visual Effects")]
    public GameObject bubblePrefab;
    public Color acidColor = new Color(0.2f, 1f, 0.2f, 1f); // 酸液颜色（绿）

    // ==========================================
    // 内部状态
    // ==========================================
    private Dictionary<GameObject, float> nextDamageTimes = new Dictionary<GameObject, float>();

    // 我们不再需要记录原始颜色了，因为我们决定离开时总是变回白色
    // private Dictionary<GameObject, Color> originalColors = new Dictionary<GameObject, Color>();
    // 但为了逻辑兼容，我们只用来记录"谁变色了"，防止重复变色
    private HashSet<GameObject> tintedObjects = new HashSet<GameObject>();

    private Dictionary<GameObject, IEnumerator> activeCoroutines = new Dictionary<GameObject, IEnumerator>();

    // ==========================================
    // 核心逻辑
    // ==========================================

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject playerObj = other.gameObject;

            // 1. 视觉：变绿 (哪怕是隐藏的物体也要变)
            HandleColorChange(playerObj);

            // 尝试在自身、父物体、子物体上找 EntityManager
            EntityManager entity = playerObj.GetComponent<EntityManager>();
            if (entity == null) entity = playerObj.GetComponentInParent<EntityManager>();
            if (entity == null) entity = playerObj.GetComponentInChildren<EntityManager>();

            // 2. 逻辑：开始扣血协程 (参考 BuffZone 逻辑)
            if (entity != null && !activeCoroutines.ContainsKey(entity.gameObject))
            {

                IEnumerator damageRoutine = DamagePlayerCoroutine(entity);
                activeCoroutines.Add(entity.gameObject, damageRoutine);
                StartCoroutine(damageRoutine);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject playerObj = other.gameObject;

            // 1. 视觉：强制变回白色 (Fix Blue Issue)
            RestoreColor(playerObj);
            EntityManager entity = playerObj.GetComponentInChildren<EntityManager>();
            // 2. 逻辑：停止扣血
            if (activeCoroutines.ContainsKey(entity.gameObject))
            {
                // 移除后，协程里的 while 循环条件失效，自动停止
                activeCoroutines.Remove(entity.gameObject);
            }
        }
    }

    // ==========================================
    // 协程逻辑
    // ==========================================

    private IEnumerator DamagePlayerCoroutine(EntityManager entity)
    {
        // 只要玩家还在字典里且不为空，就一直扣血
        while (entity != null && entity.isActiveAndEnabled && activeCoroutines.ContainsKey(entity.gameObject))
        {
            entity.TakeDamage(damageAmount, EntityData.ElementType.Normal);
            // Debug.Log($"Acid dealt {damageAmount} damage to {entity.name}");
            yield return new WaitForSeconds(damageInterval);
        }
        activeCoroutines.Remove(entity.gameObject);
    }

    // ==========================================
    // 视觉辅助函数 (核心修改在 RestoreColor)
    // ==========================================

    private void HandleColorChange(GameObject playerObj)
    {
        // 这里的 (true) 很重要，确保即使切换角色导致物体隐藏，也能被找到
        Renderer[] renderers = playerObj.GetComponentsInChildren<Renderer>(true);

        foreach (var r in renderers)
        {
            // 排除粒子特效，只变身子
            if (r != null && !(r is ParticleSystemRenderer))
            {
                // 如果还没变色
                if (!tintedObjects.Contains(playerObj) || r.material.color != acidColor)
                {
                    // 标记该物体已变色
                    tintedObjects.Add(playerObj);

                    // 变绿
                    r.material.color = acidColor;

                    // 生成泡泡
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
        // 同样的 (true)，确保能找到那个被隐藏的旧角色
        Renderer[] renderers = playerObj.GetComponentsInChildren<Renderer>(true);

        foreach (var r in renderers)
        {
            if (r != null && !(r is ParticleSystemRenderer))
            {
                // 【核心修复】：不要试图还原旧颜色，直接强制变为白色！
                // 在 Unity 中，材质球的默认颜色（让贴图正常显示的颜色）永远是白色。
                // 这解决了"变成蓝色"的问题。
                r.material.color = Color.white;
            }
        }

        // 销毁生成的泡泡
        var bubbles = playerObj.GetComponentsInChildren<ParticleSystem>(true);
        foreach (var b in bubbles)
        {
            if (b.name.Contains("Clone")) Destroy(b.gameObject);
        }

        // 移除记录
        tintedObjects.Remove(playerObj);
    }
}