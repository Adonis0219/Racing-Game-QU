using System;
using System.Collections;
using TMPro;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

class ComboInfo
{
    public Transform comboPanel;
    public int comboCount;
    public float comboFactor;
    public float curComboTime;
    public float maxComboTime;
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI factorText;
    public string comboStr;
    public Slider timeSlider;
}

public class ScoreManager : BaseManager, IPullManager
{
    // ------- Managers -------
    private DataManager _dataMgr;

    private Player player;
    
    [Header("# UI")]
    public TextMeshProUGUI curScoreText;
    public TextMeshProUGUI upScoreText;

    public Transform dodgePanel;
    public Transform crashPanel;

    #region Combo Variable

    [Header("# Variable")]
    bool IsNoCombo => DodgeCombo == 0 && CrashCombo == 0;
    /// <summary>
    /// NoCombo가 되기 전 최대 점수
    /// </summary>
    float highUpScore;

    ComboInfo dodgeCom;
    readonly float DODGE_FACTOR = .3f;
    float dFactor;

    public int DodgeCombo
    {
        get => dodgeCom.comboCount;

        set
        {
            dodgeCom.comboCount = value;

            dFactor = 1 + DodgeCombo * DODGE_FACTOR;

            NoComboChk();

            AddComboCount(dodgeCom);
        }
    }

    ComboInfo crashCom;
    readonly float CRASH_FACTOR = .5f;
    float cFactor;

    public int CrashCombo
    {
        get => crashCom.comboCount;
        set
        {
            crashCom.comboCount = value;

            cFactor = 1 + CrashCombo * CRASH_FACTOR;

            NoComboChk();

            AddComboCount(crashCom);
        }
    }

    #endregion

    #region Score Variable

    [Tooltip("초당 점수 UI가 업데이트되는 속도")]
    public float scoreUpdateSpeed = 1000f;

    // 스코어 아이템으로 올라갈 점수 (난이도에 따라 배열)
    [Tooltip("기본 추가 점수")]
    public float basicUpScore = 1000;

    private float curScore;

    public float CurScore
    {
        get => curScore;
        set
        {
            curScore = value;

            curScoreText.text = curScore.ToString("F0");
        }
    }
    
    [SerializeField]
    public float targetScore;

    private float upScore;

    #endregion

    private void Update()
    {
        if (GameManager.instance.CurScene == SceneType.Main) return;

        if (DodgeCombo != 0)
            CalculateTime(dodgeCom);
        if (CrashCombo != 0)
            CalculateTime(crashCom);

        ScoreUp();
    }

    #region Init Method

    public override void Initialize()
    {
        CreateCombo();
    }

    void CreateCombo()
    {
        dodgeCom = new ComboInfo()
        {
            comboPanel = dodgePanel,
            comboCount = 0,
            comboFactor = DODGE_FACTOR,
            maxComboTime = 5f,
            curComboTime = 0,
            comboText = dodgePanel.GetChild(0).GetComponent<TextMeshProUGUI>(),
            factorText = dodgePanel.GetChild(1).GetComponent<TextMeshProUGUI>(),
            comboStr = $"Dodge Combo x",
            timeSlider = dodgePanel.GetChild(2).GetComponent<Slider>()
        };

        crashCom = new ComboInfo()
        {
            comboPanel = crashPanel,
            comboCount = 0,
            comboFactor = CRASH_FACTOR,
            maxComboTime = 10f,
            curComboTime = 0,
            comboText = crashPanel.GetChild(0).GetComponent<TextMeshProUGUI>(),
            factorText = crashPanel.GetChild(1).GetComponent<TextMeshProUGUI>(),
            comboStr = $"Crash Combo x",
            timeSlider = crashPanel.GetChild(2).GetComponent<Slider>()
        };
    }

    public void PullUseManager()
    {
        _dataMgr = CoreManager.instance.GetManager<DataManager>();
        player = CoreManager.instance.player;
    }

    #endregion

