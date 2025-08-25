using UnityEngine;
using System.Collections.Generic;
using System;
public class PlayerColliderSystem : MonoBehaviour, IPullManager
{   
    // ------- Managers -------
    FuelManager _fuelMgr;

    [SerializeField]
    ParticleSystem healParticle;
    public Dictionary<string, Action> itemEffects;
    
    public void InitSet()
    {
        itemEffects = new Dictionary<string, Action>();
        itemEffects.Add("Recovery",RecovertFuel);
    }

    void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.layer == LayerMask.NameToLayer("Item"))
        {
            string itemName = other.name;
            foreach (var key in itemEffects.Keys)
            {
                if (itemName.Contains(key))
                {
                    itemEffects[key]?.Invoke();
                    break;
                }
            }

        }

        if (other.gameObject.layer == LayerMask.NameToLayer("ObstacleTrigger"))
        {//장애물 감지
            GetComponent<Player>().isHaveNearObj = true;
        }

    }
    void RecovertFuel()
    {
        _fuelMgr.CalculateFuel(10);    
        healParticle.Play();
    }

    public void PullUseManager()
    {
        _fuelMgr = CoreManager.instance.GetManager<FuelManager>();
    }
}
