/*
* Writer : 김준우
*
* 이 소스코드는 여러 씬을 이동할 수 있는 함수들이 코딩되어 있습니다.
*
* Last Update : 2024 / 06 / 19
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
   private static SceneChanger inst;

    void Awake()
    {
        if(null == inst)
        {
            inst = this;

            //씬 전환이 되더라도 파괴되지 않도록 함
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            //이미 만들어진 인스턴스를 사용하기 위해 새로운 씬에 같은 SceneChanger가 존재할 경우 삭제해줌
            Destroy(this.gameObject);
        }
    }
    //다른 클래스에서도 맘껏 호출이 가능함
    public static SceneChanger Instance
    { 
        get 
        {
            if (inst == null)
            {
                return null;
            }

            return inst; 
        } 
    }
    //메인화면으로 씬 전환 함수
    public void MoveToLobbyScene() 
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
    //인게임화면으로 씬 전환 함수
    public void MoveToRoomScene()
    {
        SceneManager.LoadScene("GameRoom", LoadSceneMode.Single);
    }
    //인게임화면으로 씬 전환 함수
    public void MoveToInGameScene()
    {
        SceneManager.LoadScene("InGame", LoadSceneMode.Single);
    }
}
