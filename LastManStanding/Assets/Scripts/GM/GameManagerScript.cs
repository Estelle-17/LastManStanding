using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public GameObject cameraArmPrefab;

    public Text notionText;

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
                //ī�޶� �ϰ� �÷��̾� ����
                Debug.Log("�����÷��̾ �����մϴ�.");
                GameObject cameraArm = Instantiate(cameraArmPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
                DontDestroyOnLoad(cameraArm);
                cameraArm.GetComponent<PlayerCameraWork>().targetObject = PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
            }
            else
            {
                Debug.Log("�̹� �����÷��̾ �����Ǿ��־� ���� �������� �ʽ��ϴ�.");
            }
        }
    }
    //���ο� �÷��̾� ���� �� ����ȭ
    void LoadArena()
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("������ Ŭ���̾�Ʈ�� �ƴϸ� LoadLevel�� ������ �� �����ϴ�.");
            return;
        }
        Debug.LogFormat("WaitingRoom {0}�� �ε��մϴ�.", PhotonNetwork.CurrentRoom.PlayerCount);
        PhotonNetwork.LoadLevel("WaitingRoom 1");
        //PhotonNetwork.LoadLevel("WaitingRoom " + PhotonNetwork.CurrentRoom.PlayerCount);
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        PhotonView view = PhotonView.Get(this);
        Debug.LogFormat("{0}���� �����ϼ̽��ϴ�.", newPlayer.NickName);
        Debug.LogFormat("���� �濡 �ִ� �÷��̾�� �� {0}�� �Դϴ�.", PhotonNetwork.CurrentRoom.PlayerCount);
        //view.RPC("NoticeRPC", RpcTarget.All, newPlayer.NickName + "���� �����ϼ̽��ϴ�.");

        if(PhotonNetwork.IsMasterClient)
        {
            LoadArena();
        }
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PhotonView view = PhotonView.Get(this);
        Debug.LogFormat("{0}���� �����ϼ̽��ϴ�.", otherPlayer.NickName);
        Debug.LogFormat("���� �濡 �ִ� �÷��̾�� �� {0}�� �Դϴ�.", PhotonNetwork.CurrentRoom.PlayerCount);
        //view.RPC("NoticeRPC", RpcTarget.All, otherPlayer.NickName + "���� �����ϼ̽��ϴ�.");

        if (PhotonNetwork.IsMasterClient)
        {
            LoadArena();
        }
    }
    public override void OnLeftRoom()
    {
        SceneChanger.Instance.MoveToLobbyScene();
    }

    [PunRPC]
    void NoticeRPC(string msg)
    {
        Debug.Log(msg);

        notionText.text = msg;
    }
}
