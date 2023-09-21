using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertResultBattle : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private Button btnReplay;
    [SerializeField] private Button btnHomepage;
    [SerializeField] private Button btnConfirm;

    private void Awake()
    {
        btnReplay.gameObject.SetActive(false);
        btnHomepage.gameObject.SetActive(false);
        btnConfirm.gameObject.SetActive(false);
    }

    //hiển thị thông báo khi thua(nhân vật hết máu, hết thời gian,...)
    public void OnAlertLose()
    {
        gameObject.SetActive(true);
        btnReplay.gameObject.SetActive(true);
        btnHomepage.gameObject.SetActive(true);
        btnConfirm.gameObject.SetActive(false);
        anim.SetTrigger("loseBattle");
    }

    //hiển thị thông báo khi chiến thắng (đánh bại BOSS trước khi hết thời gian và HP > 0)
    public void OnAlertWin()
    {
        gameObject.SetActive(true);
        btnReplay.gameObject.SetActive(false);
        btnHomepage.gameObject.SetActive(false);
        btnConfirm.gameObject.SetActive(true);
        anim.SetTrigger("winBattle");
    }

    public void OffAlertResultBattle()
    {
        gameObject.SetActive(false);
    }
}
