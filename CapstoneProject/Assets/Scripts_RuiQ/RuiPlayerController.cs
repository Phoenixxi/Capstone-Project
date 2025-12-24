using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using lilGuysNamespace;
using UnityEngine.SceneManagement;
using System;

// ⚠️ 必须确保文件名是: RuiQPlayerController.cs
public class RuiQPlayerController : MonoBehaviour
{
    [Header("Essentials: Do Not Change!")]
    [SerializeField] private GameObject aimDirection;
    [SerializeField] private SwappingManager swappingManager;
    [SerializeField] private LayerMask aimLayerMask;
    [SerializeField] public CheckpointController checkpointController;
    [SerializeField] private UIPlayerSwap uiPlayerSwap;

    [Header("Character Swapping VFX")]
    [SerializeField] private List<GameObject> switchOffVFXList;

    private Vector2 movementInput;
    private Vector3 mousePosition;
    private CharacterController player; // 这个是 Unity 自带的 CharacterController组件
    private Camera playerCamera;
    private List<GameObject> charactersListPC;

    // 引用你的新 RuiEntityManager
    private RuiEntityManager currentCharacterEntity;

    private int currentCharacterIndex;
    private bool isAttacking;
    private bool canMove = true;

    public Action<int> CharacterSwapEvent;
    public Action<bool> AbilityScreenPressedEvent;
    public Action Pause;

    public void Start()
    {
        // 自动获取摄像机，防呆设计
        if (playerCamera == null) playerCamera = Camera.main;

        charactersListPC = swappingManager.charactersList;
        currentCharacterIndex = 0;

        if (switchOffVFXList == null || switchOffVFXList.Count < charactersListPC.Count)
        {
            Debug.LogWarning("VFX List is empty or smaller than character list!");
        }

        swappingManager.DeathSwapEvent += OnCharacterSwapForced;

        foreach (GameObject character in charactersListPC)
        {
            // 使用 RuiEntityManager 初始化血量
            if (character.GetComponent<RuiEntityManager>())
                character.GetComponent<RuiEntityManager>().SetHealthToFull();
        }

        if (checkpointController == null)
            Debug.LogError("Checkpoint Controller not set in Player Controller on player");

        UpdateCurrentCharacterReference();
    }

    private void Awake()
    {
        player = GetComponent<CharacterController>();
        playerCamera = FindFirstObjectByType<Camera>();

        if (swappingManager.GetCurrentCharacterTransform() != null)
            currentCharacterEntity = swappingManager.GetCurrentCharacterTransform().GetComponent<RuiEntityManager>();
    }

    private void OnEnable()
    {
        swappingManager.DeathSwapEvent -= OnCharacterSwapForced;
        swappingManager.DeathSwapEvent += OnCharacterSwapForced;
    }

    private void OnDisable()
    {
        swappingManager.DeathSwapEvent -= OnCharacterSwapForced;
    }

    private void UpdateCurrentCharacterReference()
    {
        if (charactersListPC != null && charactersListPC.Count > currentCharacterIndex)
        {
            currentCharacterEntity = charactersListPC[currentCharacterIndex].GetComponent<RuiEntityManager>();
        }
    }

    #region Health Management
    public void HealAllCharacters(float healAmount)
    {
        if (uiPlayerSwap != null) uiPlayerSwap.AllPlayersHealed();

        foreach (GameObject character in charactersListPC)
        {
            RuiEntityManager entity = character.GetComponentInChildren<RuiEntityManager>();
            if (entity != null) entity.Heal(healAmount);
        }
    }

    public void TakeDamageWrapper(float damage)
    {
        if (currentCharacterEntity != null) currentCharacterEntity.TakeDamage(damage);
    }

    public void HealActiveCharacter(float healAmount)
    {
        if (currentCharacterEntity != null) currentCharacterEntity.Heal(healAmount);
    }
    #endregion

    #region Input Handlers
    private void OnMove(InputValue input)
    {
        if (!canMove) return;
        movementInput = input.Get<Vector2>().normalized;

        // 把输入传递给 RuiEntityManager
        if (currentCharacterEntity) currentCharacterEntity.SetInputDirection(movementInput);
    }

    private void OnJump()
    {
        if (!canMove) return;
        if (currentCharacterEntity) currentCharacterEntity.Jump();
    }

    private void OnAttack(InputValue input)
    {
        if (!canMove) return;
        bool pressed = input.Get<float>() > 0.5f;
        isAttacking = pressed;
    }

    private void OnAbility()
    {
        if (!canMove) return;
        if (currentCharacterEntity) currentCharacterEntity.UseAbility(movementInput);
    }

    private void OnAbilityScreen(InputValue input)
    {
        float pressedValue = input.Get<float>();
        AbilityScreenPressedEvent?.Invoke(pressedValue == 1f);
    }

    private void OnPause(InputValue input)
    {
        Pause?.Invoke();
    }
    #endregion

