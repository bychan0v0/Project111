using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIInput : MonoBehaviour, IMoveInput
{
    [SerializeField] private float leftLimit = 2f;
    [SerializeField] private float rightLimit = 8.5f;
    [SerializeField] private int dir = 0;

    // === 추가: 외부가 잠깐 방향/정지 지시 ===
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
        // 평소: 좌우 한계에서 왕복
        if (transform.position.x >= rightLimit) return -1;
        if (transform.position.x <= leftLimit) return 1;
        
        // 외부 지시 유지 시간 처리
        if (overrideActive)
        {
            if (overrideUntil > 0f && Time.time >= overrideUntil)
                overrideActive = false; // 만료
            else
                return dir; // 외부 지시 우선
        }
        
        return dir;
    }
}
