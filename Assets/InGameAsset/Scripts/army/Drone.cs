using System;
using UnityEngine;

namespace InGameAsset.Scripts.army
{
    public class Drone:Attachment
    {
        

        void Start()
        {
            AttachmentBuff = AttachmentBuff.drone;
        }
    }
}