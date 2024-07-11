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
        //�÷��̾��� ���� ActorNumber ����
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
        //waiting1Scene���� ���� ������ UI�� ����
        if(playerListManager != null)
        {
            playerListManager.SetReadyImage(isRoomReady, playerActorNumber);
        }

        //InGameScene���� ���� ������ UI�� ����
        if (inGameplayerListManager != null)
        {
            inGameplayerListManager.SetGoalCheckList(goalCheckList, playerActorNumber);
        }
    }

    //InGameScene���� �� �� �÷��̾���� ������ UI�� ǥ�����ִ� GameObject�˻�
    public void SetInGamePlayerListManager()
    {
        inGameplayerListManager = GameObject.FindWithTag("PlayerListUI").GetComponent<InGamePlayerListManager>();
    }

    #region IPunObservable Implementation

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            //Ŭ���̾�Ʈ���� ���ϴ� �� ����ȭ�� ���⿡ �ڵ� �Է�
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
