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
        //GameManagerScript.Instance.SaveValue(0, HostRoomName.text, HostToggleValue.isOn);
        networkObject.GetComponent<TestNetwork>().HostConnect(HostRoomName.text, HostToggleValue.isOn);
        //SceneChanger.Instance.MoveToWaitingRoomScene();
    }
    public void MoveToWaitingRoomScene()
    {
        //GameManagerScript.Instance.SaveValue(1, JoinRoomName.text, HostToggleValue.isOn);
        networkObject.GetComponent<TestNetwork>().JoinConnect(JoinRoomName.text);
        //SceneChanger.Instance.MoveToWaitingRoomScene();
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
