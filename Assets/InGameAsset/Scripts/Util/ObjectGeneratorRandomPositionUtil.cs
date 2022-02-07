    using System;
using System.Collections;
using System.Collections.Generic;
using InGameAsset.Scripts.Player;
using InGameAsset.Scripts.Util;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectGeneratorRandomPositionUtil : MonoBehaviour
{
    [SerializeField] GameObject _objectPrefab;
    [SerializeField] float radius = 0.01f;
    [SerializeField] ObjectPool _objectPool;
    [SerializeField] int _poolSize=3;
    [SerializeField] GameObject[] poolObjects;
    float UnitScale=1;
    

    void Awake()
    {
        _objectPool ??= gameObject.AddComponent(typeof(ObjectPool))as ObjectPool;
        _objectPool.SetObjectPoolAndPoolSize(_objectPrefab,_poolSize);
        poolObjects = new GameObject[_poolSize];
        if(_objectPrefab == null) Debug.LogError("objetPrefab haven been assign");
    }

    void Start()
    {
        if(GetComponentInParent<UnitStatus>() != null)
            UnitScale = GetComponentInParent<UnitStatus>().UnitScale * 2;
    }

    protected Vector2 GetRandomPosition()
    {
        return Random.insideUnitCircle * radius + (Vector2) transform.position;
    }

    protected Quaternion Random2DRotation()
    {
        return Quaternion.Euler(0,0,Random.Range(0,360));
    }

    public void CreateObject()
    {
        Vector2 position = GetRandomPosition();
        for (int i = 0; i < poolObjects.Length; i++)
        {
            poolObjects[i] = _objectPool.CreateObjectWithNewParent(position,transform.parent.rotation);
            Vector3 newScale = new Vector3(UnitScale, UnitScale, UnitScale);
            poolObjects[i].transform.localScale =poolObjects[i].transform.localScale.x <=1 ? poolObjects[i].transform.localScale 
                :newScale;
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position,radius);
    }
}