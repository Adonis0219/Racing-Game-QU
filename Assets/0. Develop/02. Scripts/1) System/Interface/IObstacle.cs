using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class IObstacle : PoolObject, IFall, IPullManager/*, IResetable*/
{
    // ------- Managers -------
    protected ScoreManager _scoreMgr;
    protected FuelManager _fuelMgr;
    protected StageManager _stageMgr;

    protected Player player;
    protected ObjectFactory particleFactory;
    protected float speed;
    
    public float DownSpeed
    {
        get => speed;
        set => speed = value;
    }
    
    protected void Start()
    {
        player = GameManager.instance.player;
        particleFactory = FindFirstObjectByType<ObjectFactory>();
        PullUseManager();
        DownSpeed = 7f;
    }

    protected void Update()
    {
        MoveDown();
    }

    public void MoveDown()
    {
        transform.position += Vector3.down * speed * Time.deltaTime; //점차 하강
        //if(transform.position.z <= -7f) ReturnPool();   //장애물이 화면에서 벗어났을 경우 Pool에 넣기(원본)
        if (transform.position.y <= -7f || transform.position.x >= Mathf.Abs(7f))
            ReturnPool(); //장애물이 화면에서 벗어났을 경우 Pool에 넣기
    }

    protected void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (!player.isChargeMoving)
            {
                //충돌 시 플레이어 세팅값 초기화 호출
                player.playerCollided.Invoke();
    
                //충돌시 연료 감소
                _fuelMgr.CalculateFuel(-20f);
                particleFactory.Spawn(PoolObjectType.P_Explosion, transform.position);
                //연료 모두 소진시 게임 오버
                //if (_fuelMgr.curruntFuel <= 0) _stageMgr.GameClear();
                ReturnPool();
            }
        }
    }

    public void ResetGame(SceneType scene)
    {
        SceneManager.LoadScene(0);
    }

    public void PullUseManager()
    {
        _scoreMgr = CoreManager.instance.GetManager<ScoreManager>();
        _fuelMgr = CoreManager.instance.GetManager<FuelManager>();
        _stageMgr = CoreManager.instance.GetManager<StageManager>();
    }
}
