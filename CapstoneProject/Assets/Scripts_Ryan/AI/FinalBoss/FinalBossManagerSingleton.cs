using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class FinalBossManagerSingleton : MonoBehaviour
{
    public static FinalBossManagerSingleton Instance {get; private set;}

    [SerializeField] private GameObject EndingExplosionVFX;
    public Action InitializeFinalBoss;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitializeBoss()
    {
        InitializeFinalBoss?.Invoke();
    }

    public void OnFinalBossDeath()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerInput playerInput = player.GetComponent<PlayerInput>();
        CharacterController playerController = player.GetComponent<CharacterController>();
        playerController.enabled = false;
        playerInput.actions.FindActionMap("Player").Disable();

        CanvasGroup fade = GameObject.Find("Fade").GetComponent<CanvasGroup>();

        StartCoroutine(FadeOut(fade));
        StartCoroutine(PlayExplosionVFX());
    }

    private IEnumerator FadeOut(CanvasGroup fade)
    {
        while(fade.alpha < 1.0f)
        {
            fade.alpha += 0.1f;
            yield return new WaitForSeconds(0.5f);
        }
        SceneManager.LoadScene("WinScreen");
    }

    private IEnumerator PlayExplosionVFX()
    {
        while(true)
        {
            SpawnExplosions();
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.05f, 0.15f));
        }
    }

    private void SpawnExplosions()
    {
        Vector3 screenPos = new Vector3(UnityEngine.Random.Range(0, Screen.width), UnityEngine.Random.Range(0, Screen.height), 3f);
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

        Instantiate(EndingExplosionVFX, worldPos, Quaternion.identity);
    }
}
