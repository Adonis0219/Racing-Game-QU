using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class PopupUIController : MonoBehaviour
{
    [SerializeField]
    protected GameObject popupUI; // �˾� UI ������Ʈ

    [SerializeField] 
    private Button openBt;
    [SerializeField] 
    private Button closeBt;

    public virtual void Start()
    {
        // ��ư ����
        openBt.onClick.AddListener(()=>OnOpenBtClk());
        closeBt.onClick.AddListener(()=>OnCloseBtClk());
    }

    // ���� �Լ��� �ڽ� ��� ����
    // �Լ��� �ٸ� �ڽ��� ���� ��� ����
    public abstract void OnOpenBtClk();

    public abstract void OnCloseBtClk();

}
