using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public PlayerCameraWork cameraArm;

    private void Start()
    {
        if(playerPrefab == null)
        {
            Debug.LogError("playerPrefab이 설정되지 않았습니다.");
        }
        else
        {
            if (PlayerManager.LocalPlayerInstance == null)
            {
                Debug.Log("로컬플레이어를 생성합니다.");
                cameraArm.targetObject = PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
            }
            else
            {
                Debug.Log("이미 로컬플레이어가 생성되어있어 새로 생성하지 않습니다.");
            }
        }
    }
}
