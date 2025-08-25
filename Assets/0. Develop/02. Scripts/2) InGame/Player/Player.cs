using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(PlayerColliderSystem))]
public class Player : MonoBehaviour, IPullManager, IResetable
{
    // ------- Managers -------
    private ScoreManager _scoreMgr;   
    private FuelManager _fuelMgr;
    private DataManager _dataMgr;
    private AudioManager _audioMgr;
    [Header("# UIs")]
    [SerializeField]
    protected MoveButton leftMoveButton;
    [SerializeField]
    protected MoveButton rightMoveButton;
    [SerializeField]
    GameObject dashParticle;

    #region  차징 관련 변수
    [Header("# About Charges")]
    public float maxCharge = 100f;
    public float chargePerDelta = 100f;
    public float minTimeScale = 0.2f;
    public float maxTimeScale = 1f;
    public float CurrentCharge { get; set; }
    #endregion

    #region 충돌 이벤트 관련
    [Header("# About Collider Events")]
    public PlayerColided playerCollided;
    public delegate void PlayerColided();
    PlayerColliderSystem _colliderSystem;
    public PlayerMoveController _moveController;
    #endregion

    [Header("# To Show Others")]
    public bool isChargeMoving;
    public bool inputButtonIsRight;

    [Header("# About Lines")]
    protected float[] linesXPos = new float[5];//차량이 이동 가능한 라인의 좌우 값
    float mostLeftLineX = -2;
    float gapOfLinesX = 1f;

    [Header("# Else")]
    public bool isInvincible = false;
    public bool isHaveNearObj;
    float invincibleTime = 2;
    IEnumerator chargeInvicible;

    [Tooltip("게임 시작시 차량의 정보를 받아오기 위한 함수")]
    public void GetCarData(Car_Data myCarData)
    {
        _fuelMgr.ChangeMaxFuel(myCarData.maxFuel);
        _moveController = new PlayerMoveController(rightMoveButton.SetPointActive,
            leftMoveButton.SetPointActive
            , myCarData.oriSpeed, linesXPos
            ,dashParticle);
        chargePerDelta = myCarData.chargePerDelta;
        maxCharge = myCarData.maxCharge;
    }
    public void Start()
    {
        #region 게임 시작시 라인 설정
        for (int i = 0; i < linesXPos.Length; i++)
            linesXPos[i] = mostLeftLineX + gapOfLinesX * i;
        #endregion
        chargeInvicible = ChargeInvincibleRoutine();
        _InitColliderSystem();
    }

    void _InitColliderSystem()
    {
        _colliderSystem = GetComponent<PlayerColliderSystem>();
        
        _colliderSystem.InitSet();

        _moveController = new PlayerMoveController(rightMoveButton.SetPointActive,
            leftMoveButton.SetPointActive
            , 5, linesXPos
            ,dashParticle);

        _colliderSystem.itemEffects.Add("SpeedUp", _moveController.UseSpeedUp);
        playerCollided = PlayerCollided;
    }
    
    private void Update()
    {
        if (GameManager.instance.CurScene != SceneType.Game)
            return;
        _moveController.Move(transform);
    }

    /// <summary>
    /// 버튼을 놓았을 때 이동을 위해 작동하는 함수
    /// </summary>
    /// <param name="isRight">오른쪽으로 움직이는 여부</param>
    public void ArrowBtClk(bool isRight, float currCharge)
    {
        _moveController.SetMoveData(isRight, currCharge / maxCharge);
        var sfx = Sfx.Move;
        if (isHaveNearObj)
        {
            if (currCharge > 50)
            {
                sfx = Sfx.ChargeMove;
                isChargeMoving = true;
                _scoreMgr.DodgeCombo++;
                if (!isInvincible)
                    StartCoroutine(chargeInvicible);
            }
        }
        _audioMgr.PlaySfx(sfx);
        isHaveNearObj = false;
    }
    

    [Tooltip("플레이어 충돌시 작동하는 함수")]
    public void PlayerCollided()
    {
        if (isInvincible)
            return;
        leftMoveButton.ResetButton();
        rightMoveButton.ResetButton();
        isHaveNearObj = false;

        _moveController.SpeedDown(1);
        StartCoroutine(BlinkRoutine());
    }

    #region 충돌 무적 및 차징 이동 무적
    /// <summary>
    /// 일정시간 무적 및 깜빡임 코루틴 
    /// </summary>
    /// <returns>반환값은 없음,invincible_time동안 무적</returns>
    public IEnumerator BlinkRoutine()
    {
        isInvincible = true;
        StopCoroutine(chargeInvicible);

        WaitForSeconds waitTime = new WaitForSeconds(invincibleTime / 10);
        MeshRenderer[] renders = transform.GetComponentsInChildren<MeshRenderer>();

        for (int count = 0; count < invincibleTime * 10; count++)
        {
            for (int i = 0; i < renders.Length; i++)
                renders[i].enabled = !renders[i].enabled;
            yield return waitTime;
        }
        isInvincible = false;
    }
    IEnumerator ChargeInvincibleRoutine()
    {
        isInvincible = true;

        yield return new WaitForSeconds(0.3f);

        chargeInvicible= ChargeInvincibleRoutine();
        isInvincible =false;
    }
    #endregion


    public void ResetGame(SceneType scene)
    {
        if (scene == SceneType.Game)
        {
            isInvincible = false;
            CurrentCharge = 0;
            transform.position = new Vector3(0, -7, 0);
            _moveController.ResetMoveSystem?.Invoke();
            #region 버튼 초기화
            rightMoveButton.ResetButton?.Invoke() ;
            leftMoveButton.ResetButton?.Invoke();
            leftMoveButton.SetPointActive(true);
            rightMoveButton.SetPointActive(true);
            #endregion
        }
    }
    void OnEnable()
    {
        GameManager.instance.onSceneChange += ResetGame;
    }

    public void PullUseManager()
    {
        _scoreMgr = CoreManager.instance.GetManager<ScoreManager>();
        _fuelMgr = CoreManager.instance.GetManager<FuelManager>();
        _audioMgr = CoreManager.instance.GetManager<AudioManager>();
    }

    public void SetInputButtonIsRight(bool isRight)
    {
        inputButtonIsRight = isRight;
    }

    public bool GetInputButtonIsRight()
    {
        return inputButtonIsRight;
    }
}