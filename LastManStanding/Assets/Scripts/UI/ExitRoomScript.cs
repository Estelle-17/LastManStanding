using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ExitRoomScript : MonoBehaviourPunCallbacks
{
    [SerializeField]
    GameObject ExitUI;

    public void ExitRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void ReturnGameScene()
    {
        Debug.Log("���Ӿ����� �ٽ� �̵�");
        //���콺 Ŀ���� ������Ű�� ������ �ʵ��� ��
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        ExitUI.SetActive(false);
    }

    public override void OnLeftRoom()
    {
        SceneChanger.Instance.MoveToLobbyScene();
    }
}
