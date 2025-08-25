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

    public event Action<SceneType> onSceneChange; // �� ���� �̺�Ʈ

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
            
            onSceneChange?.Invoke(value); // �� ���� �̺�Ʈ ȣ��
        }
    }

    public bool IsPause
    {
        get => isPause;
        set
        {
            isPause = value;

            Time.timeScale = isPause ? 0f : 1f; // ���� �Ͻ� ���� �Ǵ� �簳
        }
    }

    int curCarIndex = 0;    // �⺻ ���� 0

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

        onSceneChange += ChangeScene; // �� ���� �̺�Ʈ�� ChangeScene �޼��� ���
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
    /// ���� ��ȯ�ϴ� �Լ�
    /// </summary>
    /// <param name="type">�ٲ��� ��</param>
    public void ChangeScene(SceneType type)
    {
        _dataMgr.SaveData();
        
        bool isMain = type == SceneType.Main ? true : false;
        
        // ���ڰ��� ���� �� On / Off
        uis[(int)SceneType.Main].SetActive(isMain);
        uis[(int)SceneType.Game].SetActive(!isMain);

        // �ڵ��� ����
        PlayerCarSet(isMain);
        _audioMgr.BgmSoundPlay(isMain);

        if (isMain)
        {
            SceneManager.LoadScene(0);
            // ���� ������ ���� ��
            /// Ǯ�Ŵ����� �����ִ� ��� ������Ʈ ����
            PoolManager.instance.ClearPool();

            StopCoroutine(scoreCoru);
        }
        else
        {
            _stageMgr.RandMap(); // ���� ������ �Ѿ �� ���� �� ����

            _stageMgr.currentGameTime = 0f; // ���� �ð� �ʱ�ȭ

            scoreCoru = _scoreMgr.StartCoroutine(_scoreMgr.OnScoreUp());
        }
    }

    /// <summary>
    /// ������ ������ ���� �ٲ��ִ� �Լ�
    /// </summary>
    /// <param name="isGameScene">���Ӿ����� ������</param>
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