    #region Combo Method
    /// <summary>
    /// 콤보가 끊겼는지 체크 후 행동 실행
    /// </summary>
    void NoComboChk()
    {
        if (IsNoCombo)
        {
            targetScore += upScore;
            highUpScore = 0;
        }
    }

    /// <summary>
    /// 시간 재기 및 초기화
    /// </summary>
    /// <param name="combo">콤보 타입</param>
    void CalculateTime(ComboInfo combo)
    {
        combo.curComboTime -= Time.deltaTime;

        if (combo.curComboTime < 0)
        {
            if (combo == dodgeCom)
            { 
                DodgeCombo = 0;
            }
            else
                CrashCombo = 0;
        }
        combo.timeSlider.value = combo.curComboTime / combo.maxComboTime;
    }

    /// <summary>
    /// 콤보 카운트가 오를 때 행동
    /// </summary>
    /// <param name="combo"></param>
    void AddComboCount(ComboInfo combo)
    {
        #region Method Varialbe

        bool isCombo = combo.comboCount != 0;
        float factor = combo == dodgeCom ? dFactor : cFactor;

        #endregion

        // 콤보 텍스트 On/Off
        upScoreText.gameObject.SetActive(true);
        combo.comboPanel.gameObject.SetActive(isCombo);

        // 콤보 시간 최대 시간으로
        combo.curComboTime = combo.maxComboTime;

        // 콤보 UI 세팅
        combo.comboText.text = combo.comboStr + combo.comboCount;

        // 배율 및 점수 계산
        combo.factorText.text = "x" + factor;

        upScore = basicUpScore * (cFactor == 0 ? 1 : cFactor) * (dFactor == 0 ? 1 : dFactor);

        // 콤보가 끊기기 전까진 최대 점수만 출력
        if (highUpScore >= upScore)
        {
            upScore = highUpScore;
        }

        upScoreText.text = "+" + upScore;

        // 콤보가 변경되기 전 현재 점수를 최대 점수로 저장
        highUpScore = upScore;
    }

    #endregion

    #region Score Method

    /// <summary>
    /// 게임 클리어 시 현재 콤보 점수 바로 더해주기
    /// </summary>
    public void ClearScore()
    {
        targetScore += upScore;
    }
    
    /// <summary>
    /// 매초 점수 올려주는 함수
    /// </summary>
    public IEnumerator OnScoreUp()
    {
        while (true)
        {
            // 지속적으로 점수를 올려주되, 현재 점수가 아닌 목표 점수에 더합니다.
            // 화면에 보이는 점수는 ScoreUp() 메서드에서 부드럽게 따라잡을 것입니다.
            targetScore += 100;

            yield return Util.GetWaitForSeconds(1f);
        }
    }

    void ScoreUp()
    {
        upScoreText.gameObject.SetActive(!IsNoCombo);

        // Mathf.MoveTowards를 사용하여 현재 점수를 목표 점수까지 일정한 속도로 증가시킵니다.
        // Lerp보다 더 부드러운 변화 (Lerp는 약간 급발진)
        CurScore = Mathf.MoveTowards(CurScore, targetScore, scoreUpdateSpeed * Time.deltaTime);

        #region Lerp 이용 방식

        /* 현재 점수(CurScore)가 목표 점수(targetScore)에 도달하도록 부드럽게 보간합니다.
        // Mathf.Lerp를 사용하여 점수가 부드럽게 올라가는 효과를 만듭니다.
        
        //upTime = .25f;
        if (!Mathf.Approximately(CurScore, targetScore))
        {
            // upTime 변수를 속도 계수로 사용하여 Lerp의 세 번째 인자로 전달합니다.
            // upTime 몇 초만에 목표에 도달할 것인지
            CurScore = Mathf.Lerp(CurScore, targetScore, Time.deltaTime / upTime);
        }
        else
        {
            // 부동 소수점 오차를 방지하기 위해 목표 점수와 매우 가까워지면 값을 정확히 맞춰줍니다.
            CurScore = targetScore;
        }*/
        #endregion
    }

    #endregion
}