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

    [Header("Match Timer")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private float matchDuration = 90f;
    [SerializeField] private float meteorTime = 90f;

    private bool ended;
    private bool started;
    private float remain;
    private Coroutine timerCo;

    private void OnEnable()
    {
        left.OnDied  += OnLeftDied;
        right.OnDied += OnRightDied;
    }

    private void OnDisable()
    {
        left.OnDied  -= OnLeftDied;
        right.OnDied -= OnRightDied;
    }

    // === GameStartUI에서 카운트다운 끝나면 이걸 호출 ===
    public void StartMatch()
    {
        if (started) return;
        started = true;
        ended = false;

        remain = matchDuration;
        UpdateTimerUI(remain);

        if (timerCo != null) StopCoroutine(timerCo);
        timerCo = StartCoroutine(TimerLoop());
    }

    private IEnumerator TimerLoop()
    {
        while (!ended && remain > 0f)
        {
            remain -= Time.deltaTime;
            if (remain <= meteorTime)
            {
                MeteorEventManager.Instance.TriggerEvent();
            }
            if (remain < 0f) remain = 0f;
            UpdateTimerUI(remain);
            yield return null;
        }

        if (!ended)
        {
            // 시간 만료: 체력 비교로 승자 결정
            var winner = DecideByHp();
            StartCoroutine(ShowResultAfter(winner));
            ended = true;
        }
    }

    private void UpdateTimerUI(float sec)
    {
        int s = Mathf.CeilToInt(sec);
        timerText.text = $"{s}";
    }

    private PlayerHp DecideByHp()
    {
        int lh = left  ? left.CurrentHP  : 0;
        int rh = right ? right.CurrentHP : 0;
        if (lh > rh) return left;
        if (rh > lh) return right;
        return null;
    }

    private void OnLeftDied()  => EndByDeath(loserAnim:leftAnim,  winner:right);
    private void OnRightDied() => EndByDeath(loserAnim:rightAnim, winner:left);

    private void EndByDeath(Animator loserAnim, PlayerHp winner)
    {
        if (ended) return;
        ended = true;

        if (timerCo != null) StopCoroutine(timerCo);

        // 패배 애니만 트리거(간단 유지)
        if (loserAnim && !string.IsNullOrEmpty(dieTrigger))
        {
            loserAnim.SetTrigger(dieTrigger);
        }
        

        StartCoroutine(ShowResultAfter(winner));
    }

    private IEnumerator ShowResultAfter(PlayerHp winner)
    {
        yield return new WaitForSecondsRealtime(showAfterSeconds);

        resultPanel.SetActive(true);
        resultText.text = winner ? $"{winner.name} Win!" : "Draw!";

        // 완전 정지 필요하면 여기서 정지
        Time.timeScale = 0f;
    }
}
