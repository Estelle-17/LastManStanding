using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyButtonScript : MonoBehaviour
{
    [SerializeField]
    GameObject settingsUIObject;
    [SerializeField]
    GameObject networkObject;
    [SerializeField]
    TMP_InputField HostRoomName;
    [SerializeField]
    TMP_InputField JoinRoomName;
    [SerializeField]
    Toggle HostToggleValue;

    private void Start()
    {
        if(settingsUIObject != null)
        {
           settingsUIObject.SetActive(false);
        }
    }
    public void MoveToHostScene()
    {
        networkObject.GetComponent<TestNetwork>().HostConnect(HostRoomName.text, HostToggleValue.isOn);
    }
    public void MoveToWaitingRoomScene()
    {
        networkObject.GetComponent<TestNetwork>().JoinConnect(JoinRoomName.text);
    }
    public void OnSettingsUI()
    {
        settingsUIObject.SetActive(true);
    }
    public void OffSettingsUI()
    {
        settingsUIObject.SetActive(false);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
