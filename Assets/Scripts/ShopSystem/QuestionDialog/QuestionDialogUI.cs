using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class QuestionDialogUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questionTxt;
    [SerializeField] private Button yesBtn;
    [SerializeField] private Button noBtn;
    [SerializeField] private Button closeBtn;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemPrice;
    [SerializeField] private Image itemCurrency;

    [SerializeField] private UnityAction yesAction;
    [SerializeField] private UnityAction noAction;

    void Awake()
    {
        LoadComponents();
        gameObject.SetActive(false);
    }

    public void LoadComponents()
    {
        questionTxt = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        yesBtn = transform.GetChild(2).GetComponent<Button>();
        noBtn = transform.GetChild(3).GetComponent<Button>();
        closeBtn = transform.GetChild(4).GetComponent<Button>();
        itemIcon = transform.GetChild(5).GetComponent<Image>();
        itemName = transform.GetChild(6).GetComponent<TextMeshProUGUI>();
        itemPrice = transform.GetChild(7).GetComponent<TextMeshProUGUI>();
        itemCurrency = transform.GetChild(8).GetComponent<Image>();
    }

    public void DisplayConfirmWithContentItem(string _question, Sprite _itemIcon, string _itemName, string _itemQuantity, string _itemPrice, string _tier, Sprite _currency , UnityAction _yesAction, UnityAction _noAction)
    {
        questionTxt.text = _question;
        itemIcon.sprite = _itemIcon;
        itemName.text = _itemName + " x" + _itemQuantity;
        itemPrice.text = _itemPrice;
        itemCurrency.sprite = _currency;
        switch (_tier)
        {
            case "common":
                itemName.color = new Color32(209, 213, 216, 255);
                break;
            case "uncommon":
                itemName.color = new Color32(65, 168, 95, 255);
                break;
            case "rare":
                itemName.color = new Color32(44, 130, 201, 255);
                break;
            case "epic":
                itemName.color = new Color32(147, 101, 184, 255);
                break;
            case "legendary":
                itemName.color = new Color32(250, 197, 28, 255);
                break;
            case "mythic":
                itemName.color = new Color32(226, 80, 65, 255);
                break;
            default:
                Debug.Log("Not found rarity tier of item: " + _tier);
                break;
        }
        itemIcon.enabled = true;
        itemName.enabled = true;
        itemPrice.enabled = true;
        if(_currency != null)
            itemCurrency.enabled = true;
        else 
            itemCurrency.enabled = false;
        yesAction = _yesAction;
        noAction = _noAction;

        gameObject.SetActive(true);
    }

    public void DisplayPurchaseFailed(string _question, UnityAction _yesAction, UnityAction _noAction)
    {
        questionTxt.text = _question;
        itemIcon.enabled = false;
        itemName.enabled = false;
        itemPrice.enabled = false;
        itemCurrency.enabled = false;

        yesAction = _yesAction;
        noAction = _noAction;

        gameObject.SetActive(true);
    }

    public void DisplayPurchaseSuccesful(string _question, UnityAction _yesAction, UnityAction _noAction)
    {
        questionTxt.text = _question;
        itemIcon.enabled = false;
        itemName.enabled = false;
        itemPrice.enabled = false;
        itemCurrency.enabled = false;

        yesAction = _yesAction;
        noAction = _noAction;

        gameObject.SetActive(true);
    }


    private void OnEnable()
    {
        yesBtn.onClick.AddListener(YesBtnClicked) ;
        noBtn.onClick.AddListener(NoBtnClicked);
        closeBtn.onClick.AddListener(HideQuestionDialogUI);
    }

    private void OnDisable()
    {
        if(yesBtn != null)
        {
            yesBtn.onClick.RemoveListener(YesBtnClicked);
        }
        if(noBtn != null)
        {
            noBtn.onClick.RemoveListener(NoBtnClicked);
        }
        closeBtn.onClick.RemoveListener(HideQuestionDialogUI);
    }

    private void YesBtnClicked()
    {
        HideQuestionDialogUI();
        //Like yesAction() but with a null check
        yesAction?.Invoke();
    }

    private void NoBtnClicked()
    {
        HideQuestionDialogUI();
        noAction?.Invoke();
    }

    public void HideQuestionDialogUI()
    {
        this.gameObject.SetActive(false);
    }

    public void DisplayConfirmQuitGame(string _question, UnityAction _yesAction, UnityAction _noAction)
    {
        questionTxt.text = _question;
        itemIcon.enabled = false;
        itemName.enabled = false;
        itemPrice.enabled = false;
        itemCurrency.enabled = false;

        yesAction = _yesAction;
        noAction = _noAction;

        gameObject.SetActive(true);
    }
}
