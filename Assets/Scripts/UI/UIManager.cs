using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }


    [Header("Item Description")]
    [SerializeField] private Transform tfItemDescription;
    [SerializeField] private InventoryDesciptionController inventoryDesciptionController;
    public InventoryDesciptionController inventoryDesciptionController_ => inventoryDesciptionController;

    [Header("Question Dialog")]
    [SerializeField] private Transform tfQuestionDialog;
    [SerializeField] private QuestionDialogUI questionDialogUI;
    public QuestionDialogUI questionDialogUI_ => questionDialogUI;

    [Header("ListButtonTopRight")]
    [SerializeField] private Button btnExpandTopRight;
    [SerializeField] private Transform tf_listButtonTopRight;
    [SerializeField] private float expandTime = 1f;
    [SerializeField] private bool isExpand = false;

    [Header("Player UI Controller")]
    [SerializeField] private GameObject goPlayerUIController;
    [SerializeField] private GameObject tfPlayerUIController;

    [Header("Audio Manager")]
    [SerializeField] private GameObject goAudioManager;
    [SerializeField] private GameObject tfAudioManager;

    [Header("Map Info")]
    [SerializeField] private GameObject goMapInfo;
    [SerializeField] private GameObject tfMapInfo;

    void Awake()
    {
        if (instance !=  null && instance != this)
            Destroy(this);
        else
            instance = this;
        if (Time.timeScale == 0f)
            Time.timeScale = 1f;  
    }

    void Start()
    {
        tfItemDescription.gameObject.SetActive(false);
        inventoryDesciptionController = tfItemDescription.GetComponent<InventoryDesciptionController>();
        questionDialogUI = tfQuestionDialog.GetComponent<QuestionDialogUI>();
        tfPlayerUIController = GameObject.FindWithTag("PlayerUIController");
        if(tfPlayerUIController == null)
        {
            Debug.Log("tfPlayerUIController == null");
            Instantiate(goPlayerUIController);
        }
        tfAudioManager = GameObject.FindWithTag("MapInfo");
        if (tfAudioManager == null)
        {
            Debug.Log("tfMapInfo == null");
            Instantiate(goAudioManager);
        }
        tfMapInfo = GameObject.FindWithTag("MapInfo");
        if (tfMapInfo == null)
        {
            Debug.Log("tfMapInfo == null");
            Instantiate(goMapInfo);
        }
    }

    //AudioManager.instance.ClickSuccessSFX();

    public void ExitGame()
    {
        AudioManager.instance.ClickSuccessSFX();
        UIManager.instance.questionDialogUI.DisplayConfirmQuitGame("Bạn có chắc muốn thoát game?", () => { Application.Quit(); }, () => { });
    }

    public void OnClick_ExpandTopRightBtn()
    {
        if (isExpand)
        {
            btnExpandTopRight.GetComponent<RectTransform>().DORotate(new Vector3(0,0,180), expandTime * 0.4f).SetEase(Ease.Linear);
            tf_listButtonTopRight.GetComponent<RectTransform>().DOAnchorPos(new Vector3(-220, -70, 0), expandTime).SetEase(Ease.OutQuint);
            isExpand = false;
        }
        else
        {
            btnExpandTopRight.GetComponent<RectTransform>().DORotate(new Vector3(0, 0, 0), expandTime * 0.4f).SetEase(Ease.Linear);
            tf_listButtonTopRight.GetComponent<RectTransform>().DOAnchorPos(new Vector3(600, -70, 0), expandTime).SetEase(Ease.OutQuad);
            isExpand = true;
        }
    }

}
