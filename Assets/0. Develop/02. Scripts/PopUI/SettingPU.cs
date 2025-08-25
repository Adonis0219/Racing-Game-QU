using UnityEngine;

public class SettingPU : PopupUIController
{
    public override void OnOpenBtClk()
    {
        popupUI.SetActive(true);

        GameManager.instance.IsPause = true; // Pause the game
    }

    public override void OnCloseBtClk()
    {
        popupUI.SetActive(false);

        GameManager.instance.IsPause = false; // Resume the game
    }
}
