using System.Collections;
using InGameAsset.Scripts.Data;
using InGameAsset.Scripts.Util;
using UnityEngine;
using UnityEngine.Events;

namespace InGameAsset.Scripts
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] HandlingHit _handlingHit;
         BulletData _bulletData;
        Vector2 _startPosition;
        float _conquaredDistance;
        Rigidbody2D _rb2d;
        BoxCollider2D _colLengthY2D;
        int _bounce;
        bool _isAccelerationActive;
        IEnumerator updateBullet;

        public UnityEvent OnHit = new UnityEvent();
        public UnityEvent OnBounce = new UnityEvent();
        public UnityEvent OnOutBound = new UnityEvent();

        public string GetBulletOwnerTag()
        {
            return _bulletData.TagOwner.ToString();
        }

        public void Initialize(BulletData bulletData)
        {
            _bulletData = bulletData;
            _startPosition = transform.position;
            _rb2d.velocity = Vector2.zero;
            Vector2 accleration = transform.up * bulletData.InitialSpeed;
            _rb2d.AddForce(accleration, ForceMode2D.Impulse);
            _colLengthY2D ??= GetComponent<BoxCollider2D>();
            if(!bulletData.CanReactToBullet)
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Bullet"),LayerMask.NameToLayer("Bullet"));
            _bounce = bulletData.Bounce;
            gameObject.tag = bulletData.TagOwner.ToString();
            if ( bulletData.UseAcceleration)
            {
                if(updateBullet != null)
                    StopCoroutine(updateBullet);
                updateBullet = UpdateBulletSpeedIEnumerator(bulletData.UpdateFrequency,bulletData.LimitMaxSpeed);
                StartCoroutine(updateBullet);
            }
        }

        void Awake() => _rb2d = GetComponent<Rigidbody2D>();

        //void Start() => Initialize(_bulletData);

        void Update()
        {
            CheckBulletMaxDistace();
        }

        void CheckBulletMaxDistace()
        {
            _conquaredDistance += Vector2.Distance(transform.position, _startPosition);
            if (_conquaredDistance > _bulletData.MaxDistance)
            {
                OnOutBound?.Invoke();
                _conquaredDistance = 0;
                gameObject.SetActive(false);
            }
            else
                _startPosition = transform.position;
        }

        IEnumerator UpdateBulletSpeedIEnumerator(float updateFrequency,bool limitMaxSpeed)
        {
            var maxSpeed = _bulletData.MaxSpeed;
            while (_rb2d.velocity.sqrMagnitude > 1f)
            {
                yield return Helpers.GetWait(updateFrequency);
                var acceleration = _bulletData.AcceleartionPerSec* updateFrequency;
                if (limitMaxSpeed)
                    if (_rb2d.velocity.sqrMagnitude >= maxSpeed * maxSpeed)
                    {
                        Debug.Log("Max Speed is trigger");
                        continue;
                    }
                _rb2d.AddForce(transform.up * acceleration, ForceMode2D.Impulse);
            }
            _rb2d.velocity = Vector2.zero;
            OnOutBound?.Invoke();
            _conquaredDistance = 0;
            StopCoroutine(updateBullet);
            gameObject.SetActive(false);
        }

        void DisableObject()
        {
            OnHit?.Invoke();
            gameObject.SetActive(false);
        }

        void OnDisable()
        {
            _rb2d.velocity = Vector2.zero;
            _conquaredDistance = 0;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other != null)
            {
                var hit = Physics2D.Raycast(transform.position, transform.up,
                    _colLengthY2D.size.y, _bulletData.BounceLayer);
                if (hit.collider != null && _bounce > 0)
                {
                    int bounceLayerName = (int) Mathf.Log(_bulletData.BounceLayer.value, 2);
                    if (hit.collider.gameObject.layer == bounceLayerName)
                    {
                        Bounces(hit);
                    }
                }
                else if (other != null)
                {
                    DestroySelfOnLayer(other, new string[] {"Hitable", _bulletData.TargetUnit.ToString()});
                    DestroySelfOnTag(other, _bulletData.TargetUnit.ToString());
                }
            }
        }

        void DestroySelfOnLayer(Collider2D col2D, string[] LayerTagNames)
        {
            foreach (var layerTagName in LayerTagNames)
            {
                if (col2D.gameObject.layer == LayerMask.NameToLayer(layerTagName))
                {
                    var damagable = col2D.GetComponent<Damageable>();
                    if (damagable != null) damagable.Hit(_bulletData.Damage);
                    DisableObject();
                    return;
                }
            }
        }

        void DestroySelfOnTag(Collider2D col2D, string LayerTagName)
        {
            if (col2D.gameObject.CompareTag(LayerTagName))
            {
                col2D.GetComponent<Bullet>().DisableObject();
            }
        }

        void Bounces(RaycastHit2D hit)
        {
            var refelctedDirection = Vector3.Reflect(_rb2d.velocity, hit.normal);
            transform.rotation = Quaternion.LookRotation(Vector3.forward, refelctedDirection);
            _rb2d.velocity = transform.up * _bulletData.InitialSpeed;
            OnBounce.Invoke();
            _bounce--;
        }
        

        void FixedUpdate()
        {
            // if (!isAccelerationActive) return;
            // if (_rb2d.velocity.sqrMagnitude > 1f)
            // {
            //     var accelearation = _bulletData.AcceleartionPerSec * Time.deltaTime;
            //     _rb2d.AddForce(transform.up * accelearation, ForceMode2D.Impulse);
            //     Debug.Log($"acceleartion is trigger");
            // }
            // else
            // {
            //     _rb2d.velocity = Vector2.zero;
            //     OnOutBound?.Invoke();
            //     _conquaredDistance = 0;
            //     gameObject.SetActive(false);
            // }
        }
    }
}