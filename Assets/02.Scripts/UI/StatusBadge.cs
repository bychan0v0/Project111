using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusBadge : MonoBehaviour
{
    [SerializeField] private TMP_Text label;
    [SerializeField] private Image fill;
    [SerializeField] private Vector2 screenOffset = new Vector2(0, 24);

    RectTransform rt;
    Transform target;
    Vector3 worldOffset;
    Camera cam;
    
    float duration, remain;

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

        // 첫 프레임 위치 보정
        UpdatePositionImmediate();
    }

    private void LateUpdate()
    {
        if (target == null) { Destroy(gameObject); return; }

        // 위치 추적
        UpdatePositionImmediate();

        // 시간/게이지
        remain -= Time.deltaTime;
        if (fill) fill.fillAmount = Mathf.Clamp01(remain / duration);

        if (remain <= 0f)
            Destroy(gameObject);
    }

    private void UpdatePositionImmediate()
    {
        Vector3 worldPos = target.position + worldOffset;
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(cam, worldPos);
        rt.position = screenPos + screenOffset;
    }
}
