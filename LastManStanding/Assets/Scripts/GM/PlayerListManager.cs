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
        //�濡 ������������ ����
        if (!PhotonNetwork.InRoom)
        {
            return;
        }

        PlayerListUpdate();
    }

    //���� ����� �÷��̾��Ʈ ������Ʈ
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

    //ActorNumber�� ���Ͽ� �÷��̾ Ready�������� Ȯ�� �� Image����
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
