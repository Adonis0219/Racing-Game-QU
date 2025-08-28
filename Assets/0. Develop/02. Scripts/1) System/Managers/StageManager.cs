
using System;
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
    int curMapIndex = 0; // ���� �� �ε���
    
    public int CurMapIndex
    {
        get => curMapIndex;
        set
        {
            if (curMapIndex < 0 || value == curMapIndex)
                return;
            
            gameMaps[curMapIndex].SetActive(false); // ���� �� ����
            curMapIndex = value; // ���� �� �ε��� ������Ʈ
            gameMaps[curMapIndex].SetActive(true); // �� �� �ѱ�

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
    /// ���� ���� �� �������� �� ���� �Լ�
    /// </summary>
    public void RandMap()
    {
        CurMapIndex = Random.Range(0, gameMaps.Length);

        // �� ���ֱ�
        gameMaps[CurMapIndex].SetActive(true);
        // �� �̸� ���� �ٲٱ�
        mapNameText.text = gameMaps[CurMapIndex].name;
        //PoolManager.instance.initSpawn = true;  주석 풀어야됨
    }


    #region ��ȣ

    public void GameClear()
    {
        GameManager.instance.IsPause = true; // ���� �Ͻ� ����
        
        // �ְ� ���� ����
        BestRecordCheck();
        
        // ���� Ŭ���� UI ����
        resultPanel.SetActive(true);
        
        // Ŭ���� ���� ǥ��
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
