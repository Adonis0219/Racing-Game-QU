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
    /// 이동해야 되는 위치를 계산 후 전달하는 함수
    /// </summary>
    /// <param name="currPos">현재 위치</param>
    /// <param name="moveSpeedX"> </param>
    /// <param name="targetX"></param>
    /// <param name="chargeLate"></param>
    /// <param name="rotateDir">플레이어 이동시 회전해야되는 방향</param>
    /// <returns></returns>
    public virtual Vector3 CalculatePos(Vector3 currPos, float moveSpeedX, float targetX, float chargeLate,out int rotateDir)
    {
        int moveDirX;
        if (Mathf.Abs(targetX - currPos.x) > 0.1f)     //현재 위치한 라인과 목표 라인이 다를 경우
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
    /// 해당 상태로 전환할때 실행되는 함수
    /// </summary>
    /// <param name="preState">속도를 가져오기 위한 이전 상태</param>
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
