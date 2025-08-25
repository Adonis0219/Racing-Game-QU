using System;
using UnityEngine;

[Tooltip("�÷��̾��� �̵��� ����ϴ� Ŭ����")]
public class PlayerMoveController
{
    protected float moveSpeedX;
    protected float charge_late = 0;

    [Tooltip("�̵� ������ ���ε�")]
    protected float[] lines;
    protected int lineIndex = 2;
    int rotDir;

    #region ���µ� ����
    public IdleMove IdleMoveState { get; private set; }
    public IdleMove ChargeMoveState { get; private set; }
    public IdleMove SpeedDownState { get; private set; }
    public IdleMove CurrState { get; private set; }
    #endregion

    [Header("# ��ư ��/Ȱ��ȭ�� ���� �׼�")]
    public ActiveButton rightButtonActive;
    public ActiveButton leftButtonActive;
    public delegate void ActiveButton(bool active);

    [Tooltip("MoveController�� �ʱ�ȭ �ϱ� ���� �׼�")]
    public Action ResetMoveSystem;
    GameObject dashParticle;

    public PlayerMoveController(ActiveButton rightActive, ActiveButton leftActive, float moveSpeedX, float[] lines, GameObject dashParticle)
    {
        IdleMoveState = new IdleMove(this,dashParticle);
        ChargeMoveState = new SpeedUpMove(this, dashParticle);
        SpeedDownState = new SpeedDownMove(this, dashParticle);

        CurrState = IdleMoveState;

        this.moveSpeedX = moveSpeedX;
        this.lines = lines;
        this.rightButtonActive = rightActive;
        this.leftButtonActive = leftActive;
        ResetMoveSystem += () =>
        {
            CurrState = IdleMoveState;
            lineIndex = 2;
            CurrState.SpeedDown(100);
        };
        this.dashParticle = dashParticle;
    }

    public void Move(Transform transform)
    {
        Vector3 movePos = CurrState.CalculatePos(transform.position, moveSpeedX, lines[lineIndex], charge_late, out rotDir);
        if (rotDir != 0)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotDir * 20));
        }
        else
        {
            transform.rotation = Quaternion.identity;
        }
        transform.position = movePos;
    }


    [Tooltip("�̵� ���� �����͸� �����ϴ� �Լ�")]
    public void SetMoveData(bool isRight, float charge_per)
    {
        lineIndex += isRight ? 1 : -1;
        lineIndex = Mathf.Clamp(lineIndex, 0, lines.Length - 1);
        rightButtonActive.Invoke(lineIndex < lines.Length - 1);
        leftButtonActive.Invoke(lineIndex != 0);
        charge_late = charge_per;

        if (charge_late >= 0.5f)
        {
            CurrState.SpeedUp(charge_per * 0.3f);
        }
    }

    public void ChangeState(IdleMove newState)
    {
        var temp = CurrState;
        CurrState = newState;
        CurrState.StartState(temp);

    }
    public void SpeedDown(float minus)
    {
        CurrState.SpeedDown(minus);
    }
    public void UseSpeedUp()
    {
        CurrState.SpeedUp(5);
    }
}
