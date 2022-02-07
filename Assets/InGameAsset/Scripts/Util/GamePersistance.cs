using System;
using System.Collections.Generic;
using InGameAsset.Scripts.army;
using UnityEngine;
namespace InGameAsset.Scripts.Util
{
    public  class GamePersistance:ManagerManagement
    {
        public GameData _gameData;
        void Start() => LoadGame();

        void OnDisable() => SaveGame();

        void SaveGame()
        {
            Debug.Log("Saving Game Data");
            var json = JsonUtility.ToJson(_gameData);
            PlayerPrefs.SetString("GameData", json);
            Debug.Log(json);
            Debug.Log("Saving GameFlags Complete");
        }
        void LoadGame()
        {
            var json= PlayerPrefs.GetString("GameData");
            _gameData = JsonUtility.FromJson<GameData>(json);
            if (_gameData is null)
                _gameData = new GameData();
        }
    }
    
    [Serializable]
    public class GameData
    {
        public List<HostAttachment> HostAttachmentsDatas;
        
        
        public GameData()
        {
            HostAttachmentsDatas = new List<HostAttachment>();
        }
    }
}


