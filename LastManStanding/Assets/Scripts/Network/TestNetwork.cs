using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

//PUN�� ���ϰ� ���, �����ϱ� ���� MonoBehaviourPunCallbacks���� ����
public class TestNetwork : MonoBehaviourPunCallbacks
{
    private void Awake()
    {
        //������ Ŭ���̾�Ʈ�� PhotonNetwork.LoadLevel()�� ȣ���� �� �ֵ��� �ϰ�
        //���� �뿡 �ִ� ��� Ŭ���̾�Ʈ�� ������ ����ȭ�ϰ� ��
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
    }

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    /// ���� ���μ����� ����.
    /// �̹� ������ �Ǿ��ִٸ�, ������ ������
    /// ������ ���� �ʾҴٸ� �ٽ� ����
    public void HostConnect(string roomName, bool isVisible)
    {
        if (PhotonNetwork.IsConnected)
        {
            //���� ���� ����
            //���ӿ� �����ϸ� OnJointRandomFaild()�� ����Ǽ� ���� �˸�
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
            //������� �뿡 ����
            //���ӿ� �����ϸ� OnJointRandomFaild()�� ����Ǽ� ���� �˸�
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
    //Ŭ���̾�Ʈ�� �����Ϳ� ����Ǹ� ȣ��
    public override void OnConnectedToMaster()
    {
        Debug.Log("Client connected to Master");
    }
    //�濡 �����ϸ� ȣ��
    public override void OnJoinedRoom()
    {
        Debug.Log("Enter Room");
        SceneChanger.Instance.MoveToWaitingRoomScene();
    }
    //�� ���忡 �����ϸ� ȣ��
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed enter Room");
    }
    //Ŭ���̾�Ʈ�� � ������ε� ������ �������� ȣ��
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("Disconnected Server : {0}", cause);
    }
}
