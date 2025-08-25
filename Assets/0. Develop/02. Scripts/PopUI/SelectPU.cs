using UnityEngine;
using UnityEngine.UI;

public class SelectPU : PopupUIController
{
    [SerializeField]
    Button[] selectBts;
    [SerializeField]
    GameObject[] checks;

    // 체크 마크가 켜져있는 인덱스
    int prevIndex;

    public override void Start()
    {
        base.Start();

        for (int i = 0; i < selectBts.Length; i++)
        {
            int index = i;
            selectBts[index].onClick.AddListener(() => CarBtClk(index));
        }
    }

    /// <summary>
    /// 차 선택 화면을 닫을 때 -> 게임 시작
    /// </summary>
    public override void OnCloseBtClk()
    {
        popupUI.SetActive(false);

        GameManager.instance.CurScene = SceneType.Game; // 씬 타입을 게임으로 변경
    }

    /// <summary>
    /// 게임 시작 클릭
    /// </summary>
    public override void OnOpenBtClk()
    {
        AudioManager.instance.PlaySfx(Sfx.GameStart);
        
        popupUI.SetActive(true);
    }

    public void CarBtClk(int index)
    {
        prevIndex = GameManager.instance.CurCarIndex;

        checks[prevIndex].SetActive(false);
        GameManager.instance.CurCarIndex = index;
        checks[index].SetActive(true);
    }
}
