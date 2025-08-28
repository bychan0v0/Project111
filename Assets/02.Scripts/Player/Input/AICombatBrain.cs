using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICombatBrain : MonoBehaviour
{
    [Header("Core")]
    [SerializeField] private SkillManager skillManager;   // 같은 오브젝트/자식에 있을 것
    [SerializeField] private PlayerController player;     // 땅 체크용(선택)
    [SerializeField] private AIInput aiInput;             // 가끔 이동시킬 때만 사용(선택)

    [Header("Skill Rotation")]
    [SerializeField] private string[] rotation;
    [SerializeField, Min(0.05f)] private float pollInterval = 0.15f;

    [Header("Wander (optional)")]
    [SerializeField] private bool enableWander = true;
    [SerializeField] private Vector2 wanderIntervalRange = new Vector2(1.2f, 2.2f); // 다음 지시까지 대기 시간
    [SerializeField, Range(0f, 1f)] private float idleProbability = 0.3f;          // 멈추기 확률

    private int next;         // 다음에 시도할 스킬 인덱스
    private float pollT;      // 스킬 체크 타이머
    private float wanderT;    // 다음 이동 지시까지 시간

    private void Awake()
    {
        skillManager = GetComponentInChildren<SkillManager>();
        player       = GetComponent<PlayerController>();
        aiInput      = GetComponent<AIInput>();
        
        ResetWanderTimer();
    }

    private void Update()
    {
        if (skillManager.IsCasting) return;

        pollT -= Time.deltaTime;
        if (pollT <= 0f)
        {
            // 로드아웃을 그대로 돌며 사용 가능한 스킬 1개 시도
            bool used = skillManager.TryUseFirstReadyInLoadout();

            // 실패(전부 쿨타임)면 아무 것도 안 함 → 평소엔 AttackManager가 자동공격
            pollT = pollInterval;
        }

        // 2) 움직임: 스킬이 없거나(=평소) 가끔씩만 지시
        if (enableWander && aiInput && (!player || player.IsGround))
        {
            wanderT -= Time.deltaTime;
            if (wanderT <= 0f)
            {
                // -1(왼), 0(멈춤), +1(오) 중 하나를 랜덤 지시
                int dir = Random.value < idleProbability ? 0 : (Random.value < 0.5f ? -1 : +1);
                aiInput.SetDir(dir, duration: Random.Range(0.6f, 1.2f)); // 잠깐만 유지
                ResetWanderTimer();
            }
        }
    }

    private void ResetWanderTimer()
    {
        wanderT = Random.Range(wanderIntervalRange.x, wanderIntervalRange.y);
    }
}
