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
    ScrollRect scrollRect = null;   //ä���� ���� ���� ��� ��ũ�ѹ��� ��ġ�� �Ʒ��� �����ϱ� ���� ����

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
    /// ���� ��ư�� ������ ����Ǵ� �Լ�
    /// �տ� �г��Ӱ� �Բ� ���ؾ� �ϴ� ��ȭ ������ ����
    /// photonView.RPC�� ���� ��� �÷��̾�鿡�� ���۽����ָ�, �ڱ� �ڽŵ� ���� ���۽�����
    /// �޼��� ���� �� �ٷ� �޼����� �Էµǵ��� inputField.ActivateInputField()�� �ٷ� ���������
    /// </summary>
    public void SendButtonClicked(InputAction.CallbackContext context)
    {
        //Ű �Է��� ���۵� ���
        if (context.started)
        {
            //InputSystem�� ChatŰ�� �Է��Ͽ� �޼��� �Է� ����
            if(!inputField.isFocused)
            {
                inputField.ActivateInputField();
                return;
            }

            if(inputField.text.Equals("")) //�Էµ� ä���� ������ �÷��̾��� �������� Ȱ��ȭ �� return
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
        //���� ���� �� ������ Ŭ���̾�Ʈ�� LoadLevel ����
        if(PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("OnGameRoom", RpcTarget.AllBuffered);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        string msg = string.Format("<color=#00ff00>[{0}]���� �����ϼ̽��ϴ�.</color>", newPlayer);
        NoticeRPC(msg);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        string msg = string.Format("<color=#ff0000>[{0}]���� �����ϼ̽��ϴ�.</color>", otherPlayer);
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
    void OnGameRoom()   //���� ���� ��
    {
        PhotonNetwork.LoadLevel("InGame");
    }

    IEnumerator ScrollUpdate()
    {
        yield return null;
        scrollRect.verticalNormalizedPosition = 0.0f;
    }
}
