using System;
using UnityEngine;

public class Obstacle : PoolObject, IFall, IPullManager
{
    // ------- Managers -------
    private ScoreManager _scoreMgr;
    private FuelManager _fuelMgr;
    private StageManager _stageMgr;
    
    #region DnD 변수
    
    [SerializeField] private GameObject crossHair;
    
    public ObjectFactory particleFactory;
    
    public float maxCharge; // 장애물 트리거 안에서 감지된 플레이어의 최대 차지값
    
    public Rigidbody Rb;

    #endregion
    
    #region private 멤버변수
    
    private Arrow arrow;
    private Player player;
    
    private bool isCrash = false;
    private float speed; // 장애물이 내려오는 속도
    public float DownSpeed
    {
        get => speed;
        set => speed = value * (Screen.safeArea.height / 1920); //기기 해상도 기준(1920), 해상도가 다른 기기들의 속도 평준화
    }

    private int playerLayerMask;
    private RaycastHit hit;
    
    #endregion
    
    private void Awake()
    {
        particleFactory = FindFirstObjectByType<ObjectFactory>();
    }

    private void OnEnable()
    {
        //OnSceneChange += ResetGame;
        resetSettingValue();
    }


    void Start()
    {
        player = GameManager.instance.player;
        arrow = GetComponentInChildren<Arrow>(true);
        PullUseManager();
        DownSpeed = 7f;
        playerLayerMask = 1 << LayerMask.NameToLayer("Player");
    }

    // private void OnDisable()
    // {
    //     resetSettingValue();
    // }

    void Update()
    {
        MoveDown();
    }

    #region 충돌 이벤트

    /// <summary>
    /// 플레이어, 장애물 충돌할 때 처리
    /// </summary>
    /// <param name="other">플레이어 또는 장애물</param>
    private void OnCollisionEnter(Collision other)
    {
        Debug.Log(LayerMask.LayerToName(other.gameObject.layer));
        ///플레이어와 충돌이 발생했을 때
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (player.isChargeMoving) return;
            Debug.Log("Player and Obstacle Collision");
            
            
            

            //충돌 시 플레이어 세팅값 초기화 호출
            player.playerCollided.Invoke();

            //충돌시 연료 감소
            _fuelMgr.CalculateFuel(-20f);
            particleFactory.Spawn(PoolObjectType.P_Explosion, transform.position);
            
            //연료 모두 소진시 게임 오버
            //if (_fuelMgr.curruntFuel <= 0) _stageMgr.GameClear();
            
            
            ReturnPool();
                        
        }

        ///장애물끼리 충돌이 발생했을 때
        else
        {
            // 날아가는 장애물인 경우
            if (isCrash)
            {
                Debug.Log("Two Obstacle Craash!");
                ReturnPool();
                _scoreMgr.CrashCombo++;
            }
            // 날아오는 물체에 충돌된 장애물인 경우
            else
            {
                particleFactory.Spawn(PoolObjectType.P_Explosion, transform.position);
                Debug.Log($"!isCrash: {other.gameObject.GetComponent<Rigidbody>().linearVelocity}");
                Rb.linearVelocity = other.gameObject.GetComponent<Rigidbody>().linearVelocity;
                //Debug.Log($"isCrash: ");
            }
        }
    }

    #endregion


    #region 플레이어 감지

    /// <summary>
    /// 플레이어가 장애물 앞에 있다는 것이 인식되었을 때 호출됨
    /// </summary>
    /// <param name="other">플레이어</param>
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Obstacle Trigger On");


        //화살표 초기 설정
        arrow.gameObject.SetActive(true);
        crossHair.SetActive(true);
    }

    #endregion

    #region 날아가는 경로 시각화

    /// <summary>
    /// 플레이어가 차징하는 동안 화살표 표시
    /// </summary>
    /// <param name="other">플레이어</param>
    private void OnTriggerStay(Collider other)
    {
        // RayCast를 사용하여 플레이어와 떨어진 거리 계산
        if (Physics.Raycast(transform.position, -transform.up, out hit, 4f, playerLayerMask))
        {
            //플레이어가 계속 차징 중일때 날아가는 경로 그리기
            if (player.CurrentCharge > 30f)
            {
                arrow.DrawCursor(transform.position, hit.distance - 1, player.CurrentCharge);
                maxCharge = player.CurrentCharge;
            }

            //플레이어가 날리고 싶은 방향에서 차징을 풀고 다른 라인으로 이동했을 때 발동
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
        //플레이어가 차지로 트리거를 나갔다면 장애물 날리기
        if (player.isChargeMoving)
        {
            float chargePercent = maxCharge / 100;
            if(chargePercent >= 0.5f)particleFactory.Spawn(PoolObjectType.P_Explosion, transform.position);
            Rb.AddForce(30f * (chargePercent) * arrow.Dir, ForceMode.VelocityChange);
            isCrash = true;
            
            arrow?.gameObject.SetActive(false);
            crossHair?.SetActive(false);
        }
    }

    #endregion

    public void ResetGame(SceneType scene)
    {
        ReturnPool();
    }

    void resetSettingValue()
    {
        arrow?.gameObject.SetActive(false);
        crossHair?.SetActive(false);
        isCrash = false;
        maxCharge = 0;
    }


    #region 장애물 하강

    public void MoveDown()
    {
        transform.position += Vector3.down * speed * Time.deltaTime; //점차 하강
        //if(transform.position.z <= -7f) ReturnPool();   //장애물이 화면에서 벗어났을 경우 Pool에 넣기(원본)
        if (transform.position.y <= -7f && transform.position.x >= Mathf.Abs(7f))
            ReturnPool(); //장애물이 화면에서 벗어났을 경우 Pool에 넣기
    }

    #endregion
    
    public void PullUseManager()
    {
        _scoreMgr = CoreManager.instance.GetManager<ScoreManager>();
        _fuelMgr = CoreManager.instance.GetManager<FuelManager>();
        _stageMgr = CoreManager.instance.GetManager<StageManager>();
    }
}