    #region Aiming
    private void UpdateMouseAim()
    {
        if (Mouse.current == null || playerCamera == null || aimDirection == null) return;

        // --- 修复：确保箭头稍微浮在角色前面，防止被遮挡 ---
        if (aimDirection.transform.localPosition.z != -0.5f)
        {
            Vector3 currentLocal = aimDirection.transform.localPosition;
            aimDirection.transform.localPosition = new Vector3(currentLocal.x, currentLocal.y, -0.5f);
        }
        // ------------------------------------------------

        Vector2 mouseScreenPos = Mouse.current.position.value;
        Ray ray = playerCamera.ScreenPointToRay(mouseScreenPos);
        RaycastHit hit;

        if (Physics.Raycast(ray: ray, hitInfo: out hit, maxDistance: Mathf.Infinity, layerMask: aimLayerMask))
        {
            mousePosition = hit.point;

            // 注意：这里强制把目标高度设为和箭头一样高，适合 Top-Down 视角
            // 如果你是侧视平台跳跃游戏，这行可能需要调整
            mousePosition.y = aimDirection.transform.position.y;

            aimDirection.transform.LookAt(mousePosition);
        }
    }
    #endregion

    #region Character Swapping Logic
    private void OnSwapCharacter(InputValue input)
    {
        if (!canMove) return;

        float pressedValue = input.Get<float>();
        int direction = (int)pressedValue;

        if (direction == 0) return;

        if (uiPlayerSwap != null)
        {
            uiPlayerSwap.swapImageLocation(direction);
        }
        else
        {
            Debug.LogError("uiPlayerSwap not set");
        }

        int checkIndex = currentCharacterIndex;
        int attempts = 0;
        int maxAttempts = charactersListPC.Count;

        do
        {
            checkIndex += direction;
            if (checkIndex < 0) checkIndex = charactersListPC.Count - 1;
            else if (checkIndex >= charactersListPC.Count) checkIndex = 0;
            attempts++;
        } while (!charactersListPC[checkIndex].GetComponent<RuiEntityManager>().isAlive && attempts < maxAttempts);

        if (attempts >= maxAttempts && !charactersListPC[checkIndex].GetComponent<RuiEntityManager>().isAlive)
        {
            Debug.LogWarning("All characters are dead! Cannot swap.");
            return;
        }

        if (checkIndex != currentCharacterIndex)
        {
            OnCharacterSwapForced(checkIndex + 1);
        }
    }

    private void OnCharacterSwapForced(int characterNum)
    {
        int targetIndex = characterNum - 1;
        if (targetIndex < 0 || targetIndex >= charactersListPC.Count) return;

        if (uiPlayerSwap != null)
        {
            if (characterNum == 1) uiPlayerSwap.stateOne();
            else if (characterNum == 2) uiPlayerSwap.stateTwo();
            else if (characterNum == 3) uiPlayerSwap.stateThree();
        }

        PerformCharacterSwap(targetIndex);
        CharacterSwapEvent?.Invoke(characterNum);
    }

    private void PerformCharacterSwap(int targetIndex)
    {
        if (currentCharacterEntity != null && currentCharacterEntity.AbilityInUse()) return;

        GameObject targetCharObj = charactersListPC[targetIndex];
        RuiEntityManager targetEntity = targetCharObj.GetComponent<RuiEntityManager>();

        if (!targetEntity.isAlive)
        {
            Debug.Log($"{targetCharObj.name} is dead and cannot be swapped to.");
            return;
        }

        if (switchOffVFXList != null && currentCharacterIndex < switchOffVFXList.Count)
        {
            GameObject vfxPrefab = switchOffVFXList[currentCharacterIndex];
            if (vfxPrefab != null)
            {
                Instantiate(vfxPrefab, transform.position, Quaternion.identity);
            }
        }

        for (int i = 0; i < charactersListPC.Count; i++)
        {
            if (i == targetIndex) charactersListPC[i].SetActive(true);
            else charactersListPC[i].SetActive(false);
        }

        if (currentCharacterEntity != null)
        {
            targetEntity.SetMovementVelocity(currentCharacterEntity.GetMovementVelocity());
        }

        currentCharacterEntity = targetEntity;
        currentCharacterIndex = targetIndex;
    }
    #endregion

    public void SetCanMove(bool value)
    {
        canMove = value;
        movementInput = Vector2.zero;
        if (currentCharacterEntity) currentCharacterEntity.SetInputDirection(Vector2.zero);
    }

    private void Update()
    {
        UpdateMouseAim();

        if (isAttacking && currentCharacterEntity != null)
        {
            // 传递攻击指令给 EntityManager
            currentCharacterEntity.Attack(aimDirection.transform.forward, transform.position);
        }

        if (transform.position.y <= -22)
        {
            SendToCheckpoint();
        }
    }

    public void SendToCheckpoint()
    {
        if (checkpointController != null)
        {
            Vector3 location = checkpointController.RecentCheckpointLocation();
            if (player != null) player.enabled = false;
            transform.position = location;
            if (player != null) player.enabled = true;
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}