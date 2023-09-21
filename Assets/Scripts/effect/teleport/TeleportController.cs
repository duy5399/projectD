using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportController : MonoBehaviour
{
    [SerializeField] private Transform destination;
    // Start is called before the first frame update
    void Start()
    {
        destination = transform.GetChild(0);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player")){
            //if(Vector2.Distance(collider.transform.position, transform.position) <= 0.2f)
            //{
            //    collider.transform.position = destination.position;
            //}
            collider.transform.position = destination.position;
            MapController.instace.ActiveNextBoss();
            gameObject.SetActive(false);
        }
    }
}
