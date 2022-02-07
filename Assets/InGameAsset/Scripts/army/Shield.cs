using InGameAsset.Scripts.Player;
using UnityEngine;

namespace InGameAsset.Scripts.army
{
    public class Shield:Attachment
    {
        [SerializeField] ShowInvisibleEffect _showInvisibleEffect;
        [SerializeField] GameObject _parent;
        ShieldHandleHit _shieldHandleHit = new ShieldHandleHit();
        void Start()
        {
            _parent = FindObjectOfType<PlayerManager>().GetPlayerGO;
            _showInvisibleEffect ??= GetComponent<ShowInvisibleEffect>();
            
            var unitStatus = _parent.GetComponentInChildren<UnitStatus>();
            var tankAnimation = _parent.GetComponentInChildren<TankAnimation>();
            var damagaable = _parent.GetComponentInChildren<Damageable>();
            _showInvisibleEffect.Initialize(unitStatus,tankAnimation,damagaable);
            damagaable.SetHitHandling(_shieldHandleHit);
        }
        public override void setRot(Vector3 angWithRadius)
        {
            Vector3 dirToAng = angWithRadius.normalized;
            float ang = MathfPlus.DirToAng(dirToAng);
            transform.localRotation= Quaternion.AngleAxis(ang,Vector3.forward);
        }
    }
}