using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DetailBattlePvPSlotController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtPlayerName;
    [SerializeField] private TextMeshProUGUI txtKills;
    [SerializeField] private TextMeshProUGUI txtDamage;
    [SerializeField] private TextMeshProUGUI txtHealing;

    public void LoadInfo(string playerName, string kills, string damage, string healing)
    {
        txtPlayerName.text = playerName;
        txtKills.text = kills;
        txtDamage.text = damage;
        txtHealing.text = healing;
    }
}
