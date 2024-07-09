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

    //���� ����� �÷��̾��Ʈ ������Ʈ
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
