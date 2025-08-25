using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

// // 배열 프로퍼티 인덱서
// class SfxVolume
// {
//     private float[] sfxVolumes;
//     
//     // 배열 프로퍼티
//     public float this[int index]
//     {
//         get => sfxVolumes[index];
//         set => sfxVolumes[index] = value;
//     }
// }

public enum Sfx
{
    GameStart ,Click,Move,ChargeMove
}

public class AudioManager : BaseManager, IPullManager
{
    // ------- Managers -------
    private DataManager _dataMgr;
    
    public static AudioManager instance;
    
    [Header("# BGM")]
    [SerializeField]        // 브금 조정 슬라이더
    private Slider bgmSlider;
    [SerializeField]
    private Toggle bgmMute;
    
    private AudioSource bgmPlayer;
    public AudioClip[] bgmClips;
    
    [Header("# SFX")]
    [SerializeField]        // 사운드 이펙트 조정 슬라이더
    private Slider sfxSlider;
    [SerializeField]
    private Toggle sfxMute;
    
    private AudioSource[] sfxPlayers;
    [Tooltip("GameStart, Click")]
    public AudioClip[] sfxClips;
    // 효과음 채널 개수
    public int channels;
    // 효과음 채널 인덱스
    private int channelIndex;

    #region Property

    public float BgmVolume
    {
        get => _dataMgr.gameData.bgmVolume;
        set
        {
            _dataMgr.gameData.bgmVolume = value;

            bgmPlayer.volume = BgmVolume;
        }
    }

    // 추후에 효과음 클립의 볼륨이 각각 다를 경우
    // 추가적인 작업
    public float SfxVolume
    {
        get => _dataMgr.gameData.sfxVolume;
        set 
        {
            _dataMgr.gameData.sfxVolume = value;

            foreach (var player in sfxPlayers)
            {
                player.volume = SfxVolume;
            }
        }
    }

    public bool IsSfxMute
    {
        get => _dataMgr.gameData.sfxMute;
        set
        {
            _dataMgr.gameData.sfxMute = value;

            foreach (var player in sfxPlayers)
            {
                player.mute = IsSfxMute;
            }
        }
    }

    public bool IsBgmMute
    {
        get => _dataMgr.gameData.bgmMute;
        set
        {
            _dataMgr.gameData.bgmMute = value;

            bgmPlayer.mute = IsBgmMute;
        }
    }

    #endregion

    private void Start()
    {
        BgmInitSet();
        SfxInitSet();

        BgmSoundPlay(true);
    }

    #region Init Method

    public override void Initialize()
    {
        instance = this;
        
        LinkToListener();
    }
    
    public void PullUseManager()
    {
        _dataMgr = CoreManager.instance.GetManager<DataManager>();
    }

    /// <summary>
    /// 브금 초기 세팅 함수
    /// </summary>
    void BgmInitSet()
    {
        // 브금 플레이어 초기화
        // 오브젝트 생성
        GameObject bgmObj = new GameObject("BgmPlayer");
        // 부모를 오디오 매니저로
        bgmObj.transform.SetParent(transform);
        // 브금 오브젝트에 오디오소스 컴포넌트 추가 후 브금 플레이어에 할당
        bgmPlayer = bgmObj.AddComponent<AudioSource>();
        // 브금 무한반복 설정
        bgmPlayer.loop = true;
        
        // 오디오 소스 볼륨값 슬라이더값으로 넘겨주rl
        bgmSlider.value = BgmVolume;
        bgmPlayer.volume = BgmVolume;
        
        // 오디오 소스 뮤트 여부 토글값으로 넘겨주기
        bgmMute.isOn = IsBgmMute;
        bgmPlayer.mute = IsBgmMute;
    }

    /// <summary>
    /// 효과음 초기 세팅 함수
    /// </summary>
    void SfxInitSet()
    {
        // 효과음 플레이어 초기화
        GameObject sfxObj = new GameObject("SfxPlayer");
        sfxObj.transform.SetParent(transform);
        // 채널 개수만큼 오디오 소스 추가
        sfxPlayers = new AudioSource[channels];
        
        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            // 각 인덱스의 플레이어에 오디오 소스 추가 후 할당
            sfxPlayers[i] = sfxObj.AddComponent<AudioSource>();
            // 게임 처음 실행 시 플레이 안 하도록
            sfxPlayers[i].playOnAwake = false;
            sfxPlayers[i].volume = SfxVolume;
            sfxPlayers[i].mute = sfxMute;
        }
        
        // 현재 효과음 볼륨값 슬라이더에 넘겨주기
        sfxSlider.value = SfxVolume;
        
        // 현재 효과음 뮤트 여부 토글에 넘겨주기
        sfxMute.isOn = IsSfxMute;
    }
    
    /// <summary>
    /// 각종 UI와 상태가 값이 변할 때 호출할 함수를 연결
    /// </summary>
    void LinkToListener()
    {
        bgmSlider.onValueChanged.AddListener(OnBgmVolumeChanged);
        bgmMute.onValueChanged.AddListener(OnBgmMute);
        
        sfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
        sfxMute.onValueChanged.AddListener(OnSfxMute);
    }

    #endregion
    
    #region Bgm Method

    /// <summary>
    /// 브금 실행 함수
    /// </summary>
    /// <param name="isMain">메인씬인가?</param>
    // 사용하고 싶은 곳에서 AudioManager.instance.BgmSoundPlay(bool)
    public void BgmSoundPlay(bool isMain)
    {
        bgmPlayer.clip = isMain ? bgmClips[0] : bgmClips[1];
        bgmPlayer.Play();
    }

    /// <summary>
    /// 슬라이더의 볼륨값이 바뀔 때마다 호출하는 함수
    /// </summary>
    /// <param name="volume">볼륨</param>
    void OnBgmVolumeChanged(float volume)
    {
        BgmVolume = volume;
    }

    /// <summary>
    /// 브금의 음소거 여부가 변경될 때마다 호출하는 함수
    /// </summary>
    /// <param name="isMute">뮤트 여부</param>
    void OnBgmMute(bool isMute)
    {
        IsBgmMute = isMute;
    }

    #endregion

    #region sfx Method

    /// <summary>
    /// 효과음을 재생해주는 함수
    /// </summary>
    /// <param name="sfx">재생할 효과음 열거형</param>
    // 사용하고 싶은 곳에서 AudioManager.instance.PlaySfx(Sfx.Click)
    public void PlaySfx(Sfx sfx)
    {
        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            // 가장 마지막에 플레이한 인덱스 찾기
            // channelIndex가 3인 경우 i가 2만 돼도 5로 넘어감
            // 5일 때 1로 검사하기 위해 sfxPlayer.Length로 나눈 나머지로 활요
            int loopIndex = (i + channelIndex) % sfxPlayers.Length;

            // 해당 채널의 효과음이 플레이 중이면
            if (sfxPlayers[loopIndex].isPlaying)
                continue;   // 반복문 도중 아래를 실행하지 않고 다음 루프로 건너뛰는 키워드

            channelIndex = loopIndex;

            // 마지막 효과음 후 첫번째의 플레이어에 클립 넣어주고
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx];
            // 실행
            sfxPlayers[loopIndex].Play();
            
            // 플레이할 플레이어 찾았으니 루프 탈출
            break;
        }
    }

    private void OnSfxVolumeChanged(float volume)
    {
        SfxVolume = volume;
    }

    void OnSfxMute(bool isMute)
    {
        IsSfxMute = isMute;
    }

    #endregion
}
