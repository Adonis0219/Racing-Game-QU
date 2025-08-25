using System;
using UnityEngine;
using UnityEngine.UI;

public class ClickSoundPlayer : MonoBehaviour
{
    // AudioManager�� ��ϵ� ��ư Ŭ�� ������ �̸��� �ν����Ϳ��� ������ �� �ִ�.
    [SerializeField]
    string clickSoundName = "ButtonClick";  // �⺻�� ����

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
