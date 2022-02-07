using System;
using System.Collections;
using System.Collections.Generic;
using InGameAsset.Scripts.army;
using InGameAsset.Scripts.Util;
using UnityEngine;

public class SetTriggerAttachmentOff : MonoBehaviour
{
    [SerializeField]AttachmentBuff _attachmentBuff;
    HostAttachment _hostAttachment;
    
    void Start()
    {
        _hostAttachment ??= FindObjectOfType<HostAttachment>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
       if (other.gameObject.name == Const.PLAYER_MOVE_UNIT_NAME)
        {
            _hostAttachment.SetAttachmentStateToInactive(_attachmentBuff);
            gameObject.SetActive(false);
        }
    }
}
