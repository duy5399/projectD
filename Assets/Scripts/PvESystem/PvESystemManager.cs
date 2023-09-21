using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PvESystemManager : MonoBehaviour
{
    public void OnClick_OpenPvESystem()
    {
        //PhotonNetwork.LeaveRoom();
        Debug.Log("OnClick_OpenPvESystem");
    }

    public void OnClick_ClosePvESystem()
    {
        PvPSystemManager.instance.SetBoolIsSpawnPlayer(false);
    }
}
