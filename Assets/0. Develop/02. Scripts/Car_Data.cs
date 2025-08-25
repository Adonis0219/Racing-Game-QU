using UnityEngine;

[CreateAssetMenu(fileName = "Car_Data", menuName = "Scriptable Objects/Car_Data")]
public class Car_Data : ScriptableObject
{
    [Header("# Car Info")]
    public string carName;
    public string carDesc;
    public int carIndex;

    [Header("# Car Stats")]
    public float maxFuel;
    public float oriSpeed;
    public float maxCharge;
    public float chargePerDelta;

    [Header("## 미구현")]
    public int invincibleTime;
    public float speedMulti;

    // public GameObject oriCar;
    // 불릿타임 타임 스케일 최소 최대
    // 불릿타임 시작 시의 타임 스케일 값
}
