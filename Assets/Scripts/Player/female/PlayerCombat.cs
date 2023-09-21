using Newtonsoft.Json.Linq;
using Photon.Pun;
using Photon.Pun.Demo.Procedural;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCombat : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerStats playerStats;

    [Header("Components")]
    [SerializeField] private Animator anim;
    [SerializeField] private AudioSource audioSource;

    [Header("Attack")]
    [SerializeField] private int countAttacks;
    [SerializeField] private bool attacking;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.35f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Health")]
    [SerializeField] private Transform tfHealth;
    [SerializeField] private bool isDead;


    public Transform attackPoint_ => attackPoint;
    public bool isDead_ => isDead;

    [Header("Popup")]
    [SerializeField] private Transform tfPopup;
    [SerializeField] private GameObject goDamageAndHealingPopup;
    [SerializeField] private List<GameObject> listDamageAndHealingPopup;

    [Header("OnPhotonSerializeView")]
    [SerializeField] private bool otherIsDead;
    [SerializeField] private int otherCountAttacks;
    [SerializeField] private bool otherAttacking;


    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("hasTeam"))
            {
                if (PhotonNetwork.CurrentRoom.CustomProperties["team0"].ToString().Contains(photonView.Owner.NickName))
                    photonView.RPC(nameof(RPC_OnChangeHealthColor), RpcTarget.All, photonView.Owner.NickName, 0, 0, 255);
                else if (PhotonNetwork.CurrentRoom.CustomProperties["team1"].ToString().Contains(photonView.Owner.NickName))
                    photonView.RPC(nameof(RPC_OnChangeHealthColor), RpcTarget.All, photonView.Owner.NickName, 255, 0, 0);
            }
            else
                tfHealth.GetComponent<Image>().color = Color.blue;
        }
        isDead = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
        {
            //countAttacks = otherCountAttacks;
            //otherAttacking = attacking;
            //isDead = otherIsDead;
        }
        //if (Input.GetMouseButtonDown(0))
    }

    public void UpdateAttack()
    {
        if(photonView.IsMine)
        {
            if (!attacking && !isDead)
            {
                attacking = true;
                AudioManager.instance.PlayerAttackSFX(audioSource, countAttacks);
                UpdateAttackAnim();
                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
                foreach (Collider2D enemy in hitEnemies)
                {
                    if (enemy.gameObject.layer == LayerMask.NameToLayer("Mob"))
                        enemy.GetComponent<MobController>().TakeDamage(playerStats.intAttack_);
                    else if (enemy.gameObject.layer == LayerMask.NameToLayer("Totem"))
                        enemy.GetComponent<Totem>().TakeDamage(playerStats.intAttack_);
                    else if (enemy.gameObject.layer == LayerMask.NameToLayer("Boss") || enemy.gameObject.layer == LayerMask.NameToLayer("MiniBoss"))
                        enemy.GetComponent<BossController>().TakeDamage(playerStats.intAttack_);
                    else if (enemy.gameObject.layer == LayerMask.NameToLayer("Player") && enemy.gameObject.GetPhotonView().IsMine == false && (bool)PhotonNetwork.CurrentRoom.CustomProperties["mapPvP"] == true)
                        photonView.RPC(nameof(RPC_DealingDamageNormalAttack), RpcTarget.All, enemy.gameObject.GetPhotonView().Owner.NickName);
                }
            }
        }
    }

    [PunRPC]
    private void RPC_DealingDamageNormalAttack(string name)
    {
        GameObject enemy = PhotonNetwork.CurrentRoom.Players.FirstOrDefault(x => x.Value.NickName == name).Value.TagObject as GameObject;
        enemy.gameObject.GetComponent<PlayerCombat>().TakingDamage(playerStats.intAttack_, photonView);
    }

    protected void StartAttack()
    {
        attacking = false;
        if (countAttacks < 3)
            countAttacks++;
    }

    protected void EndAttack()
    {
        attacking = false;
        countAttacks = 0;
    }

    public void UpdateAttackAnim()
    {
        photonView.RPC(nameof(RPC_UpdateAttackAnim), RpcTarget.All, countAttacks);
    }

    [PunRPC]
    private void RPC_UpdateAttackAnim(int countAttacks)
    {
        anim.SetTrigger("attack_" + countAttacks);
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    [PunRPC]
    public void RPC_OnChangeHealthColor(string nickname, int r, int g, int b)
    {
        if(photonView.Owner.NickName == nickname)
            tfHealth.GetComponent<Image>().color = new Color(r, g, b);
    }

    //-----------------

    public void OnChangeAttackPoint(Vector3 vector3)
    {
        attackPoint.position = vector3;
    }

    public void Dead()
    {
        isDead = true;
        anim.SetTrigger("isDead");
        if ((bool)PhotonNetwork.CurrentRoom.CustomProperties["mapPvP"] == true)
        {
            PvPRoomManager.instance.LoadSpawnPanel(photonView.Owner.NickName);
        }
    }

    public void Resurrect()
    {
        photonView.RPC(nameof(RPC_Resurrect), RpcTarget.All);
    }

    [PunRPC]
    private void RPC_Resurrect()
    {
        isDead = false;
        anim.SetTrigger("isResurrect");
    }

    [PunRPC]
    public void ResetBlood()  //hàm đặt lại máu của nhân vật
    {
        isDead = false;
        //currentHealth = 100f;
        //blood.fillAmount = 1f;
    }

    #region Nhận sát thương
    public void TakingDamage(int damage, PhotonView sender)
    {
        if (playerStats.intCurrentHP_ > 0)
        {
            AudioManager.instance.PlayerTakeDamageSFX(audioSource);
            anim.SetTrigger("hurt");
            int damageTaken = playerStats.TakingDamage(damage);
            DisplayDamagePopup(damageTaken);
            if(sender != null)
                sender.GetComponent<PlayerCombat>().ScoreboardStats("damageDealt", damageTaken);
            if (playerStats.intCurrentHP_ <= 0)
            {
                Dead();
                if (sender != null)
                {
                    sender.GetComponent<PlayerCombat>().ScoreboardStats("kills", 1);
                }
            }
        }
    }

    public void TakingTrueDamage(int trueDamage, PhotonView sender)
    {
        if (playerStats.intCurrentHP_ > 0)
        {
            AudioManager.instance.PlayerTakeDamageSFX(audioSource);
            anim.SetTrigger("hurt");
            int damageTaken = playerStats.TakingTrueDamage(trueDamage);
            DisplayDamagePopup(damageTaken);
            sender.GetComponent<PlayerCombat>().ScoreboardStats("damageDealt", damageTaken);
            if (playerStats.intCurrentHP_ <= 0)
            {
                Dead();
                if (sender != null)
                {
                    sender.GetComponent<PlayerCombat>().ScoreboardStats("kills",1);
                    Debug.Log("TakingTrueDamage GetKill: " + sender);
                }
            }
        }
    }
    #endregion
    #region Hồi phục HP
    public void FlatHealingHP(int hp)
    {
        if (playerStats.intCurrentHP_ > 0)
        {

            DisplayHealingPopup(playerStats.FlatHealingHP(hp));
        }
    }

    public void PercentageHealingHP(float percentageHP, PhotonView sender)
    {
        if (playerStats.intCurrentHP_ > 0)
        {
            int healing = playerStats.PercentageHealingHP(percentageHP);
            DisplayHealingPopup(healing);
            sender.GetComponent<PlayerCombat>().ScoreboardStats("healing", healing);
            Debug.Log("PercentageHealingHP sender: " + sender);
        }
    }
    #endregion

    #region Damage và Healing Popup
    private void DisplayDamagePopup(int damage)
    {
        bool spawnPopup = false;
        foreach (var objPopup in listDamageAndHealingPopup)
        {
            if (objPopup != null && !objPopup.activeInHierarchy)
            {
                objPopup.GetComponent<DamageAndHealingPopupController>().SetDamagePopup(damage);
                spawnPopup = true;
                break;
            }
        }
        if (!spawnPopup)
        {
            GameObject obj = Instantiate(goDamageAndHealingPopup, tfPopup);
            obj.GetComponent<DamageAndHealingPopupController>().SetDamagePopup(damage);
            listDamageAndHealingPopup.Add(obj);
        }
        Debug.Log("spawnPopup: " + spawnPopup);
    }

    private void DisplayHealingPopup(int healing)
    {
        bool spawnPopup = false;
        foreach (var objPopup in listDamageAndHealingPopup)
        {
            if (objPopup != null && !objPopup.activeInHierarchy)
            {
                objPopup.GetComponent<DamageAndHealingPopupController>().SetHealingPopup(healing);
                spawnPopup = true;
                break;
            }
        }
        if (!spawnPopup)
        {
            GameObject obj = Instantiate(goDamageAndHealingPopup, tfPopup);
            obj.GetComponent<DamageAndHealingPopupController>().SetHealingPopup(healing);
            listDamageAndHealingPopup.Add(obj);
        }
        Debug.Log("spawnPopup: " + spawnPopup);
    }
    #endregion

    #region Cập nhật chỉ số sát thương và hồi máu trong trận chiến
    public void GetKill()
    {
        if (photonView.IsMine)
        {
            if ((bool)PhotonNetwork.CurrentRoom.CustomProperties["mapPvP"] == true)
            {
                ExitGames.Client.Photon.Hashtable setValue = PhotonNetwork.CurrentRoom.CustomProperties;
                if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("kills"))
                {
                    Dictionary<string, int> dict = (Dictionary<string, int>)PhotonNetwork.CurrentRoom.CustomProperties["kills"];
                    if (dict.ContainsKey(PlayfabDataManager.instance.displayname_))
                    {
                        dict[PlayfabDataManager.instance.displayname_] = dict[PlayfabDataManager.instance.displayname_] + 1;
                        setValue["kills"] = dict;
                        Debug.Log("dict.ContainsKey(PlayfabDataManager.instance.displayname_): " + dict[PlayfabDataManager.instance.displayname_]);
                    }
                    else
                    {
                        dict.Add(PlayfabDataManager.instance.displayname_, 1);
                        setValue["kills"] = dict;
                        Debug.Log("dict.ContainsKey(PlayfabDataManager.instance.displayname_): " + dict[PlayfabDataManager.instance.displayname_]);
                    }
                }
                else
                {

                    Dictionary<string, int> dict = new Dictionary<string, int>();
                    dict.Add(PlayfabDataManager.instance.displayname_, 1);
                    setValue.Add("kills", dict);
                    Debug.Log("!dict.ContainsKey(PlayfabDataManager.instance.displayname_): " + dict[PlayfabDataManager.instance.displayname_]);
                }
                PhotonNetwork.CurrentRoom.SetCustomProperties(setValue);
                Debug.Log("GetKill: " + PlayfabDataManager.instance.displayname_);
            }
        }
    }

    public void ScoreboardStats(string scoreboardStatsName, int detail)
    {
        if (photonView.IsMine)
        {
            if ((bool)PhotonNetwork.CurrentRoom.CustomProperties["mapPvP"] == true)
            {
                ExitGames.Client.Photon.Hashtable setValue = PhotonNetwork.CurrentRoom.CustomProperties;
                if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(scoreboardStatsName))
                {
                    Dictionary<string, int> dict = (Dictionary<string, int>)PhotonNetwork.CurrentRoom.CustomProperties[scoreboardStatsName];
                    if (dict.ContainsKey(PlayfabDataManager.instance.displayname_))
                    {
                        dict[PlayfabDataManager.instance.displayname_] = dict[PlayfabDataManager.instance.displayname_] + detail;
                        setValue[scoreboardStatsName] = dict;
                        Debug.Log("dict.ContainsKey(PlayfabDataManager.instance.displayname_): " + dict[PlayfabDataManager.instance.displayname_]);
                    }
                    else
                    {
                        dict.Add(PlayfabDataManager.instance.displayname_, detail);
                        setValue[scoreboardStatsName] = dict;
                        Debug.Log("dict.ContainsKey(PlayfabDataManager.instance.displayname_): " + dict[PlayfabDataManager.instance.displayname_]);
                    }
                }
                else
                {

                    Dictionary<string, int> dict = new Dictionary<string, int>();
                    dict.Add(PlayfabDataManager.instance.displayname_, detail);
                    setValue.Add(scoreboardStatsName, dict);
                    Debug.Log("!dict.ContainsKey(PlayfabDataManager.instance.displayname_): " + dict[PlayfabDataManager.instance.displayname_]);
                }
                PhotonNetwork.CurrentRoom.SetCustomProperties(setValue);
                Debug.Log("GetKill: " + PlayfabDataManager.instance.displayname_);
            }
        }
    }
    #endregion
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)       //ta gửi (send) dữ liệu - vị trí đi là đang writing
        {
            stream.SendNext(countAttacks);
            stream.SendNext(otherAttacking);
        }
        else if (stream.IsReading)  //ta nhận (Receive) dữ liệu - vị trí của người khác là đang reading
        {
            otherCountAttacks = (int)stream.ReceiveNext();
            otherAttacking = (bool)stream.ReceiveNext();
        }
    }
}

