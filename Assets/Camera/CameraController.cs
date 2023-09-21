using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class CameraController : MonoBehaviour
{
    public static CameraController instance {  get; private set; }
    [SerializeField] private GameObject targetPlayer;
    [SerializeField] private float smoothSpeed;
    [SerializeField] private Vector3 offset;
    //[SerializeField] private Vector3 velocity = Vector3.zero;

    [SerializeField] private List<GameObject> targetMiniBoss;
    [SerializeField] private List<GameObject> targetBoss;

    [SerializeField] private bool introduceBoss;

    [SerializeField] private float timeNextAction;

    [SerializeField] private int numberOfBoss;

    [SerializeField] private Transform panel;

    public bool introduceBoss_ => introduceBoss;

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
        targetMiniBoss.AddRange(GameObject.FindGameObjectsWithTag("MiniBoss"));
        targetBoss.AddRange(GameObject.FindGameObjectsWithTag("Boss"));
        smoothSpeed = 10f;
        offset = new Vector3(0, 0, -5);
        timeNextAction = 0f;
        numberOfBoss = 0;
        if(targetBoss.Count > 0 || targetMiniBoss.Count > 0)
        {
            introduceBoss = true;
        }
        else
        {
            introduceBoss = false;
        }
    }

    void Start()
    {
        //StartCoroutine(updateOff());
    }

    // LateUpdate is called after function Update
    void LateUpdate()
    {
        if(targetPlayer == null)
        {
            try
            {
                targetPlayer = PhotonNetwork.LocalPlayer.TagObject as GameObject;
            }
            catch
            {
                Debug.Log("Không tìm thấy targetPlayer");
            }
        }
        if (introduceBoss)
        {
            panel.gameObject.SetActive(true);
            //PlayerMovement.instance.SetMoveSpeed(0f);
            //PlayerMovement.instance.SetCanDash(false);
            timeNextAction += Time.deltaTime;
            if (timeNextAction < 2f)
            {
                CameraFollowTarget(targetPlayer);
            }
            else
            {
                StartCoroutine(IntroduceBoss());
            }
        }
        else
        {
            try
            {
                CameraFollowTarget(targetPlayer);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
    }

    //hướng camera vào mục tiêu cần quan sát
    public void CameraFollowTarget(GameObject _target)
    {
        Vector3 desiredPosition = _target.transform.position + offset; //trị trí camera mong muốn
        //Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, _smoothSpeed * Time.deltaTime);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        if (smoothedPosition.x < -7.5f || smoothedPosition.x > 7.5f)
        {
            this.transform.position = smoothedPosition.x < -7.5f ? new Vector3(-7.5f, 0, smoothedPosition.z) : new Vector3(7.5f, 0, smoothedPosition.z);
        }
        else
        {
            this.transform.position = new Vector3(smoothedPosition.x, 0, smoothedPosition.z);
        }
        //Debug.Log("_target: " + _target.name);
    }

    //giới thiệu các boss
    IEnumerator IntroduceBoss()
    {
        if (introduceBoss)
        {
            if (numberOfBoss < targetMiniBoss.Count)
            {
                for (int i = 0; i < targetMiniBoss.Count; i++)
                {
                    CameraFollowTarget(targetMiniBoss[i]);
                    yield return new WaitForSeconds(0.5f);
                    UIController.instance.OnIntroduceBoss(targetMiniBoss[i].name);
                    yield return new WaitForSeconds(UIController.instance.GetAnim().GetCurrentAnimatorStateInfo(0).length + 1f);
                    numberOfBoss++;
                }
            }
            else
            {
                for (int i = 0; i < targetBoss.Count; i++)
                {
                    CameraFollowTarget(targetBoss[i]);
                    yield return new WaitForSeconds(0.5f);
                    UIController.instance.OnIntroduceBoss(targetBoss[i].name);
                    yield return new WaitForSeconds(UIController.instance.GetAnim().GetCurrentAnimatorStateInfo(0).length + 1f);
                    if (targetBoss[i] == targetBoss[targetBoss.Count - 1])
                    {
                        //PlayerMovement.instance.SetMoveSpeed(6f);
                        //PlayerMovement.instance.SetCanDash(true);
                        panel.gameObject.SetActive(false);
                        introduceBoss = false;
                    }
                }
            }
        }
    }
}
