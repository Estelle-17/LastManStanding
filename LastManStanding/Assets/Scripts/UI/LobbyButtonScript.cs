using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyButtonScript : MonoBehaviour
{
    [SerializeField]
    GameObject SettingsUIObject;

    private void Start()
    {
        SettingsUIObject.SetActive(false);
    }
    public void MoveToInGameScene()
    {
        SceneChanger.Instance.MoveToGameScene();
    }
    public void OnSettingsUI()
    {
        SettingsUIObject.SetActive(true);
    }
    public void OffSettingsUI()
    {
        SettingsUIObject.SetActive(false);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
