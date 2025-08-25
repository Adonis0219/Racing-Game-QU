using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ObstacleSpawner : BaseSpawner
{
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
        PoolObjectType randType = (PoolObjectType)UnityEngine.Random.Range(0, 2);
 
        //Debug.Log(randType);
        
        int randIndex = UnityEngine.Random.Range(0, spawnPosX.Length);
        float randX = spawnPosX[randIndex];
        #endregion

        _factory.Spawn(randType, randX, 0);
        //_factory.Spawn(PoolObjectType.O_Tree, randX, 0);
    }
}