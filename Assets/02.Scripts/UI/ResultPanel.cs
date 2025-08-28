using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultPanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text resultText;

    void Awake()
    {
        panel.SetActive(false);
    }

    public void Show(string message)
    {
        resultText.text = message;
        panel.SetActive(true);
        
        Time.timeScale = 0f;
    }
}
