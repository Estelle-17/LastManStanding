using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InGamePlayerListManager : MonoBehaviourPunCallbacks
{
    public GameManagerScript gameManagerScript;
    public TextMeshProUGUI playerList;

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
    }
}
