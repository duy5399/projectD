using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class AvatarSlotController : MonoBehaviour
{
    [SerializeField] private Sprite sprite;
    [SerializeField] private Image imgAvatar;

    public void LoadInfoAvatar(Sprite spriteAvatar)
    {
        sprite = spriteAvatar;
        imgAvatar.sprite = spriteAvatar;
    }

    public void LoadInfoAvatarBorder(Sprite spriteAvatarBorder)
    {
        sprite = spriteAvatarBorder;
        imgAvatar.sprite = spriteAvatarBorder;
    }

    public void OnClickChangeAvatar()
    {
        AvatarSystemManager.instance.LoadCurrentAvatar(sprite);
        AvatarSystemManager.instance.UpdateAvatar(sprite);
    }

    public void OnClickChangeAvatarBorder()
    {
        AvatarSystemManager.instance.LoadCurrentAvatarBorder(sprite);
        AvatarSystemManager.instance.UpdateAvatarBorder(sprite);
    }
}
