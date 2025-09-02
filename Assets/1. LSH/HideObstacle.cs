using System;
using UnityEngine;

public class HideObstacle : PoolObject, IFall, IPullManager
{
    // ======= Managers =======
    private StageManager _stageMgr;
    
    [SerializeField] private FadeUI fade;
    
    float speed;
    public float DownSpeed { get => speed; set => speed = value; }

    private void Awake()
    {
        PullUseManager();
    }

    private void Start()
    {
        DownSpeed = 7f;
    }

    private void Update()
    {
        MoveDown();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("트리거");
        
        if (other.CompareTag("Player"))
        {
            fade = _stageMgr.hideUis[_stageMgr.CurMapIndex];
            
            ShowUI(fade);
            
            Destroy(gameObject);
        }
    }

    public void ShowUI(FadeUI fade)
    {
        fade.gameObject.SetActive(true);
        
        fade.Excute();
    }

    public void MoveDown()
    {
        transform.position += Vector3.down * DownSpeed * Time.deltaTime; //점차 하강
        //if(transform.position.z <= -7f) ReturnPool();   //장애물이 화면에서 벗어났을 경우 Pool에 넣기(원본)
        if (transform.position.y <= -7f || transform.position.x >= Mathf.Abs(7f))
            ReturnPool(); //장애물이 화면에서 벗어났을 경우 Pool에 넣기
    }

    public void PullUseManager()
    {
        _stageMgr = CoreManager.instance.GetManager<StageManager>();
    }
}