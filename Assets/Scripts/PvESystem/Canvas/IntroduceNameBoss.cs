using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroduceNameBoss : MonoBehaviour
{
    [SerializeField] private Animator anim;
    Dictionary<string, bool> introduceBossDone;

    public Animator anim_ => anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
        introduceBossDone = new Dictionary<string, bool>();
    }
    //bật animation hiển thị tên BOSS
    public void OnIntroduceBoss(string _nameBoss)
    {
        if (!introduceBossDone.ContainsKey("introduce" + _nameBoss))
        {
            //Debug.Log("IntroduceNameBoss" + _nameBoss);
            gameObject.SetActive(true);
            anim.SetTrigger("introduce" + _nameBoss);
            introduceBossDone.Add("introduce" + _nameBoss, true);
        }         
    }

    //tắt animation hiển thị tên BOSS
    public void OffIntroduceBoss()
    {
        gameObject.SetActive(false);
    }
}
