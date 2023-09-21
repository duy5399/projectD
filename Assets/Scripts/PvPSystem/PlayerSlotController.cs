using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerSlotController : MonoBehaviour
{
    [SerializeField] private string strPlayfabID;
    [SerializeField] private Transform tfCircleLight;
    [SerializeField] private Transform tfWing;
    [SerializeField] private Transform tfFace;
    [SerializeField] private Transform tfHead;
    [SerializeField] private Transform tfHair;
    [SerializeField] private Transform tfCloth;
    [SerializeField] private Transform tfGlass;
    [SerializeField] private Sprite sprFace;
    [SerializeField] private Sprite sprHead;
    [SerializeField] private Sprite sprHair;
    [SerializeField] private Sprite sprCloth;
    [SerializeField] private Sprite sprGlass;
    [SerializeField] private Sprite sprWing;
    [SerializeField] private Sprite sprWeapon;
    [SerializeField] private TextMeshProUGUI txtDisplayName;
    [SerializeField] private bool isReady;
    [SerializeField] private Image imgReadyStatus;

    public bool isReady_ => isReady;
    private void OnEnable()
    {
        isReady = false;
        ResetImgShow();
    }

    public void SetDisplayName(string displayName)
    {
        txtDisplayName.text = displayName;
    }

    public void SetReady(bool isReady)
    {
        this.isReady = isReady;
        imgReadyStatus.enabled = isReady;
    }

    public void GetUserData(string displayname, string playfabId)
    {
        strPlayfabID = playfabId;
        txtDisplayName.text = displayname;
        tfFace.GetComponent<Image>().enabled = true;
        tfHead.GetComponent<Image>().enabled = true;
        tfHair.GetComponent<Image>().enabled = true;
        tfCloth.GetComponent<Image>().enabled = true;
        tfGlass.GetComponent<Image>().enabled = true;
        GetUserDataRequest request = new GetUserDataRequest()
        {
            PlayFabId = playfabId,
            Keys = new List<string>() { "equippedItems" }
        };
        PlayFabClientAPI.GetUserReadOnlyData(request,
            result => {
                if (result.Data == null || !result.Data.ContainsKey("equippedItems"))
                    Debug.Log("No Ancestor");
                else
                {
                    var equippedItems = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, ItemInstance>>(result.Data["equippedItems"].Value);
                    foreach (var item in equippedItems)
                    {
                        LoadImgShow(item.Key, item.Value);
                    }
                }
            }, (error) => {
                Debug.Log(error.GenerateErrorReport());
            });
    }

    public void LoadImgShow(string slot, ItemInstance itemInstance)
    {
        var inforBasic01 = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(itemInstance.CustomData["inforBasic01"]);
        switch (slot)
        {
            case "weapon":
                var inforUpgrade = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(itemInstance.CustomData["inforUpgrade"]);
                int strenghLv = int.Parse(inforUpgrade["strenghLv"].ToString());
                tfCircleLight.GetComponent<Animator>().SetInteger("WeaponStrength", strenghLv);
                tfCircleLight.GetComponent<Animator>().enabled = true;
                break;
            case "wing":
                string[] parameter = itemInstance.ItemId.Split(new char[] { '_' });
                tfWing.GetComponent<Animator>().SetInteger("Wing", int.Parse(parameter[1]));
                tfWing.GetComponent<Animator>().enabled = true;
                break;
            case "face":
                tfFace.GetComponent<Image>().sprite = ImageLoader.instance.imgShowDictionary_[inforBasic01["showImg"]];
                break;
            case "head":
                tfHead.GetComponent<Image>().sprite = ImageLoader.instance.imgShowDictionary_[inforBasic01["showImg"]];
                break;
            case "hair":
                tfHair.GetComponent<Image>().sprite = ImageLoader.instance.imgShowDictionary_[inforBasic01["showImg"]];
                break;
            case "cloth":
                tfCloth.GetComponent<Image>().sprite = ImageLoader.instance.imgShowDictionary_[inforBasic01["showImg"]];
                break;
            case "glass":
                tfGlass.GetComponent<Image>().sprite = ImageLoader.instance.imgShowDictionary_[inforBasic01["showImg"]];
                break;
        }
    }

    public void ResetImgShow()
    {
        tfFace.GetComponent<Image>().sprite = sprFace;
        tfFace.GetComponent<Image>().enabled = false;
        tfHead.GetComponent<Image>().sprite = sprHead;
        tfHead.GetComponent<Image>().enabled = false;
        tfHair.GetComponent<Image>().sprite = sprHair;
        tfHair.GetComponent<Image>().enabled = false;
        tfCloth.GetComponent<Image>().sprite = sprCloth;
        tfCloth.GetComponent<Image>().enabled = false;
        tfGlass.GetComponent<Image>().sprite = sprGlass;
        tfGlass.GetComponent<Image>().enabled = false;
        tfCircleLight.GetComponent<Animator>().SetInteger("WeaponStrength", 0);
        tfCircleLight.GetComponent<Animator>().enabled = false;
        tfCircleLight.GetComponent<Image>().sprite = sprWeapon;
        tfWing.GetComponent<Animator>().SetInteger("Wing", 0);
        tfWing.GetComponent<Animator>().enabled = false;
        tfWing.GetComponent<Image>().sprite = sprWing;
        txtDisplayName.text = "";
        isReady = false;
        imgReadyStatus.enabled = false;
        strPlayfabID = "";
    }
}
