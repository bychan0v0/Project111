using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICombatBrain : MonoBehaviour
{
    [Header("Core")]
    [SerializeField] private SkillManager skillManager;   // ���� ������Ʈ/�ڽĿ� ���� ��
    [SerializeField] private PlayerController player;     // �� üũ��(����)
    [SerializeField] private AIInput aiInput;             // ���� �̵���ų ���� ���(����)

    [Header("Skill Rotation")]
    [SerializeField] private string[] rotation;
    [SerializeField, Min(0.05f)] private float pollInterval = 0.15f;

    [Header("Wander (optional)")]
    [SerializeField] private bool enableWander = true;
    [SerializeField] private Vector2 wanderIntervalRange = new Vector2(1.2f, 2.2f); // ���� ���ñ��� ��� �ð�
    [SerializeField, Range(0f, 1f)] private float idleProbability = 0.3f;          // ���߱� Ȯ��

    private int next;         // ������ �õ��� ��ų �ε���
    private float pollT;      // ��ų üũ Ÿ�̸�
    private float wanderT;    // ���� �̵� ���ñ��� �ð�

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
            // �ε�ƿ��� �״�� ���� ��� ������ ��ų 1�� �õ�
            bool used = skillManager.TryUseFirstReadyInLoadout();

            // ����(���� ��Ÿ��)�� �ƹ� �͵� �� �� �� ��ҿ� AttackManager�� �ڵ�����
            pollT = pollInterval;
        }

        // 2) ������: ��ų�� ���ų�(=���) �������� ����
        if (enableWander && aiInput && (!player || player.IsGround))
        {
            wanderT -= Time.deltaTime;
            if (wanderT <= 0f)
            {
                // -1(��), 0(����), +1(��) �� �ϳ��� ���� ����
                int dir = Random.value < idleProbability ? 0 : (Random.value < 0.5f ? -1 : +1);
                aiInput.SetDir(dir, duration: Random.Range(0.6f, 1.2f)); // ��� ����
                ResetWanderTimer();
            }
        }
    }

    private void ResetWanderTimer()
    {
        wanderT = Random.Range(wanderIntervalRange.x, wanderIntervalRange.y);
    }
}
