using System.Collections;
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

    [Header("Sudden Death")]
    [SerializeField] private bool useSuddenDeath = true;
    [SerializeField] private string suddenDeathLabel = "SD";

    private bool ended;
    private bool started;
    private bool suddenDeathActive;
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

        // 에러 방지용 정리
        if (suddenDeathActive)
        {
            SuddenDeathManager.Instance?.End();
            suddenDeathActive = false;
        }
    }

    public void StartMatch()
    {
        if (started) return;
        started = true;
        ended = false;
        suddenDeathActive = false;

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

            // 네가 쓰는 메테오 트리거 유지
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
            if (useSuddenDeath)
            {
                suddenDeathActive = true;
                if (timerText) timerText.text = suddenDeathLabel;
                SuddenDeathManager.Instance?.Begin();

                yield break;
            }

            var winner = DecideByHp();
            StartCoroutine(ShowResultAfter(winner));
            ended = true;
        }
    }

    private void UpdateTimerUI(float sec)
    {
        int s = Mathf.CeilToInt(sec);
        if (timerText) timerText.text = $"{s}";
    }

    private PlayerHp DecideByHp()
    {
        int lh = left  ? left.CurrentHP  : 0;
        int rh = right ? right.CurrentHP : 0;
        if (lh > rh) return left;
        if (rh > lh) return right;
        return null;
    }

    private void OnLeftDied()  => EndByDeath(loserAnim: leftAnim,  winner: right);
    private void OnRightDied() => EndByDeath(loserAnim: rightAnim, winner: left);

    private void EndByDeath(Animator loserAnim, PlayerHp winner)
    {
        if (ended) return;
        ended = true;

        if (timerCo != null) StopCoroutine(timerCo);

        if (suddenDeathActive)
        {
            SuddenDeathManager.Instance?.End();
            suddenDeathActive = false;
        }

        if (loserAnim && !string.IsNullOrEmpty(dieTrigger))
        {
            loserAnim.SetTrigger(dieTrigger);
        }
        
        StartCoroutine(ShowResultAfter(winner));
    }

    private IEnumerator ShowResultAfter(PlayerHp winner)
    {
        yield return new WaitForSecondsRealtime(showAfterSeconds);

        if (resultPanel) resultPanel.SetActive(true);
        if (resultText)  resultText.text = winner ? $"{winner.name} Win!" : "Draw!";

        Time.timeScale = 0f;
    }
}
