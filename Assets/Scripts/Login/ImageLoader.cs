using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine;

public class ImageLoader : MonoBehaviour
{
    public static ImageLoader instance { get; private set; }

    private byte[] downloadedContent;

    [SerializeField] private UpgradeConditionManager upgradeCondition;
    public UpgradeConditionManager upgradeCondition_ => upgradeCondition;


    [SerializeField] private ItemImgDatabaseSO itemImgDatabaseSO;

    private Dictionary<string, Sprite> imgIconDictionary = new Dictionary<string, Sprite>();
    private Dictionary<string, Sprite> imgShowDictionary = new Dictionary<string, Sprite>();

    public Dictionary<string, Sprite> imgIconDictionary_ => imgIconDictionary;
    public Dictionary<string, Sprite> imgShowDictionary_ => imgShowDictionary;

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        LoadImage();
    }

    private void LoadImage()
    {
        for(int i = 0; i < itemImgDatabaseSO.imgIconDB_.Count; i++)
        {
            imgIconDictionary.Add(itemImgDatabaseSO.imgIconDB_[i].name, itemImgDatabaseSO.imgIconDB_[i]);
        }
        foreach (var i in itemImgDatabaseSO.imgShowDB_)
        {
            imgShowDictionary.Add(i.name.ToString(), i);
        }
    }
}
