using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public GameObject cameraArmPrefab;

    public TextMeshProUGUI ReadyPlayersText;

    public int readyPlayersCount = 0;

    private void Start()
    {
        if(playerPrefab == null)
        {
            Debug.LogError("playerPrefab이 설정되지 않았습니다.");
        }
        else
        {
            if (PhotonNetwork.InRoom && PlayerManager.LocalPlayerInstance == null)
            {
                //카메라 암과 플레이어 생성
                Debug.Log("start에서 로컬플레이어를 생성합니다.");
                GameObject cameraArm = Instantiate(cameraArmPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
                DontDestroyOnLoad(cameraArm);

                GameObject player = PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
                cameraArm.GetComponent<PlayerCameraWork>().targetObject = player;

            }
            else
            {
                Debug.Log("이미 로컬플레이어가 생성되어있어 새로 생성하지 않습니다.");
            }
        }
    }

    private void Update()
    {
        CheckReadyState();
    }

    //마스터 클라이언트의 맵 동기화
    void LoadArena()
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("마스터 클라이언트가 아니면 LoadLevel을 진행할 수 없습니다.");
            return;
        }
        Debug.LogFormat("WaitingRoom 1을 로딩합니다.");
        PhotonNetwork.LoadLevel("WaitingRoom 1");
    }

    //현재 플레이어들의 준비 상태 확인
    void CheckReadyState()
    {
        if (ReadyPlayersText != null)
        {
            ReadyPlayersText.text = readyPlayersCount + "/" + PhotonNetwork.CurrentRoom.PlayerCount;
        }
    }

    //방에 입장하면 호출
    public override void OnJoinedRoom()
    {
        Debug.Log("Enter Room");
        Debug.LogFormat("플레이어의 닉네임은 {0} 입니다.", PhotonNetwork.NickName);
        Debug.LogFormat("현재 방에 있는 플레이어는 총 {0}명 입니다.", PhotonNetwork.CurrentRoom.PlayerCount);

        SceneChanger.Instance.MoveToWaitingRoomScene();

        //입장한 플레이어를 생성해줍니다.
        if (PhotonNetwork.CurrentRoom.PlayerCount > 1 && PlayerManager.LocalPlayerInstance == null)
        {
            //카메라 암과 플레이어 생성
            Debug.Log("joinedRoom에서 로컬플레이어를 생성합니다.");
            GameObject cameraArm = Instantiate(cameraArmPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
            DontDestroyOnLoad(cameraArm);

            GameObject player = PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
            cameraArm.GetComponent<PlayerCameraWork>().targetObject = player;
        }
        else
        {
            Debug.Log("이미 로컬플레이어가 생성되어있어 새로 생성하지 않습니다.");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.LogFormat("{0}님이 참가하셨습니다.", newPlayer.NickName);
        Debug.LogFormat("현재 방에 있는 플레이어는 총 {0}명 입니다.", PhotonNetwork.CurrentRoom.PlayerCount);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.LogFormat("{0}님이 퇴장하셨습니다.", otherPlayer.NickName);
        Debug.LogFormat("현재 방에 있는 플레이어는 총 {0}명 입니다.", PhotonNetwork.CurrentRoom.PlayerCount);
    }
    public override void OnLeftRoom()
    {
        SceneChanger.Instance.MoveToLobbyScene();
    }
}
