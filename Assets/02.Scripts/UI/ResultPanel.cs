using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;   

public class ResultPanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text resultText;

    [SerializeField] private Button retryButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        panel.SetActive(false);

        if (retryButton) retryButton.onClick.AddListener(OnClickRetry);
        if (quitButton)  quitButton.onClick.AddListener(OnClickQuit);
    }

    public void Show(string message)
    {
        resultText.text = message;
        panel.SetActive(true);

        Time.timeScale = 0f;
    }

    private void OnClickRetry()
    {
        Time.timeScale = 1f;
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }

    private void OnClickQuit()
    {
        Time.timeScale = 1f;
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}