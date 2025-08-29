using System.Collections.Generic;
using UnityEngine;

public class HitUIRoot : MonoBehaviour
{
    public static HitUIRoot Instance { get; private set; }

    [Header("Prefabs")]
    [SerializeField] private DamagePopup popupPrefab;
    [SerializeField] private StatusBadge badgePrefab;
    [SerializeField] private Transform badgeContainer;

    [Header("Stacking")]
    [SerializeField] private float stackPixelSpacing = 18f;
    [SerializeField] private float basePixelYOffset = 24f;

    Camera cam;
    Canvas canvas;

    private readonly Dictionary<Transform, List<StatusBadge>> stacks = new();

    private void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        canvas = GetComponentInParent<Canvas>() ?? GetComponent<Canvas>();
        cam = Camera.main;
    }

    public void ShowDamage(int amount, Vector3 worldPos)
    {
        if (!popupPrefab) return;
        var pop = Instantiate(popupPrefab, canvas.transform);
        Vector2 screen = RectTransformUtility.WorldToScreenPoint(cam, worldPos);
        pop.Setup(amount, screen);
    }

    public void ShowStatusOver(Transform target, string text, float duration, Vector3 worldOffset)
    {
        if (!badgePrefab || !target) return;

        if (!stacks.TryGetValue(target, out var list) || list == null)
        {
            list = new List<StatusBadge>();
            stacks[target] = list;
        }
        for (int i = list.Count - 1; i >= 0; i--)
            if (list[i] == null) list.RemoveAt(i);

        int index = list.Count;
        var ui = Instantiate(badgePrefab, canvas.transform);
        ui.Setup(target, worldOffset, text, duration, cam);

        ui.SetStackOffset(basePixelYOffset + index * stackPixelSpacing);
        list.Add(ui);

        ui.OnDisposed += () =>
        {
            if (!stacks.TryGetValue(target, out var l)) return;
            l.Remove(ui);
            for (int i = 0; i < l.Count; i++)
                if (l[i] != null)
                    l[i].SetStackOffset(basePixelYOffset + i * stackPixelSpacing);
        };
    }
}
