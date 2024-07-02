using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

//PUN을 편리하게 사용, 관리하기 위해 MonoBehaviourPunCallbacks으로 변경
public class TestNetwork : MonoBehaviourPunCallbacks
{
    private readonly string gameVersion = "1.0";
    [SerializeField]
    int maxPlayersPerRoom = 4;

    private string playerName;

    public TMP_InputField userIdText;

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
        // 연결 프로세스를 버전에 맞춰 시작.
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    public void HostConnect(string roomName, bool isVisible)
    {
        if (PhotonNetwork.IsConnected)
        {
            //룸을 새로 만듬
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.IsVisible = isVisible;
            roomOptions.MaxPlayers = maxPlayersPerRoom;
            roomOptions.PublishUserId = true;
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
                roomOptions.MaxPlayers = maxPlayersPerRoom;
                roomOptions.PublishUserId = true;
                PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
            }

        }
    }
    //클라이언트가 마스터에 연결되면 호출
    public override void OnConnectedToMaster()
    {
        Debug.Log("Client connected to Master");
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

    //닉네임 설정 완료 버튼
    public void FinishPlayerNameCheck()
    {
        if(string.IsNullOrEmpty(userIdText.text))
        {
            playerName = $"USER_{Random.Range(0, 100):00}";
            userIdText.text = playerName;
        }

        playerName = userIdText.text;
        PhotonNetwork.NickName = playerName;

        //닉네임 입력 완료 후 사라짐
        userIdText.gameObject.transform.parent.gameObject.SetActive(false);
    }
}
