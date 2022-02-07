using System;
using System.Collections;
using System.Collections.Generic;
using InGameAsset.Scripts.Player;
using InGameAsset.Scripts.Util;
using UnityEngine;

namespace InGameAsset.Scripts
{
    public class GameManager : ManagerManagement
    {
        [SerializeField] GameObject[] SystemPrefabs;
        public List<GameObject> InstancedSystemPrefabs= new List<GameObject>();

        void Awake()
        {
            InstantiateSystemPrefabs();
        }

        public void InstantiateSystemPrefabs()
        {
            GameObject prefabInstance;
            for (int i = 0; i < SystemPrefabs.Length; i++)
            {
                prefabInstance= Instantiate(SystemPrefabs[i],transform.parent);
                InstancedSystemPrefabs.Add(prefabInstance);
            }
        }
    }
}