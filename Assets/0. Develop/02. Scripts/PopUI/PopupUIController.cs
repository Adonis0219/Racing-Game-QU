using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class PopupUIController : MonoBehaviour
{
    [SerializeField]
    protected GameObject popupUI; // 팝업 UI 오브젝트

    [SerializeField] 
    private Button openBt;
    [SerializeField] 
    private Button closeBt;

    public virtual void Start()
    {
        // 버튼 연결
        openBt.onClick.AddListener(()=>OnOpenBtClk());
        closeBt.onClick.AddListener(()=>OnCloseBtClk());
    }

    // 오픈 함수는 자식 모두 동일
    // 함수가 다른 자식이 생길 경우 수정
    public abstract void OnOpenBtClk();

    public abstract void OnCloseBtClk();

}
