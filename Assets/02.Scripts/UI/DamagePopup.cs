using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    [SerializeField] private TMP_Text label;
    [SerializeField] private float duration = 0.6f;
    [SerializeField] private float risePixels = 40f;
    [SerializeField] private AnimationCurve alpha = AnimationCurve.Linear(0,1,1,0);
    [SerializeField] private AnimationCurve scale = AnimationCurve.EaseInOut(0,0.9f,1,1f);

    RectTransform rt;
    CanvasGroup cg;
    Vector2 startScreenPos;
    float t;

    public void Setup(int amount, Vector2 screenPos)
    {
        rt = (RectTransform)transform;
        cg = GetComponent<CanvasGroup>();
        cg = gameObject.AddComponent<CanvasGroup>();

        startScreenPos = screenPos;
        rt.position = screenPos;

        label.text = amount.ToString();
        t = 0f;
    }

    private void Update()
    {
        t += Time.deltaTime;
        float p = Mathf.Clamp01(t / duration);

        rt.position = startScreenPos + Vector2.up * (risePixels * p);
        rt.localScale = Vector3.one * scale.Evaluate(p);
        cg.alpha = alpha.Evaluate(p);

        if (p >= 1f) Destroy(gameObject);
    }
}
