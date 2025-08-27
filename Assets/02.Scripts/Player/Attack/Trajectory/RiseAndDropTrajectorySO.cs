using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Trajectory/RiseAndDrop")]
public class RiseAndDropTrajectorySO : TrajectorySO
{
    [Header("Speed Settings")]
    [SerializeField] private float speed = 12f;

    [Header("Target Head Offset")]
    [SerializeField] private float headOffsetY = 1.0f;

    [Header("Screen & Camera")]
    [SerializeField] private float screenTopMargin = 0.5f; // 화면 위로 얼마나 더 올라가서 사라질지(월드 유닛)

    private enum Phase { Rising, Dropping }
    private Phase phase;

    // 런타임 상태
    private Transform target;
    private float vanishY;     // 화면 상단(+margin) Y (월드)
    private float dropX;       // 낙하할 X (타겟 X 또는 스냅샷)
    private float dropStopY;   // 타겟 머리 높이(참고용, 종료는 충돌에서 처리)

    public override void Init(Transform proj, Rigidbody2D rb, Vector2 start, Vector2 aimPoint, Transform aimTarget)
    {
        base.Init(proj, rb, start, aimPoint, aimTarget);

        target = aimTarget;

        // 카메라 얻기
        var cam = Camera.main;

        // 화면 상단의 월드 Y 구하기 (오쏘기준, z는 무시 가능)
        float z = cam ? Mathf.Abs(proj.position.z - cam.transform.position.z) : 0f;
        float topY = cam ? cam.ViewportToWorldPoint(new Vector3(0.5f, 1f, z)).y : start.y + 10f;
        vanishY = topY + screenTopMargin;

        // 낙하 목표 X/Y 산출
        float targetX = target ? target.position.x : aimPoint.x;
        float targetY = target ? target.position.y : aimPoint.y;

        dropX = targetX;                    // 기본은 타겟 X
        dropStopY = targetY + headOffsetY;  // 참고용(보통 충돌로 끝내므로 사용하지 않아도 됨)

        // 초기 위치/방향: 발사자 X에서 수직 상승
        rb.MovePosition(new Vector2(start.x, start.y));
        proj.right = Vector2.up;

        phase = Phase.Rising;
    }

    public override bool Step(Transform proj, Rigidbody2D rb, float dt)
    {
        Vector2 pos = rb.position;

        if (phase == Phase.Rising)
        {
            // 발사자 X 고정, 위로만 이동
            pos.y += speed * dt;

            if (pos.y >= vanishY)
            {
                pos.x = dropX;
                pos.y = vanishY;    // 화면 위 시작점에서 바로 아래로 떨어지도록

                phase = Phase.Dropping;
                proj.right = Vector2.down;    // 방향 아래로
            }

            Move(proj, rb, pos);
            
            return TickLife(dt);
        }

        // Dropping
        pos.x = dropX;
        pos.y -= speed * dt;

        Move(proj, rb, pos);

        return TickLife(dt);
    }

    private void Move(Transform proj, Rigidbody2D rb, Vector2 pos)
    {
        rb.MovePosition(pos);
        proj.position = new Vector3(pos.x, pos.y, proj.position.z);
    }
}
