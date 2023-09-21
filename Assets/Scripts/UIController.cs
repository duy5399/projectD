using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance { get; private set; }

    [Header("HealthBar")]
    public Slider healthbar;
    public Image img_icon;
    public Text txt_name;
    public Text txt_hp;

    [Header("Warning")]
    [SerializeField] private Transform alertWarning;

    [Header("Result Battle")]
    [SerializeField] private Transform alertResultBattle;

    [Header("Menu Options")]
    [SerializeField] private Transform menuOptions;

    [Header("Introduce Boss")]
    [SerializeField] private Transform introduceBoss;

    public Transform alertWarning_ => alertWarning;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
        if (transform.childCount > 0 && transform.GetChild(0) != null && transform.GetChild(1) != null)
        {
            healthbar = gameObject.transform.GetChild(0).GetComponent<Slider>();
            img_icon = gameObject.transform.GetChild(0).GetChild(3).GetComponent<Image>();
            txt_name = gameObject.transform.GetChild(0).GetChild(4).GetComponent<Text>();
            txt_hp = gameObject.transform.GetChild(0).GetChild(5).GetComponent<Text>();
            alertWarning = gameObject.transform.GetChild(1);
            alertWarning.gameObject.SetActive(false);
            alertResultBattle = gameObject.transform.GetChild(2);
            alertResultBattle.gameObject.SetActive(false);
            menuOptions = gameObject.transform.GetChild(5);
            menuOptions.gameObject.SetActive(false);
            introduceBoss = gameObject.transform.GetChild(6);
            introduceBoss.gameObject.SetActive(false);
        }
        else
        {
            //Debug.Log("No child");
        }
    }

    public void SetInforBoss(GameObject _boss)
    {
        txt_name.text = _boss.name;
        var iconBoss = Resources.Load<Sprite>("Sprites/IconBoss/" + txt_name.text + "/" + txt_name.text + "_icon");
        img_icon.sprite = iconBoss;
        SetHealth(_boss.GetComponent<BossController>().currentHealth_, _boss.GetComponent<BossController>().maxHealth_);
    }

    //hiển thị máu của BOSS
    public void SetHealth(int _health, int _maxHealth)
    {
        healthbar.maxValue = _maxHealth;
        healthbar.value = _health;
        txt_hp.text = Math.Round((double)_health / _maxHealth * 100) > 0 ? Math.Round((double)_health / _maxHealth * 100) + "%" : "0%";
        //txt_hp.text = _health + "/" + _maxHealth;

        //Debug.Log("SetHealth(_boss.GetComponent<BossController>().currentHealth_, _boss.GetComponent<BossController>().maxHealth_);");
    }

    #region Introduce BOSS
    //bật animation hiển thị tên BOSS
    public void OnIntroduceBoss(string _nameBoss)
    {
        introduceBoss.GetComponent<IntroduceNameBoss>().OnIntroduceBoss(_nameBoss);
        //StartCoroutine(introduceBoss.GetComponent<IntroduceNameBoss>().OnIntroduceBoss1(_nameBoss));
    }

    //tắt animation hiển thị tên BOSS
    public void OffIntroduceBoss()
    {
        introduceBoss.GetComponent<IntroduceNameBoss>().OffIntroduceBoss();
    }

    public Animator GetAnim()
    {
        return introduceBoss.GetComponent<IntroduceNameBoss>().anim_;
    }
    #endregion

    #region AlertWarning
    //hiển thị thông báo hành động quan trọng tiếp theo của BOSS cho người chơi
    public void OnAlertWarning(string _message)
    {
        alertWarning.GetComponent<AlertWarning>().OnAlertWarning(_message);
    }

    //tắt các hiển thị thông báo bằng cách reset trigger
    public void OffAlertWarning()
    {
        alertWarning.GetComponent<AlertWarning>().OffAlertWarning();
    }
    #endregion

    #region AlertResultBattle
    //hiển thị thông báo khi thua (nhân vật hết máu, hết thời gian,...)
    public void OnAlertLose()
    {
        alertResultBattle.GetComponent<AlertResultBattle>().OnAlertLose();
    }

    //hiển thị thông báo khi chiến thắng (đánh bại BOSS trước khi hết thời gian và HP > 0)
    public void OnAlertWin()
    {
        alertResultBattle.GetComponent<AlertResultBattle>().OnAlertWin();
    }
    #endregion

    #region Button

    //bật menu options
    public void onClickOptionsBtn()
    {
        menuOptions.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    //nút Home
    public void onClickHomeBtn()
    {
        AudioManager.instance.Destroy();
        MapInfo.instance.Destroy();
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("Homepage");
    }

    //nút Chơi lại
    public void onClickReplayBtn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    //nút Tiếp tục
    public void onClickResumeBtn()
    {
        menuOptions.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    //nút Thoát game
    public void onClickExitBtn()
    {
        //Debug.Log("Application.Quit()");

        Application.Quit();
    }
    #endregion
}
