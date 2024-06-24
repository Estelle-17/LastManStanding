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
            Debug.LogError("playerPrefab�� �������� �ʾҽ��ϴ�.");
        }
        else
        {
            if (PlayerManager.LocalPlayerInstance == null)
            {
                Debug.Log("�����÷��̾ �����մϴ�.");
                cameraArm.targetObject = PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
            }
            else
            {
                Debug.Log("�̹� �����÷��̾ �����Ǿ��־� ���� �������� �ʽ��ϴ�.");
            }
        }
    }
}
