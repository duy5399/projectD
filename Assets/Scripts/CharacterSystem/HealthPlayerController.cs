using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthPlayerController : MonoBehaviour
{
    public static HealthPlayerController instance { get; private set; }

    private void Awake()
    {
        if(instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }

    private void OnEnable()
    {
        SetUsername(PlayfabDataManager.instance.displayname_);
    }

    private void Update()
    {

    }

    [SerializeField] private Image imgAvatar;
    [SerializeField] private Image imgAvatarBorder;
    [SerializeField] private TextMeshProUGUI txtUsername;
    [SerializeField] private TextMeshProUGUI txtLevel;
    [SerializeField] private Slider sliderHP;
    [SerializeField] private Slider sliderMP;
    [SerializeField] private TextMeshProUGUI txtCurrentHP;
    [SerializeField] private TextMeshProUGUI txtCurrentMP;

    public void SetAvatar(Sprite avatar)
    {
        imgAvatar.sprite = avatar;
    }

    public void SetAvatarBorder(Sprite avatarBorder)
    {
        imgAvatarBorder.sprite = avatarBorder;
    }

    public void SetUsername(string username)
    {
        txtUsername.text = username;
    }

    public void SetCurrentHP(int currentHP, int maxHP)
    {
        sliderHP.maxValue = maxHP;
        sliderHP.value = currentHP;
        txtCurrentHP.text = currentHP.ToString() + "/" + maxHP.ToString();
    }

    public void SetCurrentMP(int currentMP, int maxMP)
    {
        sliderMP.maxValue = maxMP;
        sliderMP.value = currentMP;
        txtCurrentMP.text = currentMP.ToString() + "/" + maxMP.ToString();
    }

    public void SetCurrentHP(int currentHP, int maxHP, string photonName)
    {
        if(photonName == PhotonNetwork.NickName)
        {
            sliderHP.maxValue = maxHP;
            sliderHP.value = currentHP;
            txtCurrentHP.text = currentHP < 0 ? "0/" + maxHP.ToString() : currentHP.ToString() + "/" + maxHP.ToString();
        }
    }

    public void SetCurrentMP(int currentMP, int maxMP, string photonName)
    {
        if (photonName == PhotonNetwork.NickName)
        {
            sliderMP.maxValue = maxMP;
            sliderMP.value = currentMP;
            txtCurrentMP.text = currentMP < 0 ? "0/" + maxMP.ToString() : currentMP.ToString() + "/" + maxMP.ToString();
        }
    }
}
