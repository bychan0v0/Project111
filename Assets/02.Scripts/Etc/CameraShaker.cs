using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
     public static CameraShaker Instance;

    [SerializeField] private Transform cam;
    [SerializeField] private float defaultFreq = 18f;

    private Vector3 origLocalPos;
    private Coroutine loopCo;

    private void Awake()
    {
        Instance = this;
        if (!cam) cam = Camera.main ? Camera.main.transform : transform;
        origLocalPos = cam.localPosition;
    }

    public void Begin(float amplitude = 0.15f, float freq = -1f)
    {
        if (freq <= 0f) freq = defaultFreq;
        Stop();
        loopCo = StartCoroutine(ShakeLoop(amplitude, freq));
    }

    public void Stop()
    {
        if (loopCo != null)
        {
            StopCoroutine(loopCo);
            loopCo = null;
        }
        if (cam) cam.localPosition = origLocalPos;
    }

    public void OneShot(float amplitude = 0.5f, float duration = 0.2f, float freq = -1f)
    {
        if (freq <= 0f) freq = defaultFreq;
        StartCoroutine(OneShotCo(amplitude, duration, freq));
    }

    private IEnumerator ShakeLoop(float amp, float freq)
    {
        while (true)
        {
            if (!cam) yield break;
            float x = (Mathf.PerlinNoise(Time.time * freq, 0f) - 0.5f) * 2f * amp;
            float y = (Mathf.PerlinNoise(0f, Time.time * freq) - 0.5f) * 2f * amp;
            cam.localPosition = origLocalPos + new Vector3(x, y, 0f);
            yield return null;
        }
    }

    private IEnumerator OneShotCo(float amp, float dur, float freq)
    {
        float t = 0f;
        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            float k = 1f - (t / dur); // ¼­¼­È÷ °¨¼è
            float x = (Mathf.PerlinNoise(Time.time * freq, 0f) - 0.5f) * 2f * amp * k;
            float y = (Mathf.PerlinNoise(0f, Time.time * freq) - 0.5f) * 2f * amp * k;
            if (cam) cam.localPosition = origLocalPos + new Vector3(x, y, 0f);
            yield return null;
        }
        if (loopCo == null && cam) cam.localPosition = origLocalPos;
    }
}
