using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

// // �迭 ������Ƽ �ε���
// class SfxVolume
// {
//     private float[] sfxVolumes;
//     
//     // �迭 ������Ƽ
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
    [SerializeField]        // ��� ���� �����̴�
    private Slider bgmSlider;
    [SerializeField]
    private Toggle bgmMute;
    
    private AudioSource bgmPlayer;
    public AudioClip[] bgmClips;
    
    [Header("# SFX")]
    [SerializeField]        // ���� ����Ʈ ���� �����̴�
    private Slider sfxSlider;
    [SerializeField]
    private Toggle sfxMute;
    
    private AudioSource[] sfxPlayers;
    [Tooltip("GameStart, Click")]
    public AudioClip[] sfxClips;
    // ȿ���� ä�� ����
    public int channels;
    // ȿ���� ä�� �ε���
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

    // ���Ŀ� ȿ���� Ŭ���� ������ ���� �ٸ� ���
    // �߰����� �۾�
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
    /// ��� �ʱ� ���� �Լ�
    /// </summary>
    void BgmInitSet()
    {
        // ��� �÷��̾� �ʱ�ȭ
        // ������Ʈ ����
        GameObject bgmObj = new GameObject("BgmPlayer");
        // �θ� ����� �Ŵ�����
        bgmObj.transform.SetParent(transform);
        // ��� ������Ʈ�� ������ҽ� ������Ʈ �߰� �� ��� �÷��̾ �Ҵ�
        bgmPlayer = bgmObj.AddComponent<AudioSource>();
        // ��� ���ѹݺ� ����
        bgmPlayer.loop = true;
        
        // ����� �ҽ� ������ �����̴������� �Ѱ���rl
        bgmSlider.value = BgmVolume;
        bgmPlayer.volume = BgmVolume;
        
        // ����� �ҽ� ��Ʈ ���� ��۰����� �Ѱ��ֱ�
        bgmMute.isOn = IsBgmMute;
        bgmPlayer.mute = IsBgmMute;
    }

    /// <summary>
    /// ȿ���� �ʱ� ���� �Լ�
    /// </summary>
    void SfxInitSet()
    {
        // ȿ���� �÷��̾� �ʱ�ȭ
        GameObject sfxObj = new GameObject("SfxPlayer");
        sfxObj.transform.SetParent(transform);
        // ä�� ������ŭ ����� �ҽ� �߰�
        sfxPlayers = new AudioSource[channels];
        
        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            // �� �ε����� �÷��̾ ����� �ҽ� �߰� �� �Ҵ�
            sfxPlayers[i] = sfxObj.AddComponent<AudioSource>();
            // ���� ó�� ���� �� �÷��� �� �ϵ���
            sfxPlayers[i].playOnAwake = false;
            sfxPlayers[i].volume = SfxVolume;
            sfxPlayers[i].mute = sfxMute;
        }
        
        // ���� ȿ���� ������ �����̴��� �Ѱ��ֱ�
        sfxSlider.value = SfxVolume;
        
        // ���� ȿ���� ��Ʈ ���� ��ۿ� �Ѱ��ֱ�
        sfxMute.isOn = IsSfxMute;
    }
    
    /// <summary>
    /// ���� UI�� ���°� ���� ���� �� ȣ���� �Լ��� ����
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
    /// ��� ���� �Լ�
    /// </summary>
    /// <param name="isMain">���ξ��ΰ�?</param>
    // ����ϰ� ���� ������ AudioManager.instance.BgmSoundPlay(bool)
    public void BgmSoundPlay(bool isMain)
    {
        bgmPlayer.clip = isMain ? bgmClips[0] : bgmClips[1];
        bgmPlayer.Play();
    }

    /// <summary>
    /// �����̴��� �������� �ٲ� ������ ȣ���ϴ� �Լ�
    /// </summary>
    /// <param name="volume">����</param>
    void OnBgmVolumeChanged(float volume)
    {
        BgmVolume = volume;
    }

    /// <summary>
    /// ����� ���Ұ� ���ΰ� ����� ������ ȣ���ϴ� �Լ�
    /// </summary>
    /// <param name="isMute">��Ʈ ����</param>
    void OnBgmMute(bool isMute)
    {
        IsBgmMute = isMute;
    }

    #endregion

    #region sfx Method

    /// <summary>
    /// ȿ������ ������ִ� �Լ�
    /// </summary>
    /// <param name="sfx">����� ȿ���� ������</param>
    // ����ϰ� ���� ������ AudioManager.instance.PlaySfx(Sfx.Click)
    public void PlaySfx(Sfx sfx)
    {
        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            // ���� �������� �÷����� �ε��� ã��
            // channelIndex�� 3�� ��� i�� 2�� �ŵ� 5�� �Ѿ
            // 5�� �� 1�� �˻��ϱ� ���� sfxPlayer.Length�� ���� �������� Ȱ��
            int loopIndex = (i + channelIndex) % sfxPlayers.Length;

            // �ش� ä���� ȿ������ �÷��� ���̸�
            if (sfxPlayers[loopIndex].isPlaying)
                continue;   // �ݺ��� ���� �Ʒ��� �������� �ʰ� ���� ������ �ǳʶٴ� Ű����

            channelIndex = loopIndex;

            // ������ ȿ���� �� ù��°�� �÷��̾ Ŭ�� �־��ְ�
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx];
            // ����
            sfxPlayers[loopIndex].Play();
            
            // �÷����� �÷��̾� ã������ ���� Ż��
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
