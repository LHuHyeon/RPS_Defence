using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static Define;

// TODO : 유저 정보는 서버에서 관리
// 게임 진행 시 저장할 데이터
[Serializable]
public class GameData
{
	public string Name;

	public int Money;
	public int GameGold;

	public float PlayTime;

	public WaveData CurrentWave;

	/*
	필수 데이터
	1. 용병 컬렉션
	*/
}

public class GameManagerEx
{
	private GameData _gameData = new GameData();
	public 	GameData SaveData { get { return _gameData; } set { _gameData = value; } }

	public UI_GameScene GameScene  	{ get; set; }
	public WaveSystem	WaveSystem	{ get; set; }

	public bool isDrag = false;
	public int 	remainEnemys = 0;

	private HashSet<GameObject> _mercenarys = new HashSet<GameObject>();
	private HashSet<GameObject> _enemys = new HashSet<GameObject>();

	#region 스탯
	public string Name
	{
		get { return _gameData.Name; }
		set { _gameData.Name = value; }
	}

	#endregion

	#region 재화
	public int Money
	{
		get { return _gameData.Money; }
		set { _gameData.Money = value; }
	}
	
	public int GameGold
	{
		get { return _gameData.GameGold; }
		set 
		{
			_gameData.GameGold = value;
		}
	}

	#endregion

	#region 시간

	public float PlayTime
	{
		get { return _gameData.PlayTime; }
		set { _gameData.PlayTime = value; }
	}

	public float waveRemainingTime;		// Wave 남은 시간

	#endregion

	public WaveData CurrentWave
	{
		get { return _gameData.CurrentWave; }
		set { _gameData.CurrentWave = value; }
	}

	public void Init()
	{
	}

	// 웨이브 보상
	public void WaveReward()
	{
		GameGold += CurrentWave.waveGold;
        GameScene.RefreshGold(CurrentWave.waveGold);

		GameScene.OnRPSPopup();
	}

    // 캐릭터 소환
	public Action<int> OnEnemySpawnEvent;
	public Action<int> OnMercenarySpawnEvent;
	public GameObject Spawn(Define.WorldObject type, string path, Transform parent = null)
    {
		GameObject go = Managers.Resource.Instantiate(path, parent);

		return CharacterSpawn(type, go);
    }

    public GameObject Spawn(Define.WorldObject type, GameObject obj, Transform parent = null)
    {
        GameObject go = Managers.Resource.Instantiate(obj, parent);

        return CharacterSpawn(type, go);
    }

	private GameObject CharacterSpawn(Define.WorldObject type, GameObject go)
	{
        switch(type)
        {
            case Define.WorldObject.Enemy:
				{
					_enemys.Add(go);
					if (OnEnemySpawnEvent.IsNull() == false)
						OnEnemySpawnEvent.Invoke(1);
				}
                break;
            case Define.WorldObject.Mercenary:
				{
					_mercenarys.Add(go);
					if (OnMercenarySpawnEvent.IsNull() == false)
						OnMercenarySpawnEvent.Invoke(1);
				}
                break;
            default:
                Debug.Log("GameManager : Null Type");
                break;
        }

        return go;
	}

    // 객체 타입 확인
    public Define.WorldObject GetWorldObjectType(GameObject go)
    {
        BaseController bc = go.GetComponent<BaseController>();
        if (bc.IsNull() == true)
            return Define.WorldObject.Unknown;

        return bc.WorldObjectType;
    }

    // 캐릭터 삭제
    public void Despawn(GameObject go)
    {
        switch(GetWorldObjectType(go))
        {
            case Define.WorldObject.Enemy:
                {
                    if (_enemys.Contains(go))
                    { 
                        _enemys.Remove(go);
                        if (OnEnemySpawnEvent.IsNull() == false)
                            OnEnemySpawnEvent.Invoke(-1);
                    }
                }
                break;
            case Define.WorldObject.Mercenary:
                {
                    if (_mercenarys.Contains(go))
                    {
                        _mercenarys.Remove(go);
                        if (OnMercenarySpawnEvent.IsNull() == false)
                            OnMercenarySpawnEvent.Invoke(-1);
                    }
                }
                break;
        }

        Managers.Resource.Destroy(go);
    }

	#region Save & Load	
	public string _path = Application.persistentDataPath + "/SaveData.json";

	public void SaveGame()
	{
		string jsonStr = JsonUtility.ToJson(Managers.Game.SaveData);
		File.WriteAllText(_path, jsonStr);
		Debug.Log($"Save Game Completed : {_path}");
	}

	public bool LoadGame()
	{
		if (File.Exists(_path) == false)
			return false;

		string fileStr = File.ReadAllText(_path);
		GameData data = JsonUtility.FromJson<GameData>(fileStr);
		if (data != null)
		{
			Managers.Game.SaveData = data;
		}

		Debug.Log($"Save Game Loaded : {_path}");
		return true;
	}
	#endregion
}
