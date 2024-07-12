using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CheckGoalListManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    PlayerManager[] playerList;

    [SerializeField]
    GameObject gameEndUI;
    [SerializeField]
    TextMeshProUGUI playerName;
    [SerializeField]
    TextMeshProUGUI buttonName;

    public GameObject eventSystem;
    public GameObject uiEventSystem;

    GameObject[] players;

    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        playerList = new PlayerManager[players.Length];

        for (int i = 0; i < players.Length; i++)
        {
            playerList[i] = players[i].GetComponent<PlayerManager>();
        }
    }

    void Update()
    {
        if(players.Length == 0)
        {
            players = GameObject.FindGameObjectsWithTag("Player");
            playerList = new PlayerManager[players.Length];

            for (int i = 0; i < players.Length; i++)
            {
                playerList[i] = players[i].GetComponent<PlayerManager>();
            }
        }

        if (playerList != null) 
        {
            SetGoalCheckList();
        }
    }

    //�÷��̾ ��� �� �����Ͽ����� Ȯ��
    public void SetGoalCheckList()
    {
        foreach (PlayerManager pm in playerList)
        {
            int[] checklist = pm.goalCheckList;
            int actorNumber = pm.playerActorNumber;

            int goalCnt = 0;

            for (int i = 0; i < checklist.Length; i++)
            {
                if (checklist[i] == 1)
                    goalCnt++;
            }
            //���� ��� �� ������ �÷��̾ ������ �� ��� Ŭ���̾�Ʈ�� ���� ���â ���
            if (goalCnt == 4)
            {
                foreach (Player p in PhotonNetwork.PlayerList)
                {
                    //Debug.Log(p.NickName + "is Goal!");
                    SetGameEndUI(p.NickName);
                }
            }

            goalCnt = 0;
        }
    }

    //������ ������ �� ������ UI����
    void SetGameEndUI(string name)
    {
        //�÷��̾� �Է� ����
        eventSystem.SetActive(false);
        playerName.text = name;
        if(PhotonNetwork.IsMasterClient)
        {
            buttonName.text = "Back to Lobby";
        }
        else
        {
            buttonName.text = "Waiting to Host";
        }

        foreach (PlayerManager pm in playerList)
        {
            pm.gameObject.GetComponent<PlayerAnimatorManager>().isMoveEnable = false;
        }

        gameEndUI.SetActive(true);
        //UI�Է��� ���� EventSystem�� SetActive������
        uiEventSystem.SetActive(true);
    }

    //������ ���� �� ���Ƿ� �̵�
    public void BackToWaitingScene()
    {
        //ȣ��Ʈ�� ��� ����
        if (PhotonNetwork.IsMasterClient)
        {
            //��� AI�÷��̾� ����
            GameObject[] aiPlayers = GameObject.FindGameObjectsWithTag("AIPlayer");
            foreach (GameObject aiPlayer in aiPlayers)
            {
                Destroy(aiPlayer);
            }

            PhotonNetwork.LoadLevel("WaitingRoom 1");
        }
    }
}
