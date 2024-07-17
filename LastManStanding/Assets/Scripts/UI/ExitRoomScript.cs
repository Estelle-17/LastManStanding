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
        //마우스 커서를 고졍시키며 보이지 않도록 함
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
