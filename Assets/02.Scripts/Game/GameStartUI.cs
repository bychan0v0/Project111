using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameStartUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Button startButton;
    [SerializeField] private TMP_Text countdownText;

    [Header("Options")]
    [SerializeField] private int countdownSeconds = 3;
    [SerializeField] private float goFlashTime = 0.6f;

    private bool started;

    private void Awake()
    {
        panel.SetActive(true);
        countdownText.gameObject.SetActive(false);
        startButton.onClick.AddListener(OnStartClicked);

        // 게임 정지 (카운트다운은 Realtime으로 진행)
        Time.timeScale = 0f;
    }

    private void OnStartClicked()
    {
        if (started) return;
        started = true;
        startButton.interactable = false;
        StartCoroutine(CoCountdown());
    }

    private IEnumerator CoCountdown()
    {
        panel.SetActive(false);
        countdownText.gameObject.SetActive(true);

        for (int i = countdownSeconds; i >= 1; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSecondsRealtime(1f);
        }

        countdownText.text = "GO!";
        yield return new WaitForSecondsRealtime(goFlashTime);

        countdownText.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }
}
