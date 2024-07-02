using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ChatManager : MonoBehaviourPunCallbacks
{
    public InputController inputControl;
    public TextMeshProUGUI chatLog;
    public TextMeshProUGUI playerList;
    public TMP_InputField inputField;
    string players;
    ScrollRect scrollRect = null;   //채팅이 많이 쌓일 경우 스크롤바의 위치를 아래로 고정하기 위한 변수

    void Start()
    {
        inputControl = GetComponent<InputController>();
        if (inputControl != null)
        {
            inputControl.playerInputControl.PlayerAction.Chat.started += SendButtonClicked;
            inputControl.playerInputControl.PlayerAction.Mouse.started += DeactivateInputFieldFocus;
        }

        PhotonNetwork.IsMessageQueueRunning = true;
        scrollRect = GameObject.FindObjectOfType<ScrollRect>();
    }

    /// <summary>
    /// 전송 버튼을 누르면 실행되는 함수
    /// 앞에 닉네임과 함께 전해야 하는 대화 내용을 전송
    /// photonView.RPC를 통해 모든 플레이어들에게 전송시켜주며, 자기 자신도 같이 전송시켜줌
    /// 메세지 전송 후 바로 메세지가 입력되도록 inputField.ActivateInputField()를 바로 실행시켜줌
    /// </summary>
    public void SendButtonClicked(InputAction.CallbackContext context)
    {
        //키 입력이 시작된 경우
        if (context.started)
        {
            //InputSystem의 Chat키를 입력하여 메세지 입력 시작
            if(!inputField.isFocused)
            {
                inputField.ActivateInputField();
                return;
            }

            if(inputField.text.Equals("")) //입력된 채팅이 없으면 플레이어의 움직임을 활성화 후 return
            {
                inputField.DeactivateInputField();
                return;
            }
            string msg = string.Format("[{0}] {1}", PhotonNetwork.LocalPlayer.NickName, inputField.text);
            photonView.RPC("NoticeRPC", RpcTarget.OthersBuffered, msg);
            NoticeRPC(msg);
            inputField.DeactivateInputField();
            inputField.text = "";
        }
    }
    public void DeactivateInputFieldFocus(InputAction.CallbackContext context)
    {
        inputField.DeactivateInputField();
    }

        public void GameStart()
    {
        //게임 시작 시 마스터 클라이언트가 LoadLevel 실행
        if(PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("OnGameRoom", RpcTarget.AllBuffered);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        string msg = string.Format("<color=#00ff00>[{0}]님이 입장하셨습니다.</color>", newPlayer);
        NoticeRPC(msg);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        string msg = string.Format("<color=#ff0000>[{0}]님이 퇴장하셨습니다.</color>", otherPlayer);
        NoticeRPC(msg);
    }

    [PunRPC]
    void NoticeRPC(string msg)
    {
        Debug.Log(msg);
        chatLog.text += "\n" + msg;
        StartCoroutine(ScrollUpdate());
    }
    [PunRPC]
    void OnGameRoom()   //게임 입장 시
    {
        PhotonNetwork.LoadLevel("InGame");
    }

    IEnumerator ScrollUpdate()
    {
        yield return null;
        scrollRect.verticalNormalizedPosition = 0.0f;
    }
}
