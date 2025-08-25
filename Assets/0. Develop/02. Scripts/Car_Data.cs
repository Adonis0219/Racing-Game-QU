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

    [Header("## �̱���")]
    public int invincibleTime;
    public float speedMulti;

    // public GameObject oriCar;
    // �Ҹ�Ÿ�� Ÿ�� ������ �ּ� �ִ�
    // �Ҹ�Ÿ�� ���� ���� Ÿ�� ������ ��
}
