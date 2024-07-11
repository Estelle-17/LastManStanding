using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGamePlayerListManager : MonoBehaviourPunCallbacks
{
    public GameManagerScript gameManagerScript;
    public TextMeshProUGUI playerList;
    public GameObject checkListGameUI;

    string players;

    void Update()
    {
        PlayerListUpdate();
    }

    //우측 상단의 플레이어리스트 업데이트
    void PlayerListUpdate()
    {
        players = "";
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            players += p.NickName + "\n";
        }
        playerList.text = players;

        for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
        {
            if (i < PhotonNetwork.CurrentRoom.PlayerCount)
                checkListGameUI.transform.GetChild(i).gameObject.SetActive(true);
            else
                checkListGameUI.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void SetGoalCheckList(int[] checklist, int actorNumber)
    {
        int cnt = 0;
        string str = "";
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.ActorNumber == actorNumber)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (checklist[i] == 1)
                        str += "v ";
                    else
                        str += "  ";
                    
                }
                checkListGameUI.transform.GetChild(cnt).GetComponent<Text>().text = str;
                return;
            }
            else
            {
                cnt++;
            }
        }
    }
}
