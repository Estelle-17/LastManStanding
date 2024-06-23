using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

//PUN을 편리하게 사용, 관리하기 위해 MonoBehaviourPunCallbacks으로 변경
public class TestNetwork : MonoBehaviourPunCallbacks
{
    private void Awake()
    {
        //마스터 클라이언트가 PhotonNetwork.LoadLevel()을 호출할 수 있도록 하고
        //같은 룸에 있는 모든 클라이언트가 레벨을 동기화하게 함
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
    }

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    /// 연결 프로세스를 시작.
    /// 이미 연결이 되어있다면, 무작위 룸으로
    /// 연결이 되지 않았다면 다시 연결
    public void HostConnect(string roomName, bool isVisible)
    {
        if (PhotonNetwork.IsConnected)
        {
            //룸을 새로 만듬
            //접속에 실패하면 OnJointRandomFaild()이 실행되서 실패 알림
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.IsVisible = isVisible;
            roomOptions.MaxPlayers = 4;
            PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
        }
    }
    public void JoinConnect(string roomName)
    {
        if (PhotonNetwork.IsConnected)
        {
            //만들어진 룸에 접속
            //접속에 실패하면 OnJointRandomFaild()이 실행되서 실패 알림
            if(roomName == "")
            {
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                RoomOptions roomOptions = new RoomOptions();
                roomOptions.IsVisible = false;
                roomOptions.MaxPlayers = 4;
                PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
            }

        }
    }
    //클라이언트가 마스터에 연결되면 호출
    public override void OnConnectedToMaster()
    {
        Debug.Log("Client connected to Master");
    }
    //방에 입장하면 호출
    public override void OnJoinedRoom()
    {
        Debug.Log("Enter Room");
        SceneChanger.Instance.MoveToWaitingRoomScene();
    }
    //방 입장에 실패하면 호출
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed enter Room");
    }
    //클라이언트가 어떤 방식으로든 연결이 끊어지면 호출
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("Disconnected Server : {0}", cause);
    }
}
