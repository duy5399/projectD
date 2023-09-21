using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviourPun, IPunObservable
{
    void Awake()
    {
        GetPlayerStat();
    }

    [Header("Components")]
    [SerializeField] private PlayerCombat playerCombat;
    [SerializeField] private HealthBarController healthBar;

    [Header("Stats")]
    [SerializeField] private int intAttack;
    [SerializeField] private int intDefense;
    [SerializeField] private int intHitPoint;
    [SerializeField] private int intLuck;
    [SerializeField] private int intManaPoint;

    [Header("Health")]
    [SerializeField] private int intCurrentHP;
    [SerializeField] private float intHPRegenPer5s;
    [SerializeField] private float floatTimeRegenHP;

    [Header("ManaPoints")]
    [SerializeField] private int intCurrentMP;
    [SerializeField] private float intMPRegenPer5s;
    [SerializeField] private float floatTimeRegenMP;

    [Header("OnPhotonSerializeView")]
    [SerializeField] private int otherAttack;
    [SerializeField] private int otherDefense;
    [SerializeField] private int otherHitPoint;
    [SerializeField] private int otherLuck;
    [SerializeField] private int otherManaPoint;
    [SerializeField] private int otherCurrentHP;
    [SerializeField] private int otherCurrentMP;

    public int intAttack_ => intAttack;
    public int intDefense_ => intDefense;
    public int intHitPoint_ => intHitPoint;
    public int intLuck_ => intLuck;
    public int intCurrentHP_ => intCurrentHP;
    public int intManaPoint_ => intManaPoint;
    public int intCurrentMP_ => intCurrentMP;

    private void Update()
    {
        if (!photonView.IsMine)
        {
            intAttack = otherAttack;
            intDefense = otherDefense;
            intHitPoint = otherHitPoint;
            intLuck = otherLuck;
            intCurrentHP = otherCurrentHP;
            intManaPoint = otherManaPoint;
            intCurrentMP = otherCurrentMP;
        }
        else
        {
            try
            {
                //HPRengenPer5s();
                MPRengenPer5s();
            }
            catch
            {
                Debug.Log("Đang lấy thông tin player");
            }
        }
    }
    public void GetPlayerStat()
    {
        var request = new GetUserDataRequest { Keys = new List<string> { "playerStats" } };
        PlayFabClientAPI.GetUserReadOnlyData(request,
            result =>
            {
                if (result.Data.ContainsKey("playerStats"))
                {
                    var playerStats = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, int>>(result.Data["playerStats"].Value);
                    intAttack = playerStats["atk"];
                    intDefense = playerStats["def"];
                    intHitPoint = playerStats["hp"];
                    intLuck = playerStats["luk"];
                    intManaPoint = 1000;
                    intHPRegenPer5s = 0.01f;
                    floatTimeRegenHP = 5f;
                    intMPRegenPer5s = 0.02f;
                    floatTimeRegenMP = 5f;
                    LoadStats();
                    Debug.Log("Lấy chỉ số nhân vật thành công: " + result.Data["playerStats"].Value.ToString());
                }
                else
                {
                    Debug.Log("Lấy chỉ số nhân vật thất bại");
                }
            },
            error =>
            {
                Debug.Log(error.Error);
            });
    }

    public void LoadStats()
    {
        intCurrentHP = intHitPoint;
        intCurrentMP = intManaPoint;
        healthBar.SetHealth(intCurrentHP, intHitPoint);
        HealthPlayerController.instance.SetCurrentHP(intCurrentHP, intHitPoint, photonView.Owner.NickName);
        HealthPlayerController.instance.SetCurrentMP(intCurrentMP, intManaPoint, photonView.Owner.NickName);
    }

    #region Hồi phục HP
    private void HPRengenPer5s()
    {
        if (photonView.IsMine && intCurrentHP < intHitPoint && !playerCombat.isDead_)
        {
            if (floatTimeRegenHP > 0)
            {
                floatTimeRegenHP -= Time.deltaTime;
                Debug.Log("floatTimeRegenHP: " + floatTimeRegenHP);
                return;
            }
            PercentageHealingHP(intHPRegenPer5s);
            floatTimeRegenHP = 5f;
        }
    }

    public int FlatHealingHP(int hp)
    {
        if (intCurrentHP > 0)
        {
            intCurrentHP = intCurrentHP + hp > intHitPoint ? intHitPoint : intCurrentHP + hp;
            healthBar.SetHealth(intCurrentHP, intHitPoint);
            HealthPlayerController.instance.SetCurrentHP(intCurrentHP, intHitPoint, photonView.Owner.NickName);
            return hp;
        }
        return 0;
    }

    public int PercentageHealingHP(float percentageHP)
    {
        if (intCurrentHP > 0)
        {
            var healing = (int)(float)((double)intHitPoint * percentageHP);
            intCurrentHP = intCurrentHP + healing > intHitPoint ? intHitPoint : intCurrentHP + healing;
            healthBar.SetHealth(intCurrentHP, intHitPoint);
            HealthPlayerController.instance.SetCurrentHP(intCurrentHP, intHitPoint, photonView.Owner.NickName);
            return healing;
        }
        return 0;
    }
    #endregion

    #region Hồi phục MP
    private void MPRengenPer5s()
    {
        if (photonView.IsMine && intCurrentMP < intManaPoint && !playerCombat.isDead_)
        {
            if (floatTimeRegenMP > 0)
            {
                floatTimeRegenMP -= Time.deltaTime;
                //Debug.Log("floatTimeRegenMP: " + floatTimeRegenMP);
                return;
            }
            PercentageRevoveryMP(intMPRegenPer5s);
            floatTimeRegenMP = 5f;
        }
    }
    public void FlatMPRevovery(int mp)
    {
        intCurrentMP = intCurrentMP + mp > intManaPoint ? intManaPoint : intCurrentMP + mp;
        HealthPlayerController.instance.SetCurrentMP(intCurrentMP, intManaPoint, photonView.Owner.NickName);
    }

    public void PercentageRevoveryMP(float percentageMP)
    {
        var mpRecovery = (int)(float)((double)intManaPoint * percentageMP);
        Debug.Log("MPRevovery: " + (intCurrentMP + mpRecovery) + "/" + intManaPoint);
        intCurrentMP = intCurrentMP + mpRecovery > intManaPoint ? intManaPoint : intCurrentMP + mpRecovery;
        HealthPlayerController.instance.SetCurrentMP(intCurrentMP, intManaPoint, photonView.Owner.NickName);
    }
    #endregion

    #region Nhận sát thương
    public int TakingDamage(int damage)
    {
        if (intCurrentHP > 0)
        {
            int damage_ = damage * 100 / (100 + intDefense);
            Debug.Log("TakingDamage: " + intCurrentHP);
            Debug.Log("intCurrentHP: " + damage);
            Debug.Log("intCurrentHP - damage: " + (intCurrentHP - damage));
            intCurrentHP -= damage;
            healthBar.SetHealth(intCurrentHP, intHitPoint);
            HealthPlayerController.instance.SetCurrentHP(intCurrentHP, intHitPoint, photonView.Owner.NickName);
            return damage;
        }
        return 0;
    }

    public int TakingTrueDamage(int trueDamage)
    {
        if (intCurrentHP > 0)
        {
            intCurrentHP -= trueDamage;
            healthBar.SetHealth(intCurrentHP, intHitPoint);
            HealthPlayerController.instance.SetCurrentHP(intCurrentHP, intHitPoint, photonView.Owner.NickName);
            return trueDamage;
        }
        return 0;
    }
    #endregion

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)       //ta gửi (send) dữ liệu - vị trí đi là đang writing
        {
            stream.SendNext(intAttack);
            stream.SendNext(intDefense);
            stream.SendNext(intHitPoint);
            stream.SendNext(intLuck);
            stream.SendNext(intCurrentHP);
            stream.SendNext(intManaPoint);
            stream.SendNext(intCurrentMP);
        }
        else if (stream.IsReading)  //ta nhận (Receive) dữ liệu - vị trí của người khác là đang reading
        {
            otherAttack = (int)stream.ReceiveNext();
            otherDefense = (int)stream.ReceiveNext();
            otherHitPoint = (int)stream.ReceiveNext();
            otherLuck = (int)stream.ReceiveNext();
            otherCurrentHP = (int)stream.ReceiveNext();
            otherManaPoint = (int)stream.ReceiveNext();
            otherCurrentMP = (int)stream.ReceiveNext();
        }
    }
}
