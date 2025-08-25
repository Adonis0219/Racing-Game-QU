using System;
using System.Collections;
using UnityEngine;

public class ObjectFactory : MonoBehaviour
{
    public void Spawn(PoolObjectType type, float spawnPosX, float spawnPosZ)
    {
        GameObject obj = PoolManager.instance.GetPool(type);

        if (obj != null)
        {
            obj.transform.position = new Vector3(spawnPosX, 12, spawnPosZ);
        }
    }

    #region 민수
    public void Spawn(PoolObjectType type, Vector3 spawnPos)
    {
        GameObject obj = PoolManager.instance.GetPool(type);

        if (obj != null)
        {
            obj.transform.position = spawnPos;
            obj.GetComponent<ParticleSystem>()?.Play();
        }

        StartCoroutine(StopParticle(obj));
    }

    IEnumerator StopParticle(GameObject obj)
    {
        ParticleSystem ps = obj.GetComponent<ParticleSystem>();
        while (ps.isPlaying)
        {
            yield return null;
        }
        ps.Stop();
    }
    

    #endregion
}
