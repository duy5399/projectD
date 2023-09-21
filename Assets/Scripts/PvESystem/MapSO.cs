using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Map", menuName = "ScriptableObjects/Map")]
public class MapSO : ScriptableObject
{
    [SerializeField] private int mapIndex;
    [SerializeField] private string mapIdRewards;
    [SerializeField] private string mapName;
    [SerializeField] private string mapDescription;
    [SerializeField] private Sprite mapImage;
    [SerializeField] private MapScene sceneToLoad;
    [SerializeField] private MapDifficulty mapDifficulty;
    [SerializeField] private List<Boss> mapMiniBoss;
    [SerializeField] private List<Boss> mapBoss;
    [SerializeField] private List<Rewards> mapRewards;
    [SerializeField] private int mapTime;
    [SerializeField] private int mapEnergy;

    public int mapIndex_ => mapIndex;
    public string mapIdRewards_ => mapIdRewards;
    public string mapName_ => mapName;
    public string mapDescription_ => mapDescription;
    public Sprite mapImage_ => mapImage;
    public MapScene sceneToLoad_ => sceneToLoad;
    public MapDifficulty mapDifficulty_ => mapDifficulty;
    public List<Boss> mapMiniBoss_ => mapMiniBoss;
    public List<Boss> mapBoss_ => mapBoss;
    public List<Rewards> mapRewards_ => mapRewards;
    public int mapTime_ => mapTime;
    public int mapEnergy_ => mapEnergy;
}

[System.Serializable]
public class MapRewards{
    [SerializeField] private ItemSO item;
    [SerializeField] private int quantity;
    [SerializeField] private float dropRate;

    public ItemSO item_ => item;
    public int quantity_ => quantity;
    public float dropRate_ => dropRate;

    public MapRewards(ItemSO item, int quantity, float dropRate)
    {
        this.item = item;
        this.quantity = quantity;
        this.dropRate = dropRate;
    }
}

[System.Serializable]
public class Boss
{
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private BossSO bossInfo;

    public GameObject bossPrefab_ => bossPrefab;
    public BossSO bossInfo_ => bossInfo;

    public Boss(GameObject bossPrefab, BossSO bossInfo)
    {
        this.bossPrefab = bossPrefab;
        this.bossInfo = bossInfo;
    }
}

public enum MapDifficulty
{
    easy,
    normal,
    difficult,
    hero
}

public enum MapScene
{
    AntCave,
    BugouCastle,
    EvilTribe
}

//--------------
[System.Serializable]
public class Rewards
{
    [SerializeField] private string itemName;
    [SerializeField] private Sprite itemIcon;
    [SerializeField] private string itemTier;
    [SerializeField] private int quantity;
    [SerializeField] private float dropRate;

    public string itemName_ => itemName;
    public Sprite itemIcon_ => itemIcon;
    public string itemTier_ => itemTier;
    public int quantity_ => quantity;
    public float dropRate_ => dropRate;

    public Rewards(string itemName, Sprite itemIcon, int quantity, float dropRate)
    {
        this.itemName = itemName;
        this.itemIcon = itemIcon;
        this.quantity = quantity;
        this.dropRate = dropRate;
    }
}
