using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace InGameAsset.Scripts.Util
{
    public class ObjectPool : SerializedMonoBehaviour
    {
        [SerializeField] GameObject _objectToPoolGO;
        public Queue<GameObject> _objectPoolGOs = new Queue<GameObject>();
        Transform _parent;

        public Transform _spawnedObjectsParent;
        public bool _alwaysDestroy = false;
        GameObject _masterObjectPool;
        int _poolSize = 10;
        public void SetObjectPoolAndPoolSize(GameObject objectToPool, int poolSize = 10)
        {
            _objectToPoolGO ??= objectToPool;
            _poolSize = poolSize;
            CreateObjectParentIfNeeded();
            while(_objectPoolGOs.Count < _poolSize)
            {
               var spawnedObject = Instantiate(_objectToPoolGO);
                spawnedObject.name = _objectToPoolGO.name + "_" + _objectPoolGOs.Count;
                spawnedObject.transform.SetParent(_spawnedObjectsParent);
                var destroyIfDisabled =spawnedObject.AddComponent<DestroyIfDisabled>();
                destroyIfDisabled.ObjectPool = this;
                spawnedObject.SetActive(false);
                _objectPoolGOs.Enqueue(spawnedObject);
            }

            SetObjectPoolParentToMasterObjectPool();
        }
        
        public GameObject CreateObjectWithNewParent(Vector3 newPos, Quaternion newRot)
        {
            GameObject spawnedObject = null;
           // Debug.Log($"objectPoolGO count is { _objectPoolGOs.Count}");
            if (_objectPoolGOs.Count < _poolSize)
            {
                spawnedObject = Instantiate(_objectToPoolGO, newPos, newRot);
                spawnedObject.name = _objectToPoolGO.name + "_" + _objectToPoolGO.name + "_" + _objectPoolGOs.Count;
                spawnedObject.transform.SetParent(_spawnedObjectsParent);
                var destroyIfDisabled =spawnedObject.AddComponent<DestroyIfDisabled>();
                destroyIfDisabled.ObjectPool = this;
            }
            else
            {
                spawnedObject = _objectPoolGOs.Dequeue();
                spawnedObject.transform.position = newPos;
                spawnedObject.transform.rotation = newRot;
                spawnedObject.SetActive(true);
            }
            _objectPoolGOs.Enqueue(spawnedObject);
            return spawnedObject;
        }

        void SetObjectPoolParentToMasterObjectPool()
        {
             _masterObjectPool = GameObject.Find("ObjectPool");
            if (_masterObjectPool != null)
            {
             //   Debug.Log($"{_objectToPoolGO.name } its Find master  find is trigger");
                _spawnedObjectsParent.parent = _masterObjectPool.transform;
            }
            else
                Debug.LogError("Main Object Pool game object cant be found!!");
        }

        void CreateObjectParentIfNeeded()
        {
            if (_spawnedObjectsParent == null)
            {
                string name = "ObjectPool_" + _objectToPoolGO.name;
                var parentObject = GameObject.Find(name);
                if (parentObject != null)
                {
                    _spawnedObjectsParent = parentObject.transform;
                }
                else
                {
                    _spawnedObjectsParent = new GameObject(name).transform;
                }
            }
        }

        public int Count()
        {
            return _objectPoolGOs.Count;
        }

        public void ReturnToPool(GameObject objectToReturn)
        {
            if(objectToReturn.activeSelf)
                objectToReturn.SetActive(false);
            _objectPoolGOs.Enqueue(objectToReturn);
        }


        void OnDestroy()
        {
             foreach (var item in _objectPoolGOs)
             {
                 if (item is null) continue; 
                 if (item != null && item.activeSelf == false || _alwaysDestroy)
                     Destroy(item);
                 else if (item != null)
                 {
                     item.GetComponent<DestroyIfDisabled>().SelfDestructionEnable = true;
                 }
             }
        }

        // void OnEnable()
        // {
        //     if(!_objectPoolGOs.Contains(_objectToPoolGO))
        //         _objectPoolGOs.Clear();
        // }
    }
}