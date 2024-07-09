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

    public int playerActorNumber;

    public bool isRoomReady;

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
    }

    void Update()
    {
        if(playerListManager != null)
        {
            playerListManager.SetReadyImage(isRoomReady, playerActorNumber);
        }

    }

    #region IPunObservable Implementation

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            //클라이언트끼리 원하는 값 동기화시 여기에 코드 입력
            stream.SendNext(isRoomReady);
        }
        else
        {
            this.isRoomReady = (bool)stream.ReceiveNext();
        }
    }

    #endregion

}
