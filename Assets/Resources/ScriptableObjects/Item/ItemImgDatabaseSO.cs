using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Image Database", menuName = "ScriptableObjects/Item Img Database")]
public class ItemImgDatabaseSO : ScriptableObject
{
    [SerializeField] private List<Sprite> imgIconDB;
    [SerializeField] private List<Sprite> imgShowDB;
    public List<Sprite> imgIconDB_ => imgIconDB;
    public List<Sprite> imgShowDB_ => imgShowDB;
}
