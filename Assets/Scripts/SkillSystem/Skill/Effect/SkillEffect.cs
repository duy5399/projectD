using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEffect : MonoBehaviour
{
    [SerializeField] protected PhotonView photonView;
    [SerializeField] protected PhotonView photonViewOwner;
    [SerializeField] protected Animator animator;
    [SerializeField] protected float floatDestroyTime;

    [SerializeField] protected float floatDamage;
    [SerializeField] protected float floatMoveSpeed;
    [SerializeField] protected bool boolFlipX;

    protected virtual void OnEnable()
    {

    }

    protected virtual void Update()
    {
        
    }

    public void SetOwner(PhotonView photonViewOwner)
    {
        this.photonViewOwner = photonViewOwner;
    }

    public void SetDamage(float floatDamage)
    {
        this.floatDamage = floatDamage;
    }

    public void SetFloatMoveSpeed(float floatMoveSpeed)
    {
        this.floatMoveSpeed = floatMoveSpeed;
    }

    public void SetFloatDestroyTime(float floatDestroyTime)
    {
        this.floatDestroyTime = floatDestroyTime;
    }

    protected virtual void DestroyGameObject(string condition)
    {
        if (floatDestroyTime >= 0)
        {
            floatDestroyTime -= Time.deltaTime;
        }
        else
        {
            animator.SetBool(condition, false);
            Destroy(gameObject);
        }
    }

    protected virtual void DealingDamage(GameObject gameObject)
    {
        gameObject.GetComponent<PlayerCombat>().TakingDamage((int)floatDamage, photonViewOwner);
    }
}
