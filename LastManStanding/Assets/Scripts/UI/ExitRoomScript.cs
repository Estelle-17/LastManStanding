using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ExitRoomScript : MonoBehaviourPunCallbacks
{
    [SerializeField]
    GameObject ExitUI;

    public GameObject eventSystem;
    public GameObject uiEventSystem;

    public void ExitRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void ReturnGameScene()
    {
        //���콺 Ŀ���� ������Ű�� ������ �ʵ��� ��
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        ExitUI.SetActive(false);

        uiEventSystem.SetActive(false);
        eventSystem.SetActive(true);
    }

    public override void OnLeftRoom()
    {
        SceneChanger.Instance.MoveToLobbyScene();
    }
}
