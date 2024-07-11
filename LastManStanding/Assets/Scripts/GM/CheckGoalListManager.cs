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

    void Start()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        playerList = new PlayerManager[players.Length];

        for (int i = 0; i < players.Length; i++)
        {
            playerList[i] = players[i].GetComponent<PlayerManager>();
        }
    }

    void Update()
    {
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

            for (int i = 0; i < 4; i++)
            {
                if (checklist[i] == 1)
                    goalCnt++;
            }
            //���� ��� �� ������ �÷��̾ ������ �� ��� Ŭ���̾�Ʈ�� ���� ���â ���
            if (goalCnt == 4)
            {
                foreach (Player p in PhotonNetwork.PlayerList)
                {
                    if (p.ActorNumber == actorNumber)
                    {
                        SetGameEndUI(p.NickName);
                    }
                }
            }

            goalCnt = 0;
        }
    }

    //������ ������ �� ������ UI����
    void SetGameEndUI(string name)
    {
        playerName.text = name;
        if(PhotonNetwork.IsMasterClient)
        {
            buttonName.text = "Back to Lobby";
        }
        else
        {
            buttonName.text = "Waiting to Host";
        }

        gameEndUI.SetActive(true);
    }

    //������ ���� �� ���Ƿ� �̵�
    public void BackToWaitingScene()
    {
        //ȣ��Ʈ�� ��� ����
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("WaitingRoom 1");
            foreach (PlayerManager pm in playerList)
            {
                pm.gameObject.transform.position = new Vector3(0f, 5f, 0f);
            }
        }
    }
}
