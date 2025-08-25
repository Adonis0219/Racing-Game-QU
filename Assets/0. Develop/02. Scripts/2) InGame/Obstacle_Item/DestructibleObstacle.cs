using System;
using UnityEngine;

public class DestructibleObstacle : IObstacle
{
    private Arrow arrow;
    private int playerLayerMask;
    private RaycastHit hit;
    [SerializeField] private GameObject crossHair;
    private bool isCrash = false;

    public float maxCharge; // 장애물 트리거 안에서 감지된 플레이어의 최대 차지값
    
    public Rigidbody Rb;

    private void Awake()
    {
        particleFactory = FindFirstObjectByType<ObjectFactory>();
    }

    protected void Start()
    {
        base.Start();
        arrow = GetComponentInChildren<Arrow>(true);
        playerLayerMask = 1 << LayerMask.NameToLayer("Player");
    }


    private void OnEnable()
    {
        //OnSceneChange += ResetGame;
        resetSettingValue();
    }

    protected void OnCollisionEnter(Collision other)
    {
        base.OnCollisionEnter(other);

        if (other.gameObject.layer == LayerMask.NameToLayer("ObstacleCollider"))
        {
            // 날아가는 장애물인 경우
            if (isCrash)
            {
                particleFactory.Spawn(PoolObjectType.P_Explosion, transform.position);
                ReturnPool();
                _scoreMgr.CrashCombo++;
            }
            // 날아오는 물체에 충돌된 장애물인 경우
            else
            {
                //particleFactory.Spawn(PoolObjectType.P_Explosion, transform.position);
                Rb.linearVelocity = other.gameObject.GetComponent<Rigidbody>().linearVelocity;
                Rb.angularVelocity = other.gameObject.GetComponent<Rigidbody>().angularVelocity;
                isCrash = true;
                //Debug.Log($"isCrash: ");
            }
        }
    }
    
    #region 플레이어 감지
    /// <summary>
    /// 플레이어가 장애물 앞에 있다는 것이 인식되었을 때 호출됨
    /// </summary>
    /// <param name="other">플레이어</param>
    private void OnTriggerEnter(Collider other)
    {
        if (!isCrash)
        {
            arrow.gameObject.SetActive(true);
            crossHair.SetActive(true);
        }
    }
    #endregion
    
    #region 날아가는 경로 시각화
    /// <summary>
    /// 플레이어가 차징하는 동안 화살표 표시
    /// </summary>
    /// <param name="other">플레이어</param>
    private void OnTriggerStay(Collider other)
    {
        if (Physics.Raycast(transform.position, -transform.up, out hit, 4f, playerLayerMask))
        {
            if (player.CurrentCharge > 20f)
            {
                arrow.DrawCursor(transform.position, hit.distance - 1, player.CurrentCharge);
                maxCharge = player.CurrentCharge;
            }
            arrow.SetDir(transform.position);
        }
    }
    #endregion
    
    #region 장애물 회피하거나 날리기
    /// <summary>
    /// 플레이어와 거리가 멀어진 장애물의 경로 표시 끄기
    /// </summary>
    /// <param name="other">플레이어</param>
    private void OnTriggerExit(Collider other)
    {
        if (player.isChargeMoving)
        {
            float chargePercent = maxCharge / 100;
            if (chargePercent >= 0.5f)
            {
                particleFactory.Spawn(PoolObjectType.P_Explosion, transform.position);
                Rb.AddForce(30f * (chargePercent) * arrow.Dir, ForceMode.VelocityChange);
                Rb.AddTorque(20f * (chargePercent) * arrow.Dir, ForceMode.VelocityChange);
                isCrash = true;
                arrow?.gameObject.SetActive(false);
                crossHair?.SetActive(false);
                player.isChargeMoving = false;
            }
        }
    }
    #endregion
    
    void resetSettingValue()
    {
        arrow?.gameObject.SetActive(false);
        crossHair?.SetActive(false);
        isCrash = false;
        maxCharge = 0;
    }
}
