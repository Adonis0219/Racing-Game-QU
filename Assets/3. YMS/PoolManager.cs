// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public enum PoolObjectType
// {
//     O_Tree, O_Rock,
//     I_SpeedUp, I_Recovery,
//     P_Explosion
// }
//
// [Serializable]
// public class PoolObjectData
// {
//     public PoolObjectType type;
//     public int initCount;
//     public GameObject original;
// }
//
// public class PoolManager : MonoBehaviour, IPullManager
// {
//     private StageManager _stageMgr;
//     public static PoolManager instance;
//     
//
//     [SerializeField]    
//     public PoolObjectData[] poolObjDatasDesert; 
//     
//     [SerializeField]    
//     public PoolObjectData[] poolObjDatasCity; 
//     
//     [SerializeField]    
//     public PoolObjectData[] poolObjDatasBeach; 
//     
//     [SerializeField]    
//     public PoolObjectData[] poolObjDatasPublic; 
//     
//     Dictionary<PoolObjectType, Queue<GameObject>> objectDict;
//     
//     Transform[] parents;
//
//     public bool initSpawn = false;
//
//     private void Awake()
//     {
//         instance = this;
//
//         objectDict = new Dictionary<PoolObjectType, Queue<GameObject>>();
//     }
//
//     private IEnumerator Start()
//     {
//         yield return new WaitUntil(()=>initSpawn);
//         CreateParents();
//         InitPoolSet();
//         initSpawn = false;
//     }
//
//     void InitPoolSet()
//     {
//         switch (_stageMgr.CurMapIndex)
//         {
//             case 0:
//                 foreach (var data in poolObjDatasDesert)
//                 {
//                     Queue<GameObject> newQueue = new Queue<GameObject>();
//                     objectDict[data.type] = newQueue;
//         
//                     for (int i = 0; i < data.initCount; i++)
//                     {
//                         newQueue.Enqueue(CreateObject(data));
//                     }
//                 }
//                 break;
//             case 1:
//                 foreach (var data in poolObjDatasCity)
//                 {
//                     Queue<GameObject> newQueue = new Queue<GameObject>();
//                     objectDict[data.type] = newQueue;
//         
//                     for (int i = 0; i < data.initCount; i++)
//                     {
//                         newQueue.Enqueue(CreateObject(data));
//                     }
//                 }
//                 break;
//             case 2:
//                 foreach (var data in poolObjDatasBeach)
//                 {
//                     Queue<GameObject> newQueue = new Queue<GameObject>();
//                     objectDict[data.type] = newQueue;
//         
//                     for (int i = 0; i < data.initCount; i++)
//                     {
//                         newQueue.Enqueue(CreateObject(data));
//                     }
//                 }
//                 break;
//         }
//         
//         foreach (var data in poolObjDatasPublic)
//         {
//             Queue<GameObject> newQueue = new Queue<GameObject>();
//             objectDict[data.type] = newQueue;
//         
//             for (int i = 0; i < data.initCount; i++)
//             {
//                 newQueue.Enqueue(CreateObject(data));
//             }
//         }
//     }
//     
//     void CreateParents()
//     {
//         int mapDataLength = 0;
//         
//         Debug.Log($"mapIndex: {_stageMgr.CurMapIndex}");
//         switch (_stageMgr.CurMapIndex)
//         {
//             case 0:
//                 parents = new Transform[poolObjDatasDesert.Length+poolObjDatasPublic.Length];
//
//                 for (int i = 0; i < poolObjDatasDesert.Length; i++)
//                 {
//                     GameObject parent = new GameObject(poolObjDatasDesert[i].type.ToString());
//                     parent.transform.SetParent(transform);
//                     parents[i] = parent.transform;
//                 }
//                 mapDataLength = poolObjDatasDesert.Length;
//                 break;
//             case 1:
//                 parents = new Transform[poolObjDatasCity.Length+poolObjDatasPublic.Length];
//
//                 for (int i = 0; i < poolObjDatasCity.Length; i++)
//                 {
//                     GameObject parent = new GameObject(poolObjDatasCity[i].type.ToString());
//                     parent.transform.SetParent(transform);
//                     parents[i] = parent.transform;
//                 }
//                 mapDataLength = poolObjDatasCity.Length;
//                 break;
//             case 2:
//                 parents = new Transform[poolObjDatasBeach.Length+poolObjDatasPublic.Length];
//
//                 for (int i = 0; i < poolObjDatasBeach.Length; i++)
//                 {
//                     GameObject parent = new GameObject(poolObjDatasBeach[i].type.ToString());
//                     parent.transform.SetParent(transform);
//                     parents[i] = parent.transform;
//                 }
//                 mapDataLength = poolObjDatasBeach.Length;
//                 break;
//         }
//         for (int i = 0; i < poolObjDatasPublic.Length; i++)
//         {
//             GameObject parent = new GameObject(poolObjDatasPublic[i].type.ToString());
//             parent.transform.SetParent(transform);
//             parents[i+mapDataLength] = parent.transform;
//         }
//         
//         // parents = new Transform[poolObjDatas.Length];
//         //
//         // for (int i = 0; i < poolObjDatas.Length; i++)
//         // {
//         //     if (poolObjDatas[i].original.GetComponent<IObstacle>()?.objectMapType != (MapType)_stageMgr.CurMapIndex) continue;
//         //     Debug.Log(poolObjDatas[i].type);
//         //     GameObject parent = new GameObject(poolObjDatas[i].type.ToString());
//         //     parent.transform.SetParent(transform);
//         //
//         //     parents[i] = parent.transform;
//         // }
//     }
//     
//     GameObject CreateObject(PoolObjectData data)
//     {
//         int createIndex = (int)data.type;
//         
//         GameObject obj = Instantiate(data.original, parents[createIndex]);
//         obj.name = data.type.ToString();
//         obj.SetActive(false);
//
//         return obj;
//     }
//     
//     public GameObject GetPool(PoolObjectType type)
//     {
//         if (!objectDict.ContainsKey(type))
//         {
//             Debug.LogWarning($"PoolManager : {type}�� ���� Ǯ ������ �����ϴ�");
//             return null;
//         }
//         
//         var queue = objectDict[type];
//         GameObject returnObj;
//        
//         if (queue.Count == 0)
//         {
//             if ((int)type > 1)
//             {
//                 var publicData = poolObjDatasPublic[(int)type-2];
//                 returnObj = CreateObject(publicData);
//             }
//             else
//             {
//                 switch (_stageMgr.CurMapIndex)
//                 {
//                     case 0:
//                         var desertData = poolObjDatasDesert[(int)type];
//                         returnObj = CreateObject(desertData);
//                         break;
//                     case 1:
//                         var cityData = poolObjDatasCity[(int)type];
//                         returnObj = CreateObject(cityData);
//                         break;
//                     case 2:
//                         var beachData = poolObjDatasBeach[(int)type];
//                         returnObj = CreateObject(beachData);
//                         break;
//                     default:
//                         returnObj = CreateObject(poolObjDatasBeach[(int)type]);
//                         break;
//                 }
//             }
//         }
//         else
//         {
//             returnObj = queue.Dequeue();
//         }
//
//         returnObj.SetActive(true);
//
//         return returnObj;
//     }
//
//     public void SetPool(GameObject setObj, PoolObjectType type)
//     {
//         if (!objectDict.ContainsKey(type))
//         {
//             Debug.LogWarning($"PoolManager: {type}�� ���� Ǯ ������ �����ϴ�.");
//             return; 
//         }
//
//         setObj.transform.ResetForce();
//
//         setObj.SetActive(false);
//         objectDict[type].Enqueue(setObj);
//     }
//
//     public void ClearPool()
//     {
//         for (int i = 0; i < transform.childCount; i++)
//         {
//             Debug.Log(transform.GetChild(i).gameObject.name);
//             Destroy(transform.GetChild(i).gameObject);
//         }
//         // foreach (var parent in parents)
//         // {
//         //     parents.
//         //     foreach (Transform child in parent)
//         //     {
//         //         child.gameObject.SetActive(false);
//         //     }
//         // }
//     }
//
//     public void PullUseManager()
//     {
//         _stageMgr = CoreManager.instance.GetManager<StageManager>();
//     }
// }
