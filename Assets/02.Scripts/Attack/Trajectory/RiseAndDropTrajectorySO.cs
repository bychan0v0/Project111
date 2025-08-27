using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Trajectory/RiseAndDrop")]
public class RiseAndDropTrajectorySO : TrajectorySO
{
    [Header("Speed Settings")]
    [SerializeField] private float speed = 12f;

    [Header("Screen & Camera")]
    [SerializeField] private float screenTopMargin = 0.5f; // ȭ�� ���� �󸶳� �� �ö󰡼� �������(���� ����)

    private enum Phase { Rising, Dropping }
    private Phase phase;

    // ��Ÿ�� ����
    private Transform target;
    private float vanishY;     // ȭ�� ���(+margin) Y (����)
    private float dropX;       // ������ X (Ÿ�� X �Ǵ� ������)
    private float dropStopY;   // Ÿ�� �Ӹ� ����(�����, ����� �浹���� ó��)

    public override void Init(Rigidbody2D rb, Transform shooter, Transform target)
    {
        base.Init(rb, shooter, target);

        // ī�޶� ���
        var cam = Camera.main;

        // ȭ�� ����� ���� Y ���ϱ� (�������, z�� ���� ����)
        float z = cam ? Mathf.Abs(shooter.position.z - cam.transform.position.z) : 0f;
        float topY = cam ? cam.ViewportToWorldPoint(new Vector3(0.5f, 1f, z)).y : shooter.position.y + 10f;
        vanishY = topY + screenTopMargin;

        // ���� ��ǥ X/Y ����
        float targetX = target.position.x;
        float targetY = target.position.y;

        dropX = targetX;    // �⺻�� Ÿ�� X
        dropStopY = targetY;    // �����(���� �浹�� �����Ƿ� ������� �ʾƵ� ��)

        // �ʱ� ��ġ/����: �߻��� X���� ���� ���
        rb.MovePosition(new Vector2(shooter.position.x, shooter.position.y));
        shooter.right = Vector2.up;

        phase = Phase.Rising;
    }

    public override bool Step(Rigidbody2D rb, Transform shooter, float dt)
    {
        Vector2 pos = rb.position;

        if (phase == Phase.Rising)
        {
            // �߻��� X ����, ���θ� �̵�
            pos.y += speed * dt;

            if (pos.y >= vanishY)
            {
                pos.x = dropX;
                pos.y = vanishY;    // ȭ�� �� ���������� �ٷ� �Ʒ��� ����������

                phase = Phase.Dropping;
                shooter.right = Vector2.down;    // ���� �Ʒ���
            }

            Move(shooter, rb, pos);
            
            return TickLife(dt);
        }

        // Dropping
        pos.x = dropX;
        pos.y -= speed * dt;

        Move(shooter, rb, pos);

        return TickLife(dt);
    }

    private void Move(Transform proj, Rigidbody2D rb, Vector2 pos)
    {
        rb.MovePosition(pos);
        proj.position = new Vector3(pos.x, pos.y, proj.position.z);
    }
}
