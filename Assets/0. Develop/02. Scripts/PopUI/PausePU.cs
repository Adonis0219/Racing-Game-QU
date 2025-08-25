using UnityEngine;
using UnityEngine.UI;

public class PausePU : PopupUIController
{
    [SerializeField] private Button eixtBt; // Continue button

    public override void Start()
    {
        base.Start();

        // Continue button event listener
        eixtBt.onClick.AddListener(() => OnEixtBtClk());
    }

    public override void OnOpenBtClk()
    {
        popupUI.SetActive(true);

        GameManager.instance.IsPause = true; // Pause the game
    }

    // 계속하기
    public override void OnCloseBtClk()
    {
        popupUI.SetActive(false);

        GameManager.instance.IsPause = false; // Resume the game
    }

    public void OnEixtBtClk()
    {
        popupUI.SetActive(false);
    }
}
