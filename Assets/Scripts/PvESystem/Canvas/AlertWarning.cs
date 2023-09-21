using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AlertWarning : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private TextMeshProUGUI warningTxt;

    void Awake()
    {
        anim = GetComponent<Animator>();
        warningTxt = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    //hiển thị thông báo hành động quan trọng tiếp theo của BOSS cho người chơi
    public void OnAlertWarning(string _message)
    {
        warningTxt.text = _message;
        gameObject.SetActive(true);
        anim.SetTrigger("warning");
    }

    //tắt các hiển thị thông báo bằng cách reset trigger
    public void OffAlertWarning()
    {
        gameObject.SetActive(false);
    }
}
