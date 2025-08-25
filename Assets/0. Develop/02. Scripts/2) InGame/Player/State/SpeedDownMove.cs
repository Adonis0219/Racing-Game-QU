using UnityEngine;

public class SpeedDownMove : IdleMove
{
    [Tooltip("시간당 감소하는 속도")]
    float speedDownPerDelta = 0;
    public SpeedDownMove(PlayerMoveController controller, GameObject dashParticle) : base(controller,dashParticle) { }


    public override Vector3 CalculatePos(Vector3 currPos, float moveSpeedX, float targetX, float chargeLate, out int rotateDir)
    {
        speedDownPerDelta += 0.1f * Time.deltaTime; //시간마다 감소하는 속도 증가
        MoveSpeedY -= speedDownPerDelta * Time.deltaTime;

        if (Mathf.Abs(MoveSpeedY - (int)MoveSpeedY) < 0.1f)
            speedDownPerDelta = 0.1f;

        if (MoveSpeedY <= 0.1f)
        {
            controller.ChangeState(controller.IdleMoveState);
        }


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

        float moveY = MoveSpeedY * 2f - 4.5f;



        Vector3 movePos = new Vector3(moveX, moveY, 0);


        return Vector3.MoveTowards(currPos, movePos, Time.deltaTime * (1 + chargeLate) * moveSpeedX);
    }

    public override void StartState(IdleMove preState)
    {
        dashParticle.SetActive(false);
        base.StartState(preState);
        speedDownPerDelta = 0;
    }

}
