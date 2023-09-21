using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Shop Database", menuName = "ScriptableObjects/Shop Database")]
public class ShopDatabaseSO : ScriptableObject
{
    [SerializeField] private List<ShopItem> shopItem;
    public List<ShopItem> shopItem_ => shopItem;
}

[System.Serializable]
public class ShopItem
{
    [SerializeField] private ItemSO item;
    [SerializeField] private ItemSO currency;
    [SerializeField] private int price;
    [SerializeField] private int quantity;

    public ItemSO item_ => item;
    public ItemSO currency_ => currency;
    public int price_ => price;
    public int quantity_ => quantity;
    public ShopItem(ItemSO item, ItemSO currency, int price, int quantity)
    {
        this.item = item;
        this.currency = currency;
        this.price = price;
        this.quantity = quantity;
    }
}