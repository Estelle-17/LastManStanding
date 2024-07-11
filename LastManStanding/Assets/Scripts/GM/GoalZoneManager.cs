using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalZoneManager : MonoBehaviour
{
    [SerializeField]
    int goalNumber = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerManager>().goalCheckList[goalNumber] = 1;
        }
    }
}
