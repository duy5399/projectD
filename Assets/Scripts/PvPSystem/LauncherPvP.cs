using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LauncherPvP : MonoBehaviour
{
    [SerializeField] protected GameObject goPlayer;
    protected virtual void OnEnable()
    {
        SpawnPlayer();
    }
    protected virtual void SpawnPlayer()
    {
        Vector3 position = new Vector3(UnityEngine.Random.Range(-8f, 8f), 7, 0);
        PhotonNetwork.Instantiate(Path.Combine("Prefabs/Player", goPlayer.name), position, goPlayer.transform.rotation);
    }
}
