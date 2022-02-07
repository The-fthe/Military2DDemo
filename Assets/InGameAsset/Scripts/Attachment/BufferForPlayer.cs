using System;
using Cysharp.Threading.Tasks;
using InGameAsset.Scripts.army;
using InGameAsset.Scripts.Util;
using UnityEngine;
using UnityEngine.Events;

public class BufferForPlayer : MonoBehaviour
{
    [SerializeField]AttachmentBuff _attachmentBuff;
    [SerializeField] HostAttachment _hostAttachment;
    public UnityEvent OnWaitTrigger;
    public SpriteRenderer sprite;
    public UnityEvent OnBuffTrigger;

    void Start()
    {
        OnWaitTrigger?.Invoke();
    }

    void OnEnable()
    {
        OnWaitTrigger?.Invoke();
    }

    async void HandleBuffTrigger()
    {
        OnBuffTrigger?.Invoke();
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == Const.PLAYER_MOVE_UNIT_NAME)
        {
            var hostAttachment= other.gameObject.GetComponentInParent<PlayerInputEvent>().gameObject;
            hostAttachment.GetComponentInChildren<HostAttachment>().SetAllAttachmentStateToActive(_attachmentBuff);
            sprite.color = new Color(1, 1, 1,0);
            HandleBuffTrigger();
        }
    }
}
