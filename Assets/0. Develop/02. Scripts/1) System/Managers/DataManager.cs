using UnityEngine;

public class GameData
{
    // Score
    public int bestScore; // �ְ� ����

    public float bestTime;
    
    // Audio
    public float bgmVolume = 1.0f;
    public float sfxVolume = 1.0f;
    public bool bgmMute = false;
    public bool sfxMute = false;

    public bool isFirstTime = true;

    // JsonUtility -> ����Ÿ�Ը�����

    // �����س��� �� float[]
    // �������� Ŭ���� ���� bool�̳� ��Ʈ����ũ

    // class Ÿ��
}

public class DataManager : BaseManager
{
    public GameData gameData; // ���� �����͸� �����ϴ� ����

    const string SAVE_KEY = "GameData"; // PlayerPrefs�� ������ Ű
    
    public override void Initialize()
    {
        LoadData(); // ���� ���� �� ������ �ε�
    }

    // ���� ���� �� ������ ����
    private void OnApplicationQuit()
    {
        SaveData();
    }

    // ���� �����Ͱ� ���������� JsonConvert ���
    // string saveJD = JsonConvert.serializeObject(gameData);
    
    public void SaveData()
    {
        string saveJD = JsonUtility.ToJson(gameData);
        PlayerPrefs.SetString(SAVE_KEY, saveJD); // PlayerPrefs�� ����
    }

    public void LoadData()
    {
        gameData = JsonUtility.FromJson<GameData>(PlayerPrefs.GetString(SAVE_KEY, "")); // JSON ���ڿ��� GameData ��ü�� ��ȯ

        if (gameData == null)
        {
            gameData = new GameData(); // �����Ͱ� ������ ���� ����
        }
    }
}
