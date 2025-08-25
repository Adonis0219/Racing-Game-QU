using UnityEngine;
using UnityEngine.UI;

public class SelectPU : PopupUIController
{
    [SerializeField]
    Button[] selectBts;
    [SerializeField]
    GameObject[] checks;

    // üũ ��ũ�� �����ִ� �ε���
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
    /// �� ���� ȭ���� ���� �� -> ���� ����
    /// </summary>
    public override void OnCloseBtClk()
    {
        popupUI.SetActive(false);

        GameManager.instance.CurScene = SceneType.Game; // �� Ÿ���� �������� ����
    }

    /// <summary>
    /// ���� ���� Ŭ��
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
