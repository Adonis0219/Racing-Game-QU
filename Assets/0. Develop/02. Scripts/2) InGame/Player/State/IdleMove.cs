using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class IdleMove
{
    protected PlayerMoveController controller;

    protected float moveSpeedY;
    protected GameObject dashParticle;
    public float MoveSpeedY
    {
        get => moveSpeedY;
        set => moveSpeedY = Mathf.Clamp(value, 0, 2.5f);
    }
    
    public IdleMove(PlayerMoveController controller,GameObject dashParticle)
    {
        this.controller = controller;
        this.dashParticle = dashParticle;
    }

    /// <summary>
    /// �̵��ؾ� �Ǵ� ��ġ�� ��� �� �����ϴ� �Լ�
    /// </summary>
    /// <param name="currPos">���� ��ġ</param>
    /// <param name="moveSpeedX"> </param>
    /// <param name="targetX"></param>
    /// <param name="chargeLate"></param>
    /// <param name="rotateDir">�÷��̾� �̵��� ȸ���ؾߵǴ� ����</param>
    /// <returns></returns>
    public virtual Vector3 CalculatePos(Vector3 currPos, float moveSpeedX, float targetX, float chargeLate,out int rotateDir)
    {
        int moveDirX;
        if (Mathf.Abs(targetX - currPos.x) > 0.1f)     //���� ��ġ�� ���ΰ� ��ǥ ������ �ٸ� ���
        {
            moveDirX = targetX > currPos.x ? 1 : -1;
            rotateDir = -moveDirX;
        }
        else
        {
            moveDirX = 0;
            rotateDir = 0;
        }
        float moveX = currPos.x + moveDirX;

        Vector3 movePos = new Vector3(moveX, -4.5f, 0);
        return Vector3.MoveTowards(currPos, movePos,Time.deltaTime * (1 + chargeLate)*moveSpeedX);

    }

    /// <summary>
    /// �ش� ���·� ��ȯ�Ҷ� ����Ǵ� �Լ�
    /// </summary>
    /// <param name="preState">�ӵ��� �������� ���� ���� ����</param>
    public virtual void StartState(IdleMove preState) {
        MoveSpeedY = preState.MoveSpeedY;
    }

    public void SpeedDown(float down)
    {
        MoveSpeedY -= down;
        controller.ChangeState(controller.SpeedDownState);
    }
    public void SpeedUp(float up)
    {
        dashParticle.SetActive(false);
        MoveSpeedY += up;
        controller.ChangeState(controller.ChargeMoveState);
    }

}
