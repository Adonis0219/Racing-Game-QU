using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MoveButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    /// <summary>
    /// 버튼이 눌려지고 있는 중인지 여부
    /// </summary>
    bool isButtonDown = false;

    [SerializeField]
    bool isMoveRight;
    [SerializeField]
    [Tooltip("해당 버튼의 차징 정도를 표현하는 이미지")]
    Image chargingImage;

    Player player;

    #region 차징 관련
    float charge;
    float maxCharge=100;
    delegate void SetMaxChargeAnim(bool setAnim);
    [Tooltip("최대 차징 애니매이션을 실행 여부를 결정하는 델리게이트")]
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

        #region 버튼 차징 초기화 정의
        ResetButton += () =>
        {
            charge = 0;
            isButtonDown = false;
            Time.timeScale = 1f;
            chargingImage.fillAmount = 0;
            player.CurrentCharge = 0;
            maxChargeAnim?.Invoke(false);
            //차징 완료 애니매이션 중단
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
            if (!player.isHaveNearObj && charge >= 30)//장애물이 없을때 일정량 이상 차지 한다면
            {
                player.PlayerCollided();
                ResetButton();
            }

        }
    }
    /// <summary>
    /// 차지를 올리는 기능
    /// </summary>
    void Charging()
    {
        charge += player.chargePerDelta * Time.deltaTime / Time.timeScale;
        player.CurrentCharge = Mathf.Min(charge, maxCharge);

        if (charge >= maxCharge)
            maxChargeAnim?.Invoke(true);
        //최대 차징시 애니매이션 재생

        #region 불렛타임 조절
        if (player.isHaveNearObj)//장애물이 없다면 불렛타임 X
        {
            float tempTimeScale = 1f - (charge / maxCharge * 10);//차징 정도에 따라 불릿타임 강화
            tempTimeScale = Mathf.Clamp(tempTimeScale, player.minTimeScale, player.maxTimeScale);//불릿타임 최소,최대값 보정

            Time.timeScale = tempTimeScale;
            //타임 스케일을 조절하는 기능
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
