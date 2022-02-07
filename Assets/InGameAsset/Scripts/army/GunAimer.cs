using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using InGameAsset.Scripts.Data;
using InGameAsset.Scripts.Managers;
using InGameAsset.Scripts.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InGameAsset.Scripts.army
{
    public class GunAimer : Attachment
    {
        [SerializeField] ArmyData armyData;
        [SerializeField] BoolGameData canStartShoot;
        [SerializeField] Gun _gun;
        [SerializeField] Camera _mainCamera;
        [SerializeField] PlayerManager _playerManager;

        float _desiredAngle;
        Vector2 _aimVec2;
        void Start()
        {
            _playerManager = FindObjectOfType<PlayerManager>();
            _playerManager.OnPlayerActivated.AddListener(Initialize);
        }

        public override void Initialize()
        { 
            _mainCamera ??= Camera.main;
            AttachmentBuff = AttachmentBuff.army;
            _desiredAngle = gameObject.transform.rotation.eulerAngles.z;
            _gun ??= GetComponentInChildren<Gun>();
            _gun.Initialize();
        }

        public void OnLook(InputValue inputValue)
        {
            if (_mainCamera == null) _mainCamera = Camera.main;
            _aimVec2 = inputValue.Get<Vector2>() != Vector2.zero? inputValue.Get<Vector2>(): _aimVec2;
            Aim(_aimVec2);
        }

        public void OnMouseLook(InputValue inputValue)
        {
            if (_mainCamera == null) _mainCamera = Camera.main;
            Vector3 pointerPosition = inputValue.Get<Vector2>();
            if (pointerPosition.x < -1 && pointerPosition.y < -1 || pointerPosition.x > 1 && pointerPosition.y > 1)
            {
                Vector3 mousePosition = pointerPosition;
                mousePosition.z = _mainCamera.nearClipPlane;
                Vector2 mouseWorldPosition = _mainCamera.ScreenToWorldPoint(mousePosition);
                _aimVec2 = (mouseWorldPosition - (Vector2) gameObject.transform.position)
                    .normalized;
                Aim(_aimVec2);
            }
            else
            {
                _aimVec2 = pointerPosition;
                Aim(_aimVec2);
            }
        }


        public override void tickUpdate()
        {
            if (!canStartShoot) return;
            _gun.Shoot();
            var rotationstep = armyData.rotationSpeed * Time.deltaTime;
            transform.rotation=  Quaternion.RotateTowards(transform.rotation,
                Quaternion.Euler(0,0,_desiredAngle),rotationstep);
        }
    
        void Aim(Vector2 inputPointerPosition)
        {
            _mainCamera ??= Camera.main;
            _aimVec2 = inputPointerPosition;
             if (inputPointerPosition.x < 0 && inputPointerPosition.y < 0 || inputPointerPosition.x > 1 && inputPointerPosition.y > 1 )
             {

                 Vector3 mousePosition = inputPointerPosition;
                // mousePosition.z = mainCamera.nearClipPlane;
                 Vector2 mouseWorldPosition = _mainCamera.ScreenToWorldPoint(mousePosition);
                  _aimVec2 = ((Vector3) mouseWorldPosition - transform.position).normalized;
                 _desiredAngle = Mathf.Atan2(_aimVec2.y, _aimVec2.x) * Mathf.Rad2Deg;

             }else
                 _desiredAngle = Mathf.Atan2(inputPointerPosition.y, inputPointerPosition.x) * Mathf.Rad2Deg;
        }
        
    }
}