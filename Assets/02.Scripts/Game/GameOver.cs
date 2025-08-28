using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    [Header("Players")]
    [SerializeField] private PlayerHp left;
    [SerializeField] private PlayerHp right;
    [SerializeField] private Animator leftAnim;
    [SerializeField] private Animator rightAnim;

    [Header("Anim Triggers")]
    [SerializeField] private string dieTrigger = "Die";

    [Header("Result UI")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private float showAfterSeconds = 1.0f;

    private bool ended;

    private void OnEnable()
    {
        left.OnDied  += OnLeftDied;
        right.OnDied += OnRightDied;
    }

    private void OnLeftDied()  => End(lAnim:leftAnim,  winner:right);
    private void OnRightDied() => End(lAnim:rightAnim, winner:left);

    private void End(Animator lAnim, PlayerHp winner )
    {
        if (ended) return;
        ended = true;

        lAnim.SetTrigger(dieTrigger);
        StartCoroutine(ShowResultAfter(winner));
    }

    private IEnumerator ShowResultAfter(PlayerHp winner)
    {
        // 게임 로직은 계속 돌되(애니 재생을 위해) UI만 조금 늦게
        yield return new WaitForSecondsRealtime(showAfterSeconds);

        resultPanel.SetActive(true);
        resultText.text = $"{winner.name} Win!";

        // 완전 정지 원하면 여기서 멈추기
        Time.timeScale = 0f;
    }
}
