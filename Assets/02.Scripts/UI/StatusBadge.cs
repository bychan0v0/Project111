using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class StatusBadge : MonoBehaviour
{
    [SerializeField] private TMP_Text label;
    [SerializeField] private Image fill;

    public event Action OnDisposed;

    RectTransform rt;
    Transform target;
    Vector3 worldOffset;
    Camera cam;

    float duration, remain;
    float stackPixelYOffset = 0f;

    public void Setup(Transform target, Vector3 worldOffset, string text, float duration, Camera cam)
    {
        this.target = target;
        this.worldOffset = worldOffset;
        this.duration = Mathf.Max(0.0001f, duration);
        this.remain = this.duration;
        this.cam = cam ? cam : Camera.main;

        rt = (RectTransform)transform;
        if (label) label.text = text;
        if (fill)  fill.fillAmount = 1f;

        UpdatePositionImmediate();
    }

    // ★ HitUIRoot가 인덱스 바뀔 때마다 호출
    public void SetStackOffset(float pixelYOffset)
    {
        stackPixelYOffset = pixelYOffset;
        UpdatePositionImmediate();
    }

    void LateUpdate()
    {
        if (target == null) { Dispose(); return; }

        // 위치 추적
        UpdatePositionImmediate();

        // 시간/게이지
        remain -= Time.deltaTime;
        if (fill) fill.fillAmount = Mathf.Clamp01(remain / duration);

        if (remain <= 0f)
            Dispose();
    }

    void UpdatePositionImmediate()
    {
        if (!rt || !cam || target == null) return;
        Vector3 worldPos = target.position + worldOffset;
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(cam, worldPos);
        rt.position = screenPos + new Vector2(0f, stackPixelYOffset);
    }

    void Dispose()
    {
        OnDisposed?.Invoke();
        Destroy(gameObject);
    }
}