using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MeteorEventManager : MonoBehaviour
{
    public static MeteorEventManager Instance;
    
    [Header("UI")]
    [SerializeField] private Button btn;
    [SerializeField] private SpriteRenderer backgroundSprite;
    [SerializeField] private TMP_Text warningText; 

    [Header("Warning FX Settings")]
    [SerializeField] private float overlayTargetAlpha = 0.35f;
    [SerializeField] private float overlayFadeIn = 0.25f;
    [SerializeField] private float overlayFadeOut = 0.25f;
    [SerializeField] private float warningDuration = 2.0f;
    [SerializeField] private float blinkInterval = 0.35f;
    
    [Header("Meteor Settings")]
    [SerializeField] private GameObject meteorPrefab;
    [SerializeField] private GameObject dangerZonePrefab;
    [SerializeField] private Transform[] meteorTargets;   // 4 spots

    [Header("Layers")]
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private LayerMask groundMask;

    private bool triggered;
    private Coroutine warningCo;
    private Color bgOrigColor;   
    
    private void Awake()
    {
        Instance = this;
        
        bgOrigColor = backgroundSprite.color;
        var c = warningText.color;
        warningText.color = new Color(c.r, c.g, c.b, 0f);
        warningText.gameObject.SetActive(false);
    }

    public void TriggerEvent()
    {
        if (triggered) return;
        triggered = true;

        btn.gameObject.SetActive(true);
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(DropMeteor);
        
        backgroundSprite.color = bgOrigColor;
    }

    private void DropMeteor()
    {
        btn.gameObject.SetActive(false);

        if (warningCo != null) StopCoroutine(warningCo);
        warningCo = StartCoroutine(PlayWarningFX());
        
        int idx = Random.Range(0, meteorTargets.Length);
        Transform target = meteorTargets[idx];

        // Danger Zone ����
        GameObject zone = Instantiate(dangerZonePrefab, target.position, Quaternion.identity);
        var zoneCol = zone.GetComponent<BoxCollider2D>(); // isTrigger = true �ʿ�

        // ���׿� ����
        GameObject meteorObj = Instantiate(meteorPrefab, target.position + Vector3.up * 15f, Quaternion.identity);
        var meteor = meteorObj.GetComponent<Meteor>();

        // zone �ð� ���Ŵ� Meteor�� �浹 �� ����
        meteor.Init(
            targetPos: target.position,
            zoneCollider: zoneCol,
            playerMask: playerMask,
            groundMask: groundMask,
            onAfterImpact: () => Destroy(zone)
        );
    }
    
    private IEnumerator PlayWarningFX()
    {
         if (!backgroundSprite && !warningText) yield break;

        // ��ǥ ƾƮ��: ������ ������ overlayTargetAlpha ������ ���� ��
        Color targetTint = backgroundSprite
            ? Color.Lerp(bgOrigColor, Color.red, Mathf.Clamp01(overlayTargetAlpha))
            : Color.red;

        // Warning �ؽ�Ʈ Ȱ��ȭ(���� 0���� ����)
        if (warningText)
        {
            var c0 = warningText.color;
            warningText.color = new Color(c0.r, c0.g, c0.b, 0f);
            warningText.gameObject.SetActive(true);
        }

        // ��� ���̵� ��(��������� ƾƮ)
        float t = 0f;
        while (t < overlayFadeIn)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / overlayFadeIn);
            if (backgroundSprite)
                backgroundSprite.color = Color.Lerp(bgOrigColor, targetTint, k);

            // �ؽ�Ʈ�� ������ ���̱�
            if (warningText)
            {
                var c = warningText.color;
                warningText.color = new Color(c.r, c.g, c.b, k);
            }
            yield return null;
        }

        // ������ ����
        float elapsed = 0f, blinkT = 0f;
        while (elapsed < warningDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            blinkT  += Time.unscaledDeltaTime;

            if (warningText)
            {
                bool on = (Mathf.FloorToInt(blinkT / blinkInterval) % 2) == 0;
                var c = warningText.color;
                warningText.color = new Color(c.r, c.g, c.b, on ? 1f : 0f);
            }
            yield return null;
        }

        // ���̵� �ƿ�(��� ���� + �ؽ�Ʈ ����)
        t = 0f;
        float startA = warningText ? warningText.color.a : 0f;
        while (t < overlayFadeOut)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / overlayFadeOut);

            if (backgroundSprite)
                backgroundSprite.color = Color.Lerp(targetTint, bgOrigColor, k);

            if (warningText)
            {
                float a = Mathf.Lerp(startA, 0f, k);
                var c = warningText.color;
                warningText.color = new Color(c.r, c.g, c.b, a);
            }
            yield return null;
        }

        warningText.gameObject.SetActive(false);
        warningCo = null;
    }
}