using UnityEngine;
using UnityEngine.UI;

public class ResultPU : PopupUIController, IPullManager
{
    // ------- Managers -------
    private StageManager _stageMgr;
    
    public override void OnOpenBtClk()
    { 
        _stageMgr.GameClear();
    }

    public override void OnCloseBtClk()
    {
        GameManager.instance.IsPause = false; // 시간 흐르게 하기

        popupUI.SetActive(false);

        // Exit to the main menu logic
        GameManager.instance.CurScene = SceneType.Main; // Change to Main scene
    }

    void OnRestartBtClk()
    {
        popupUI.SetActive(false); // Close the result popup

        // Restart the game logic
        GameManager.instance.CurScene = SceneType.Game; // Change to Game scene

        GameManager.instance.IsPause = false; // Resume the game
    }

    public void PullUseManager()
    {
        _stageMgr = CoreManager.instance.GetManager<StageManager>();
    }
}
