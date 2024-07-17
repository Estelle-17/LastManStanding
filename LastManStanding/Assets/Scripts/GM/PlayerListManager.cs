using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListManager : MonoBehaviourPunCallbacks
{
    public GameManagerScript gameManagerScript;
    public TextMeshProUGUI playerList;
    public GameObject greenGameUI;

    string players;

    void Update()
    {
        //방에 입장했을때만 갱신
        if (!PhotonNetwork.InRoom)
        {
            return;
        }

        PlayerListUpdate();
    }

    //우측 상단의 플레이어리스트 업데이트
    void PlayerListUpdate()
    {
        players = "";
        foreach(Player p in PhotonNetwork.PlayerList)
        {
            players += p.NickName + "\n";
        }
        playerList.text = players;

        for(int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
        {
            if(i < PhotonNetwork.CurrentRoom.PlayerCount)
                greenGameUI.transform.GetChild(i).gameObject.SetActive(true);
            else
                greenGameUI.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    //ActorNumber를 비교하여 플레이어가 Ready상태인지 확인 및 Image변경
    public void SetReadyImage(bool isReady, int actorNumber)
    {
        int cnt = 0;
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.ActorNumber == actorNumber)
            {
                if (isReady)
                {
                    greenGameUI.transform.GetChild(cnt).GetComponent<Image>().color = Color.green;
                }
                else
                    greenGameUI.transform.GetChild(cnt).GetComponent<Image>().color = Color.red;
                return;
            }
            else
            {
                cnt++;
            }
        }
    }
}
