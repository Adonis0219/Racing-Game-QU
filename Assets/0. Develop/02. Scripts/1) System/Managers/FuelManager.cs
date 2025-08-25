using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class FuelManager : BaseManager, IPullManager, IResetable
{
    //====Manager
    StageManager _stageMgr;


    float maxFuel = 100;
    public float curruntFuel = 100;
    float minusPerDelta = 2f;
    bool isGameOver = false;

    [SerializeField]
    Slider fuelSlider;

    void OnEnable()
    {
        GameManager.instance.onSceneChange += ResetGame;
    }

    void Update()
    {
        if (curruntFuel <= 0f)
        {
            if (!isGameOver)
            {
                isGameOver = true;
                _stageMgr.GameClear();
            }
            return;
        }
        if (GameManager.instance.CurScene != SceneType.Game)
            return;
        curruntFuel -= minusPerDelta * Time.deltaTime;

        fuelSlider.value = curruntFuel / maxFuel;
    }

    public void CalculateFuel(float plus)
    {
        curruntFuel = curruntFuel + plus;
        curruntFuel = Mathf.Clamp(curruntFuel, 0, maxFuel);
    }
    public void ChangeMaxFuel(float max)
    {
        maxFuel=max;
        curruntFuel=max;
    }
    public override void Initialize()
    {
    }

    public void ResetGame(SceneType scene)
    {
        if (scene == SceneType.Game)
        {
            curruntFuel = maxFuel;
            isGameOver=false;
        }
    }

    public void PullUseManager()
    {
        _stageMgr=CoreManager.instance.GetManager<StageManager>();
    }
}
