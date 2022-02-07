using System;
using InGameAsset.Scripts.Util;
using Unity.Mathematics;
using UnityEngine;

class ObjectOpposivePosGenerator: MonoBehaviour
{
    [SerializeField] GameObject _objectPrefab;
    [SerializeField] ObjectPool _objectPool;// TODO inplement objectPool
    [SerializeField] int _poolSize=3;
    [SerializeField] GameObject[] poolObjects;

    void Awake()
    {
        _objectPool ??= gameObject.AddComponent(typeof(ObjectPool))as ObjectPool;
        _objectPool.SetObjectPoolAndPoolSize(_objectPrefab,_poolSize);
        poolObjects = new GameObject[_poolSize];
    }

    public void CreateObject()
    {
        for (int i = 0; i < poolObjects.Length; i++)
        {
            var parent = transform.parent;
            poolObjects[i] = _objectPool.CreateObjectWithNewParent( parent.position, parent.rotation);
        }
    }
}