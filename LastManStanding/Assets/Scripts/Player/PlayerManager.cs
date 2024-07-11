using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static GameObject LocalPlayerInstance;

    [SerializeField]
    PlayerListManager playerListManager;

    [SerializeField]
    InGamePlayerListManager inGameplayerListManager;

    public int playerActorNumber;

    public bool isRoomReady;
    public int[] goalCheckList;

    private void Awake()
    {
        if(photonView.IsMine)
        {
            PlayerManager.LocalPlayerInstance = gameObject;
        }
        DontDestroyOnLoad(gameObject);

        playerListManager = GameObject.FindWithTag("PlayerListUI").GetComponent<PlayerListManager>();
        //플레이어의 고유 ActorNumber 저장
        playerActorNumber = GetComponent<PhotonView>().ControllerActorNr;

        isRoomReady = false;
        goalCheckList = new int[4];
        for(int i = 0; i < 4; i++)
        {
            goalCheckList[i] = 0;
        }
    }

    void Update()
    {
        //waiting1Scene에서 현재 정보를 UI에 갱신
        if(playerListManager != null)
        {
            playerListManager.SetReadyImage(isRoomReady, playerActorNumber);
        }

        //InGameScene에서 현재 정보를 UI에 갱신
        if (inGameplayerListManager != null)
        {
            inGameplayerListManager.SetGoalCheckList(goalCheckList, playerActorNumber);
        }
    }

    //InGameScene입장 시 각 플레이어들의 정보를 UI로 표현해주는 GameObject검색
    public void SetInGamePlayerListManager()
    {
        inGameplayerListManager = GameObject.FindWithTag("PlayerListUI").GetComponent<InGamePlayerListManager>();
    }

    #region IPunObservable Implementation

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            //클라이언트끼리 원하는 값 동기화시 여기에 코드 입력
            stream.SendNext(isRoomReady);
            stream.SendNext(goalCheckList);
        }
        else
        {
            this.isRoomReady = (bool)stream.ReceiveNext();
            this.goalCheckList = (int[])stream.ReceiveNext();
        }
    }

    #endregion

}
