using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class HideSpawner : BaseSpawner, IPullManager
{
    // ======= Managers =======
    private StageManager _stageMgr;
    
    [SerializeField]
    int spawnTime = 3;

    protected override int SpawnTime { get => spawnTime; set => spawnTime = 3; }

    new void Start()
    {
        base.Start();

        spawnPosX = new float[] { -2f, -1f, 0, 1f, 2f };
    }

    public override void SpawnConnect()
    {
        #region Method Param
        int mapIndex = _stageMgr.CurMapIndex;
        //PoolObjectType type = (PoolObjectType)(mapIndex + 5);
        PoolObjectType type = (PoolObjectType)(mapIndex + 5);
        
        int randIndex = UnityEngine.Random.Range(0, spawnPosX.Length);
        float randX = spawnPosX[randIndex];
        #endregion

        _factory.Spawn(type, randX, 0);
    }

    public void PullUseManager()
    {
        _stageMgr = CoreManager.instance.GetManager<StageManager>();
    }
}