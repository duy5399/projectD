using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Mob", menuName = "ScriptableObjects/Mob")]
public class MobSO : ScriptableObject
{
    public int maxHealth;
    public int armor;
    public float moveSpeed;
    public int attackDamage;
    public float attackRange;
}
