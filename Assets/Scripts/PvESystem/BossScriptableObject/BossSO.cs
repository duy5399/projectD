using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New BOSS", menuName = "ScriptableObjects/BOSS")]
public class BossSO : ScriptableObject
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 1f;
    [Header("Flip")]
    [SerializeField] private bool facingLeft = true;
    [Header("Attack")]
    [SerializeField] private int attackDamage = 40;
    [SerializeField] private List<GameObject> prefabBullet;
    [Header("Health")]
    [SerializeField] private int maxHealth = 1000;
    [SerializeField] private int armor = 50;
    [Header("SummonMob")]
    [SerializeField] private List<GameObject> prefabMob;
    [SerializeField] private int numberOfMob;
    [Header("Mechanic")]
    [SerializeField] private float intervalNextAction = 2f;

    public float moveSpeed_ => moveSpeed;
    public bool facingLeft_ => facingLeft;
    public int attackDamage_ => attackDamage;
    public List<GameObject> prefabBullet_ => prefabBullet;
    public int maxHealth_ => maxHealth;
    public int armor_ => armor;
    public float intervalNextAction_ => intervalNextAction;  
    public List<GameObject> prefabMob_ => prefabMob;
    public int numberOfMob_ => numberOfMob;
}
