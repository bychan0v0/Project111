using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitUIRoot : MonoBehaviour
{
    public static HitUIRoot Instance { get; private set; }

    [Header("Prefabs")]
    [SerializeField] private DamagePopup popupPrefab;  // �Ʒ� ��ũ��Ʈ
    [SerializeField] private StatusBadge badgePrefab;  // �Ʒ� ��ũ��Ʈ
    [SerializeField] private Transform badgeContainer; // HUD ���� ���� �г�

    Camera cam;
    Canvas canvas;

    private void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        canvas = GetComponentInParent<Canvas>() ?? GetComponent<Canvas>();
        cam = Camera.main;
    }

    public void ShowDamage(int amount, Vector3 worldPos, bool isCrit = false)
    {
        if (!popupPrefab) return;
        var pop = Instantiate(popupPrefab, canvas.transform);
        Vector2 screen = RectTransformUtility.WorldToScreenPoint(cam, worldPos);
        pop.Setup(amount, isCrit, screen);
    }

    public void ShowStatus(string text, float duration)
    {
        if (!badgePrefab || !badgeContainer) return;
        var badge = Instantiate(badgePrefab, badgeContainer);
        badge.Setup(text, duration);
    }
}
