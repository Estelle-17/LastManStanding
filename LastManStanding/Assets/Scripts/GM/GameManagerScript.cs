using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    private static GameManagerScript inst;

    int hostOrJoin; //0 : host, 1 : join
    string roomName;
    bool isRoomVisible;

    void Awake()
    {
        if (null == inst)
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

    public static GameManagerScript Instance
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

    public void SaveValue(int i, string r, bool b)
    {
        hostOrJoin = i;
        roomName = r;
        isRoomVisible = b;
    }
}
