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

    bool isGameEnd;

    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        playerList = new PlayerManager[players.Length];

        for (int i = 0; i < players.Length; i++)
        {
            playerList[i] = players[i].GetComponent<PlayerManager>();
        }

        isGameEnd = false;
    }

    void Update()
    {
        if(players.Length != PhotonNetwork.PlayerList.Length)
        {
            players = GameObject.FindGameObjectsWithTag("Player");
            playerList = new PlayerManager[players.Length];

            for (int i = 0; i < players.Length; i++)
            {
                playerList[i] = players[i].GetComponent<PlayerManager>();
            }
        }

        if (playerList != null && players.Length == PhotonNetwork.PlayerList.Length && !isGameEnd) 
        {
            CheckRemainPlayers();
            CheckGoalList();
        }
    }

    public void CheckRemainPlayers()
    {
        int remainCnt = 0;
        foreach (PlayerManager pm in playerList)
        {
            if(!pm.isDead)
            {
                remainCnt++;
            }
        }

        //���� �÷��̾ �Ѹ� ���Ҵٸ� ��� Ŭ���̾�Ʈ�� ���� ���â ���
        if (remainCnt == 1)
        {
            foreach (PlayerManager pm in playerList)
            {
                int actorNumber = pm.playerActorNumber;
                if (pm.isDead)
                    continue;
                foreach (Player p in PhotonNetwork.PlayerList)
                {
                    if(p.ActorNumber == actorNumber)
                    {
                        SetGameEndUI(p.NickName);
                        return;
                    }
                }
            }
        }
        else if (remainCnt == 0)
        {
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                SetGameEndUI("None");
                return;
            }
        }
    }

    //�÷��̾ ��� �� �����Ͽ����� Ȯ��
    public void CheckGoalList()
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
                    SetGameEndUI(p.NickName);
                }
            }

            goalCnt = 0;
        }
    }

    //������ ������ �� ������ UI����
    void SetGameEndUI(string name)
    {
        isGameEnd = true;

        //���콺 Ŀ���� ������Ű�� ������ �ʵ��� ��
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

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
        Debug.Log("IsClicked!");

        //ȣ��Ʈ�� ��� ����
        if (PhotonNetwork.IsMasterClient)
        {
            //Photon.Instantiate�� ������ ��� GameObject ����
            GameObject[] aiPlayers = GameObject.FindGameObjectsWithTag("AIPlayer");
            foreach (GameObject aiPlayer in aiPlayers)
            {
                PhotonNetwork.Destroy(aiPlayer);
            }

            foreach (PlayerManager pm in playerList)
            {
                if (pm.gameObject.GetComponent<PhotonView>().IsMine == true)
                {
                    PhotonNetwork.Destroy(pm.gameObject);
                }
            }

            PhotonNetwork.LoadLevel("WaitingRoom 1");
        }
    }
}
