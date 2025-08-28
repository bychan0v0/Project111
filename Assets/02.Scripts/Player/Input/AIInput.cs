using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIInput : MonoBehaviour, IMoveInput
{
    [SerializeField] private float leftLimit = 2f;
    [SerializeField] private float rightLimit = 8.5f;
    [SerializeField] private int dir = 0;

    // === �߰�: �ܺΰ� ��� ����/���� ���� ===
    private bool overrideActive;
    private float overrideUntil;

    public void SetDir(int d, float duration = 0f)
    {
        overrideActive = true;
        overrideUntil  = (duration > 0f) ? Time.time + duration : 0f;
        dir = Mathf.Clamp(d, -1, 1); // -1,0,+1
    }

    public float GetMoveX()
    {
        // ���: �¿� �Ѱ迡�� �պ�
        if (transform.position.x >= rightLimit) return -1;
        if (transform.position.x <= leftLimit) return 1;
        
        // �ܺ� ���� ���� �ð� ó��
        if (overrideActive)
        {
            if (overrideUntil > 0f && Time.time >= overrideUntil)
                overrideActive = false; // ����
            else
                return dir; // �ܺ� ���� �켱
        }
        
        return dir;
    }
}
