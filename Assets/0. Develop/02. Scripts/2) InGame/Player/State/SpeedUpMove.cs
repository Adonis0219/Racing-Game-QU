using UnityEngine;

public class SpeedUpMove : IdleMove
{
    float chargeCount = 0;
    public SpeedUpMove(PlayerMoveController controller, GameObject dashParticle) : base(controller, dashParticle) { }

    public override Vector3 CalculatePos(Vector3 currPos, float moveSpeedX, float targetX, float chargeLate, out int rotateDir)
    {
        int moveDirX;
        chargeCount += Time.deltaTime;
        if (chargeCount > 3)
        {
            controller.ChangeState(controller.SpeedDownState);
        }

        if (Mathf.Abs(targetX - currPos.x) > 0.1f)      //현재 위치한 라인과 목표 라인이 다를 경우
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

        float moveY = MoveSpeedY * 2f - 4.5f;

        Vector3 movePos = new Vector3(moveX, moveY, 0);


        return Vector3.MoveTowards(currPos, movePos, Time.deltaTime * (1 + chargeLate) * moveSpeedX);
    }

    public override void StartState(IdleMove preState)
    {
        dashParticle.SetActive(true);
        base.StartState(preState);
        chargeCount = 0;
    }

}
