using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyTrigger : MonoBehaviour
{
    
    void Start()
    {
        
    }

    //플레이어가 입장/퇴장 시 bool값 변경
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerManager>().isRoomReady = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerManager>().isRoomReady = false;
        }
    }
}
