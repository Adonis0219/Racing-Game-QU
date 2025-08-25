
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class StageManager : BaseManager, IPullManager
{
    // -------- Managers -------
    private DataManager _dataMgr;
    private ScoreManager _scoreMgr;

    [Header("# Maps")]
    [SerializeField]
    Light dLight;
    
    [SerializeField]
    Color[] mapColors;

    [Header("# UIs")]
    [SerializeField]
    private Button exitGameBt;
    
    [SerializeField]
    private TextMeshProUGUI mapNameText;

    [SerializeField]
    GameObject resultPanel;
    [SerializeField]
    GameObject tutoPanel;
    [SerializeField]
    TextMeshProUGUI resultDesc;

    [SerializeField]
    public GameObject[] gameMaps;

    [SerializeField]
    Slider timeSlider;

    [Header("# Variables")]
    int curMapIndex = 0; // 현재 맵 인덱스
    
    public int CurMapIndex
    {
        get => curMapIndex;
        set
        {
            if (curMapIndex < 0 || value == curMapIndex)
                return;

            gameMaps[curMapIndex].SetActive(false); // 이전 맵 끄기
            curMapIndex = value; // 현재 맵 인덱스 업데이트
            gameMaps[curMapIndex].SetActive(true); // 새 맵 켜기

            dLight.color = mapColors[curMapIndex];
        }
    }

    [SerializeField]
    float maxGameTime = 120f; // 2 minutes
    public float currentGameTime = 0f;

    private void Update()
    {
        if (GameManager.instance.CurScene == SceneType.Main 
            || GameManager.instance.IsPause)
            return;

        if (_dataMgr.gameData.isFirstTime) Tutorial();
        
        if (currentGameTime < maxGameTime)
        {
            currentGameTime += Time.deltaTime;
            timeSlider.value = currentGameTime / maxGameTime;
        }
        else
        {
            GameClear();
        }
    }
    
    public override void Initialize()
    {
        
    }

    void Tutorial()
    {
        Time.timeScale = 0f;
        
        tutoPanel.SetActive(true);
    }

    /// <summary>
    /// 게임 시작 시 랜덤으로 맵 선택 함수
    /// </summary>
    public void RandMap()
    {
        CurMapIndex = Random.Range(0, gameMaps.Length);

        // 맵 켜주기
        gameMaps[CurMapIndex].SetActive(true);
        // 맵 이름 글자 바꾸기
        mapNameText.text = gameMaps[CurMapIndex].name;
    }


    #region 승호

    public void GameClear()
    {
        GameManager.instance.IsPause = true; // 게임 일시 정지
        
        // 최고 점수 갱신
        BestRecordCheck();
        
        // 게임 클리어 UI 띄우기
        resultPanel.SetActive(true);
        
        // 클리어 설명 표시
        resultDesc.text = $"Best Score : {_dataMgr.gameData.bestScore:F0}\n" +
                          $"Current Score : {_scoreMgr.targetScore:F0}\n\n" +
                          $"Best Time : {_dataMgr.gameData.bestTime:F2}\n" +
                          $"Current Time : {currentGameTime:F2}";
    }

    #endregion

    void BestRecordCheck()
    {
        _scoreMgr.ClearScore();
        
        int curScore = (int)_scoreMgr.CurScore;
        int bestScore = _dataMgr.gameData.bestScore;
        float bestTime = _dataMgr.gameData.bestTime;

        if (curScore > bestScore) _dataMgr.gameData.bestScore = curScore;
        if (currentGameTime > bestTime) _dataMgr.gameData.bestTime = currentGameTime;
    }
    
    public void PullUseManager()
    {
        _dataMgr = CoreManager.instance.GetManager<DataManager>();
        _scoreMgr = CoreManager.instance.GetManager<ScoreManager>();
    }
}
