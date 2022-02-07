using System;
using UnityEngine;

namespace InGameAsset.Scripts.Util
{
    public class InstantiateUtil : MonoBehaviour
    {
        [SerializeField] GameObject _objectPrefab;
        [SerializeField] ObjectPool _objectPool;
        [SerializeField] int _poolSize=3;

        [SerializeField] GameObject[] poolObjects;

        int index = 0;
        public void Awake()
        {
            _objectPool ??=gameObject.AddComponent(typeof(ObjectPool))as ObjectPool;
            _objectPool.SetObjectPoolAndPoolSize(_objectPrefab,_poolSize);
                poolObjects= new GameObject[_poolSize];
        }

        public void InstantiateObject()
        {
            if (index >= _poolSize) index = 0;
            poolObjects[index] =_objectPool.CreateObjectWithNewParent(transform.position,Quaternion.identity);
            index++;
        }

        public void DestroyGameObject()
        {
            if (index >= _poolSize) index = 0;
            _objectPool.ReturnToPool(poolObjects[index]);
        }

        string GetPoolName( GameObject poolObject)
        {
            return "pool_" + poolObject.name;
        }

    }
}