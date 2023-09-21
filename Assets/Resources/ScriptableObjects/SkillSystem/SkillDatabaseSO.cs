using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillDatabaseSO", menuName = "ScriptableObjects/Skill/SkillDatabaseSO")]
public class SkillDatabaseSO : ScriptableObject
{
    [SerializedDictionary("Skill ID", "Skill ScriptableObject")] public SerializedDictionary<string, List<SkillSO>> skillData;
}