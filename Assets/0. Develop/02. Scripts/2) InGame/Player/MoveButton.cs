using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MoveButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    /// <summary>
    /// ��ư�� �������� �ִ� ������ ����
    /// </summary>
    bool isButtonDown = false;

    [SerializeField]
    bool isMoveRight;
    [SerializeField]
    [Tooltip("�ش� ��ư�� ��¡ ������ ǥ���ϴ� �̹���")]
    Image chargingImage;

    Player player;

    #region ��¡ ����
    float charge;
    float maxCharge=100;
    delegate void SetMaxChargeAnim(bool setAnim);
    [Tooltip("�ִ� ��¡ �ִϸ��̼��� ���� ���θ� �����ϴ� ��������Ʈ")]
    SetMaxChargeAnim maxChargeAnim;
    #endregion

    public Action ResetButton;
    private void Start()
    {
        player = GameManager.instance.player;

        maxChargeAnim = (bool setAnim) =>
        {
            if (chargingImage.GetComponent<Animator>() != null)
                chargingImage.GetComponent<Animator>().SetBool("MaxCharge", setAnim);
        };

        #region ��ư ��¡ �ʱ�ȭ ����
        ResetButton += () =>
        {
            charge = 0;
            isButtonDown = false;
            Time.timeScale = 1f;
            chargingImage.fillAmount = 0;
            player.CurrentCharge = 0;
            maxChargeAnim?.Invoke(false);
            //��¡ �Ϸ� �ִϸ��̼� �ߴ�
        };
        #endregion
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isButtonDown = true;
        player.inputButtonIsRight = isMoveRight;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isButtonDown)
            return;
        else
        {
            player.ArrowBtClk(isMoveRight, charge);
            player.CurrentCharge = Mathf.Min(charge, maxCharge);
            ResetButton();
        }
    }

    void Update()
    {
        if (isButtonDown)
        {
            Charging();
            if (!player.isHaveNearObj && charge >= 30)//��ֹ��� ������ ������ �̻� ���� �Ѵٸ�
            {
                player.PlayerCollided();
                ResetButton();
            }

        }
    }
    /// <summary>
    /// ������ �ø��� ���
    /// </summary>
    void Charging()
    {
        charge += player.chargePerDelta * Time.deltaTime / Time.timeScale;
        player.CurrentCharge = Mathf.Min(charge, maxCharge);

        if (charge >= maxCharge)
            maxChargeAnim?.Invoke(true);
        //�ִ� ��¡�� �ִϸ��̼� ���

        #region �ҷ�Ÿ�� ����
        if (player.isHaveNearObj)//��ֹ��� ���ٸ� �ҷ�Ÿ�� X
        {
            float tempTimeScale = 1f - (charge / maxCharge * 10);//��¡ ������ ���� �Ҹ�Ÿ�� ��ȭ
            tempTimeScale = Mathf.Clamp(tempTimeScale, player.minTimeScale, player.maxTimeScale);//�Ҹ�Ÿ�� �ּ�,�ִ밪 ����

            Time.timeScale = tempTimeScale;
            //Ÿ�� �������� �����ϴ� ���
        }
        #endregion

        chargingImage.fillAmount = charge / maxCharge;
    }

    public void SetPointActive(bool active)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(active);
        }
        gameObject.GetComponent<Image>().raycastTarget = active;
    }
}
