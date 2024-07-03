using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
            if (PhotonNetwork.InRoom && PlayerManager.LocalPlayerInstance == null)
            {
                //ī�޶� �ϰ� �÷��̾� ����
                Debug.Log("start���� �����÷��̾ �����մϴ�.");
                GameObject cameraArm = Instantiate(cameraArmPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
                DontDestroyOnLoad(cameraArm);

                GameObject player = PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
                cameraArm.GetComponent<PlayerCameraWork>().targetObject = player;
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
        Debug.LogFormat("WaitingRoom 1�� �ε��մϴ�.");
        PhotonNetwork.LoadLevel("WaitingRoom 1");
    }

    //�濡 �����ϸ� ȣ��
    public override void OnJoinedRoom()
    {
        Debug.Log("Enter Room");
        Debug.LogFormat("�÷��̾��� �г����� {0} �Դϴ�.", PhotonNetwork.NickName);
        Debug.LogFormat("���� �濡 �ִ� �÷��̾�� �� {0}�� �Դϴ�.", PhotonNetwork.CurrentRoom.PlayerCount);

        SceneChanger.Instance.MoveToWaitingRoomScene();

        //������ �÷��̾ �������ݴϴ�.
        if (PhotonNetwork.CurrentRoom.PlayerCount > 1 && PlayerManager.LocalPlayerInstance == null)
        {
            //ī�޶� �ϰ� �÷��̾� ����
            Debug.Log("start���� �����÷��̾ �����մϴ�.");
            GameObject cameraArm = Instantiate(cameraArmPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
            DontDestroyOnLoad(cameraArm);

            GameObject player = PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
            cameraArm.GetComponent<PlayerCameraWork>().targetObject = player;
        }
        else
        {
            Debug.Log("�̹� �����÷��̾ �����Ǿ��־� ���� �������� �ʽ��ϴ�.");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.LogFormat("{0}���� �����ϼ̽��ϴ�.", newPlayer.NickName);
        Debug.LogFormat("���� �濡 �ִ� �÷��̾�� �� {0}�� �Դϴ�.", PhotonNetwork.CurrentRoom.PlayerCount);

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient);
            //LoadArena();
        }
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.LogFormat("{0}���� �����ϼ̽��ϴ�.", otherPlayer.NickName);
        Debug.LogFormat("���� �濡 �ִ� �÷��̾�� �� {0}�� �Դϴ�.", PhotonNetwork.CurrentRoom.PlayerCount);

        if (PhotonNetwork.IsMasterClient)
        {
            //LoadArena();
        }
    }
    public override void OnLeftRoom()
    {
        SceneChanger.Instance.MoveToLobbyScene();
    }
}
