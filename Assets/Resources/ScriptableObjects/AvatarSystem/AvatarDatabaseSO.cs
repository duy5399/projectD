using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AvatarDatabaseSO", menuName = "ScriptableObjects/AvatarDatabaseSO")]
public class AvatarDatabaseSO : ScriptableObject
{
    [SerializeField] private List<Sprite> avatarDB;
    [SerializeField] private List<Sprite> avatarBorderDB;

    public List<Sprite> avatarDB_ => avatarDB;
    public List<Sprite> avatarBorderDB_ => avatarBorderDB;
}
