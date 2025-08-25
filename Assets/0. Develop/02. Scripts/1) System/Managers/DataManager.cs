using UnityEngine;

public class GameData
{
    // Score
    public int bestScore; // 최고 점수

    public float bestTime;
    
    // Audio
    public float bgmVolume = 1.0f;
    public float sfxVolume = 1.0f;
    public bool bgmMute = false;
    public bool sfxMute = false;

    public bool isFirstTime = true;

    // JsonUtility -> 원시타입만가능

    // 설정해놓은 값 float[]
    // 스테이지 클리어 여부 bool이나 비트마스크

    // class 타입
}

public class DataManager : BaseManager
{
    public GameData gameData; // 게임 데이터를 저장하는 변수

    const string SAVE_KEY = "GameData"; // PlayerPrefs에 저장할 키
    
    public override void Initialize()
    {
        LoadData(); // 게임 시작 시 데이터 로드
    }

    // 게임 종료 시 데이터 저장
    private void OnApplicationQuit()
    {
        SaveData();
    }

    // 게임 데이터가 복잡해지면 JsonConvert 사용
    // string saveJD = JsonConvert.serializeObject(gameData);
    
    public void SaveData()
    {
        string saveJD = JsonUtility.ToJson(gameData);
        PlayerPrefs.SetString(SAVE_KEY, saveJD); // PlayerPrefs에 저장
    }

    public void LoadData()
    {
        gameData = JsonUtility.FromJson<GameData>(PlayerPrefs.GetString(SAVE_KEY, "")); // JSON 문자열을 GameData 객체로 변환

        if (gameData == null)
        {
            gameData = new GameData(); // 데이터가 없으면 새로 생성
        }
    }
}
