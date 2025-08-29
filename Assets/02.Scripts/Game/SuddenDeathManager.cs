using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuddenDeathManager : MonoBehaviour
{
    public static SuddenDeathManager Instance;

    [Header("Sudden Death Options")]
    [SerializeField] private bool startArrowRain = true;
    [SerializeField] private float startDelay = 0.5f;

    private Coroutine co;
    public bool IsActive { get; private set; }

    private void Awake()
    {
        if (Instance != this && Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Begin()
    {
        if (IsActive) return;
        IsActive = true;

        if (co != null) StopCoroutine(co);
        co = StartCoroutine(BeginCo());
    }

    private IEnumerator BeginCo()
    {
        if (startDelay > 0f) yield return new WaitForSeconds(startDelay);

        if (startArrowRain) ArrowRainManager.Instance?.Begin();
    }

    public void End()
    {
        if (!IsActive) return;
        IsActive = false;

        if (co != null) StopCoroutine(co);
        co = null;

        ArrowRainManager.Instance?.Stop();
    }
}
