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

        //만약 플레이어가 한명만 남았다면 모든 클라이언트에 게임 결과창 출력
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

    //플레이어가 모든 골에 접근하였는지 확인
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
            //만약 모든 골에 접근한 플레이어가 존재할 시 모든 클라이언트에 게임 결과창 출력
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

    //게임이 끝났을 때 보여질 UI설정
    void SetGameEndUI(string name)
    {
        isGameEnd = true;

        //마우스 커서를 고졍시키며 보이지 않도록 함
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        //플레이어 입력 해제
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
        //UI입력을 위한 EventSystem을 SetActive시켜줌
        uiEventSystem.SetActive(true);
    }

    //게임이 끝난 후 대기실로 이동
    public void BackToWaitingScene()
    {
        Debug.Log("IsClicked!");

        //호스트만 사용 가능
        if (PhotonNetwork.IsMasterClient)
        {
            //Photon.Instantiate로 생성된 모든 GameObject 제거
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
