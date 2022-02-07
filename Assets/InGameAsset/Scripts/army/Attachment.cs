using UnityEngine;

namespace InGameAsset.Scripts.army
{
    public abstract class Attachment : MonoBehaviour,IAttachment
    {
        [SerializeField]SpriteRenderer _sprite;
        public AttachmentBuff AttachmentBuff;
        public float GetSpriteRadius => GetSpriteScaleRadius(_sprite);

        void OnValidate()
        {
            _sprite ??= GetComponentInChildren<SpriteRenderer>();
        }
        public virtual void Initialize(){}
        public virtual void tickUpdate()
        {
        }
        public virtual void SetPos(Vector3 pos)
        {
            transform.localPosition = pos;
        }

        float GetSpriteScaleRadius(SpriteRenderer sprite)
        {
            Vector2 spriteSize = sprite.size;
            float spriteRadius = spriteSize.x > spriteSize.y ? spriteSize.x : spriteSize.y;
            return spriteRadius/2*transform.localScale.x;
        }

        public void DoAttachmentStuff()
       {
           throw new System.NotImplementedException();
       }

        public virtual void setRot(Vector3 angWithRadius)
        {
          
        }

        public void ChangeAttachment(IAttachment attachment)
       {
           throw new System.NotImplementedException();
       }
    }
}

public interface IAttachment
{
    void DoAttachmentStuff();
    void ChangeAttachment(IAttachment attachment);
}