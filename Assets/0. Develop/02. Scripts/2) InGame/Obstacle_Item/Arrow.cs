using System;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private float maxArrowSize = 0.6f;
    private Vector3 dir;

    public Vector3 Dir
    {
        get
        {
            return dir;
        }
    }

    private Vector3 initPosition = new Vector3(-2, 0, 0);
    private Vector3 initEulerRotation = new Vector3(0, -90f, 90f);

    public void OnEnable()
    {
        SetInitialPos();
    }

    public void OnDisable()
    {
        
    }
    
    #region 화살표 초기화
    //화살표 초기 위치 설정
    public void SetInitialPos()
    {
        transform.SetLocalPositionAndRotation(initPosition, Quaternion.Euler(initEulerRotation));
        transform.localScale = Vector3.zero;
    }
    #endregion

    #region 장애물이 날아가는 방향 설정
    /// <summary>
    /// 화살표 방향 설정
    /// </summary>
    /// <param name="originPos"> 장애물 날리려는 자동차의 위치 </param>
    public void SetDir(Vector3 originPos)
    {
        if(!GameManager.instance.player.isChargeMoving) return;
        dir = transform.position - originPos;
        dir.Normalize();
    }
    #endregion

    #region 날아가는 방향 그리기
    /// <summary>
    /// 장애물이 날아가는 경로 표시하는 화살표를 그리는 기능
    /// </summary>
    /// <param name="distance">장애물과 플레이어 사이의 거리(0 ~ 3f)</param>
    /// <param name="power">차지 파워</param>
    public void DrawCursor(Vector3 originPos, float distance, float power)
    {
        if (GameManager.instance.player.CurrentCharge <= 30f) return;
        //차지 파워(0~100)에 따른 화살표 크기 조절
        transform.localScale = Vector3.one * maxArrowSize * (power*0.01f);

        //거리에 따른 화살표의 위치, 회전을 계산하기 위한 백분율 값
        //float percent = !playerObject.inputButtonIsRight ? (1f -distance/7f) : 3f-distance/7f;
        float percent = GameManager.instance.player.inputButtonIsRight ? (1f - distance / 3f) : (distance / 3f - 3f);

        float x = -2 * Mathf.Cos(Mathf.Deg2Rad * 90f * percent);
        float y = Mathf.Abs(2 * Mathf.Sin(Mathf.Deg2Rad * 90f * percent));

        // 떨어져 있는 거리에 따른 장애물 날리는 방향 계산
        transform.position = originPos + new Vector3(x, y, 0);
        transform.localRotation = Quaternion.Euler(percent * -90f, -90f, 90f);
    }
    #endregion
}
