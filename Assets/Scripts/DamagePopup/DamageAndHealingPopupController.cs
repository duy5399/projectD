using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageAndHealingPopupController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer imgCritPopup;
    [SerializeField] private TextMeshPro txtDmgAndHealingPopup;

    public void SetDamagePopup(int damage)
    {
        gameObject.SetActive(true);
        imgCritPopup.enabled = true;
        txtDmgAndHealingPopup.text = damage.ToString();
        txtDmgAndHealingPopup.color = new Color32(247, 218, 80, 255);
        StartCoroutine(DisplayDamageAndHealingPopup());
    }

    public void SetHealingPopup(int healing)
    {
        gameObject.SetActive(true);
        imgCritPopup.enabled = false;
        txtDmgAndHealingPopup.text = healing.ToString();
        txtDmgAndHealingPopup.color = new Color32(102, 255, 102, 255);
        StartCoroutine(DisplayDamageAndHealingPopup());
    }

    IEnumerator DisplayDamageAndHealingPopup()
    {
        Vector3 vector3 = transform.parent.position;
        transform.DOMoveY(vector3.y + 1.5f, 0.75f).SetEase(Ease.OutExpo);
        while (transform.position.y < vector3.y + 1.4f)
            yield return null;
        yield return new WaitForSeconds(0.5f);
        transform.DOMoveY(vector3.y + 2f, 0.75f).SetEase(Ease.InExpo);
        txtDmgAndHealingPopup.DOFade(0, 0.75f);
        imgCritPopup.DOFade(0, 0.75f);
        while (transform.position.y < vector3.y + 2f)
            yield return null;
        txtDmgAndHealingPopup.DOFade(1, 0);
        imgCritPopup.DOFade(1, 0);
        transform.position = transform.parent.position;
        gameObject.SetActive(false);
    }
}
