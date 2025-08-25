using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public enum SceneType { Main, Game }

public class GameManager : MonoBehaviour, IPullManager
{
    public static GameManager instance;

    // ------- Managers -------
    private StageManager _stageMgr;
    private ScoreManager _scoreMgr;
    private AudioManager _audioMgr;
    private DataManager _dataMgr;

    [SerializeField] public Car_Data[] carDatas;

    #region Parsing Variable

    [SerializeField] public Player player;

    [Header("# MainScene")] [SerializeField]
    private Button startBt;

    [SerializeField] public GameObject[] uis;

    public event Action<SceneType> onSceneChange; // 씬 변경 이벤트

    #endregion

    #region Reset Method Example

    public void ResetFuel(SceneType type)
    {
        if (type == SceneType.Main) return;
    }

    #endregion
    
    private SceneType curScene = SceneType.Main;

    private bool isPause = false;

    Coroutine scoreCoru;

    #region Property
    public SceneType CurScene
    {
        get => curScene;
        set
        {
            curScene = value;
            
            onSceneChange?.Invoke(value); // 씬 변경 이벤트 호출
        }
    }

    public bool IsPause
    {
        get => isPause;
        set
        {
            isPause = value;

            Time.timeScale = isPause ? 0f : 1f; // 게임 일시 정지 또는 재개
        }
    }

    int curCarIndex = 0;    // 기본 선택 0

    public int CurCarIndex
    {
        get => curCarIndex;

        set
        {
            if (curCarIndex == value)
                return;

            curCarIndex = value;
        }
    }

    #endregion

    private void Awake()
    {
        instance = this;

        onSceneChange += ChangeScene; // 씬 변경 이벤트에 ChangeScene 메서드 등록
    }

    private void Update()
    {
        #if UNITY_EDITOR
        
        for (int i = 0; i < 9; i++)
        {
            if (Input.GetKeyDown((KeyCode)(49 + i)))
            {
                Time.timeScale = 1 + i;
            }
        }
        
        #endif
    }

    #region Setting Method

    /// <summary>
    /// 씬을 전환하는 함수
    /// </summary>
    /// <param name="type">바꿔줄 씬</param>
    public void ChangeScene(SceneType type)
    {
        _dataMgr.SaveData();
        
        bool isMain = type == SceneType.Main ? true : false;
        
        // 인자값에 따라 씬 On / Off
        uis[(int)SceneType.Main].SetActive(isMain);
        uis[(int)SceneType.Game].SetActive(!isMain);

        // 자동차 세팅
        PlayerCarSet(isMain);
        _audioMgr.BgmSoundPlay(isMain);

        if (isMain)
        {
            SceneManager.LoadScene(0);
            // 메인 씬으로 나올 때
            /// 풀매니저의 켜져있는 모든 오브젝트 끄기
            PoolManager.instance.ClearPool();

            StopCoroutine(scoreCoru);
        }
        else
        {
            _stageMgr.RandMap(); // 게임 씬으로 넘어갈 때 랜덤 맵 선택

            _stageMgr.currentGameTime = 0f; // 게임 시간 초기화

            scoreCoru = _scoreMgr.StartCoroutine(_scoreMgr.OnScoreUp());
        }
    }

    /// <summary>
    /// 선택한 차종에 따라 바꿔주는 함수
    /// </summary>
    /// <param name="isGameScene">게임씬으로 들어가는지</param>
    void PlayerCarSet(bool isMainScene)
    {
        player.transform.GetChild(CurCarIndex).gameObject.SetActive(!isMainScene);
        //player.MyData = carDatas[CurCarIndex];
    }

    public void PullUseManager()
    {
        CoreManager cm = CoreManager.instance;
        
        _stageMgr = cm.GetManager<StageManager>();
        _scoreMgr = cm.GetManager<ScoreManager>();
        _audioMgr = cm.GetManager<AudioManager>();
        _dataMgr = cm.GetManager<DataManager>();
    }

    #endregion
}
