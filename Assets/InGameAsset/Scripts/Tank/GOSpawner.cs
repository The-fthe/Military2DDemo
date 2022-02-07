using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace InGameAsset.Scripts.Tank
{
    public class GOSpawner : MonoBehaviour
    {
        public List<GameObject> GetSpawnObject => SpawnObjectList;
        [SerializeField] GameObject _spawnPrefab;
        [SerializeField] string _path;
        [SerializeField] List<GameObject> SpawnObjectList = new List<GameObject>();
        [SerializeField] EnemySpawnPoint[] _spawnPoints;
        CancellationTokenSource cts;
    
        void  Start()
        {
            cts = new CancellationTokenSource();
        }

        public void SpawnEnemyFromPrefab(bool selfActive)
        {
            if (_spawnPrefab != null)
            {
                foreach (var spawnPoint in _spawnPoints)
                {
                    var instantiateGO = Instantiate(_spawnPrefab,spawnPoint.Pos,Quaternion.identity);
                    instantiateGO.transform.parent = transform;
                    instantiateGO.SetActive(selfActive);
                    SpawnObjectList.Add(instantiateGO);
                }
            }else
                Debugger.LogError("SpawnPrefab is null ");
        }

        public void SetRandomActiveAtSpawnPoint()
        {
            if(SpawnObjectList.Count <=0) SpawnEnemyFromPrefab(false);
            foreach (var  spawnObject in SpawnObjectList)
            {
                var spawnIndex = Random.Range(0, _spawnPoints.Length);
                spawnObject.transform.position = _spawnPoints[spawnIndex].Pos;
                spawnObject.SetActive(true);
            }            
        }
    
        public async void SpawnEnemyFromResources(bool selfActive= false)
        {
            if (_path == string.Empty)
            {
                Debugger.LogError($"{this.name}String is empty");
                return;
            }
            var prefab = await LoadResources(_path, cts.Token);
            if (prefab != null)
            {
                var instantiatedGO = Instantiate(prefab.gameObject);
                instantiatedGO.SetActive(selfActive);
                SpawnObjectList.Add(instantiatedGO);
            }
            else
            {
                Debugger.LogError("ResourcesPrefab Cant be loaded");
            }
        }

        async UniTask<Transform> LoadResources(string path, CancellationToken ct)
        {
            return await Resources.LoadAsync(path).WithCancellation(ct) as Transform;
        }
    }
}