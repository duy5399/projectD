using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PvP Game Mode", menuName = "ScriptableObjects/PvP/GameMode")]

public class GameModPvPSO : ScriptableObject
{
    [SerializeField] private string strName;
    [SerializeField] private int intMaxPlayers;
    [SerializeField] private bool boolHasTeam;
    [SerializeField] private int intTeamQuantity;
    [SerializeField] private int intTeamMember;

    public string strName_ => strName;
    public int intMaxPlayers_ => intMaxPlayers;
    public bool boolHasTeam_ => boolHasTeam;
    public int intTeamQuantity_ => intTeamQuantity;
    public int intTeamMember_ => intTeamMember;
}
