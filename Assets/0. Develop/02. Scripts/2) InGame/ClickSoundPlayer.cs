using System;
using UnityEngine;
using UnityEngine.UI;

public class ClickSoundPlayer : MonoBehaviour
{
    // AudioManager에 등록된 버튼 클릭 사운드의 이름을 인스펙터에서 지정할 수 있다.
    [SerializeField]
    string clickSoundName = "ButtonClick";  // 기본값 설정

    private Button bt;

    private void Awake()
    {
        bt = GetComponent<Button>();
    }

    private void OnEnable()
    {
        bt.onClick.AddListener(Play);
    }

    void Play()
    {
        AudioManager.instance.PlaySfx(Sfx.Click);
    }
}
