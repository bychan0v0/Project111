using System.Collections.Generic;
using UnityEngine;

public class HitUIRoot : MonoBehaviour
{
    public static HitUIRoot Instance { get; private set; }

    [Header("Prefabs")]
    [SerializeField] private DamagePopup popupPrefab;
    [SerializeField] private StatusBadge badgePrefab; // ← 머리 위 따라다니는 배지 스크립트
    [SerializeField] private Transform badgeContainer;      // (HUD용이면 사용)

    [Header("Stacking")]
    [SerializeField] private float stackPixelSpacing = 18f; // 배지 사이 간격(px)
    [SerializeField] private float basePixelYOffset = 24f;  // 머리 위 기본 오프셋(px)

    Camera cam;
    Canvas canvas;

    // 대상별로 현재 떠있는 배지 목록
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

    // ★ 머리 위 상태이상 표시(겹침 방지)
    public void ShowStatusOver(Transform target, string text, float duration, Vector3 worldOffset)
    {
        if (!badgePrefab || !target) return;

        // 1) 스택 확보/정리
        if (!stacks.TryGetValue(target, out var list) || list == null)
        {
            list = new List<StatusBadge>();
            stacks[target] = list;
        }
        // 죽은 참조 정리
        for (int i = list.Count - 1; i >= 0; i--)
            if (list[i] == null) list.RemoveAt(i);

        // 2) 새 배지 생성 + 인덱스 부여
        int index = list.Count;
        var ui = Instantiate(badgePrefab, canvas.transform);
        ui.Setup(target, worldOffset, text, duration, cam);

        // 기본 화면 y오프셋 + 스택 간격*인덱스 적용
        ui.SetStackOffset(basePixelYOffset + index * stackPixelSpacing);
        list.Add(ui);

        // 3) 배지가 종료될 때 스택에서 제거하고 재인덱싱
        ui.OnDisposed += () =>
        {
            if (!stacks.TryGetValue(target, out var l)) return;
            l.Remove(ui);
            // 재인덱싱: 아래로 당기기
            for (int i = 0; i < l.Count; i++)
                if (l[i] != null)
                    l[i].SetStackOffset(basePixelYOffset + i * stackPixelSpacing);
        };
    }
}
