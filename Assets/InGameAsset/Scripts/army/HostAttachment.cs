using System.Collections;
using System.Linq;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace InGameAsset.Scripts.army
{
    public class HostAttachment : MonoBehaviour
    {
        public Attachment[] _attachments;
        [SerializeField] Attachment[] _loadedAttachment;
        [SerializeField] AttachmentBuff[] _distrubutionAttachments;
        [SerializeField] Transform _attachedTarget;
        [SerializeField] float _arcRadius = 0.5f;
        [SerializeField] float _angleBetAttachments = 5;
        [SerializeField, Range(3, 12)] int _triangleSide = 3;
        [SerializeField, Range(0, 180)] float _arcAngle = 45;

        Vector3[] itemCenters;
        float[] _attachmentRadiis;
        
        void OnValidate()
        {
            if (_distrubutionAttachments.Length <= 0)
            {
                _distrubutionAttachments = new[]
                {
                    AttachmentBuff.army,
                    AttachmentBuff.army,
                    AttachmentBuff.army,
                    AttachmentBuff.drone,
                    AttachmentBuff.drone,
                    AttachmentBuff.drone,
                    AttachmentBuff.shelid,
                    AttachmentBuff.shelid,
                    AttachmentBuff.shootgun
                };
            }
        }

        void Awake()
        {
            _attachments = new Attachment[_triangleSide];
            LoadResourceAttachment();
            _angleBetAttachments = 360f / _triangleSide;
            AddAttachmentToPos();
            for (int i = 0; i < _distrubutionAttachments.Length; i++)
            {
                _attachments[i].Initialize();
                _attachments[i].gameObject.SetActive(false);
            }
        }

        void Update()
        {
            transform.position = _attachedTarget.position;
            foreach (var attachment in _attachments)
            {
                if(attachment.gameObject.activeSelf)
                    attachment.tickUpdate();
            }
        }

        public void AddAttachmentToPos()
        {
            for (int i = 0; i < _distrubutionAttachments.Length; i++)
            {
                for (int j = 0; j < _loadedAttachment.Length; j++)
                {
                    if (_loadedAttachment[j].AttachmentBuff == _distrubutionAttachments[i])
                    {
                        Attachment attachment = Instantiate(_loadedAttachment[j], _loadedAttachment[j].transform.position,
                            quaternion.identity);
                        attachment.name += "_" + i;
                        attachment.transform.parent = transform;
                        _attachments[i] = attachment;
                        UpdateAllAttachmentRadius();
                    }
                }
            }
            StartCoroutine(SeperatingAttachment());
        }

        void UpdateAllAttachmentRadius()
        {
            for (int i = 0; i < _distrubutionAttachments.Length; i++)
            {
                for (int j = 0; j < _loadedAttachment.Length; j++)
                {
                    if (_loadedAttachment[j].AttachmentBuff == _distrubutionAttachments[i])
                    {
                        _attachmentRadiis[j] = _loadedAttachment[j].GetSpriteRadius;
                    }
                }
            }
        }

        public void SetAttachmentStateToActive(AttachmentBuff attachmentBuff)
        {
            for (int i = 0; i < _attachments.Length; i++)
            {
                if (_distrubutionAttachments[i] == attachmentBuff &&  _attachments[i].gameObject.activeSelf == false)
                {
                    _attachments[i].gameObject.SetActive(true);
                    return;
                }
            }
        }
        public void SetAttachmentStateToInactive(AttachmentBuff attachmentBuff)
        {
            for (int i = 0; i < _attachments.Length; i++)
            {
                if (_distrubutionAttachments[i] == attachmentBuff &&  _attachments[i].gameObject.activeSelf)
                {
                    _attachments[i].gameObject.SetActive(false);
                    return;
                }
            }
        }
        public void SetAllAttachmentStateToActive(AttachmentBuff attachmentBuff)
        {
            for (int i = 0; i < _attachments.Length; i++)
            {
                if (_distrubutionAttachments[i] == attachmentBuff &&  !_attachments[i].gameObject.activeSelf)
                {
                    _attachments[i].gameObject.SetActive(true);
                }
            }
        }

        IEnumerator SeperatingAttachment( )
        {
            if (_distrubutionAttachments.Length <= 0) yield break;
                int itemCount = _triangleSide;
                float[] angleBetween = new float[itemCount];

                for (int i = 0; i < angleBetween.Length - 1; i++)
                {
                    float aRadius = _attachmentRadiis[i];
                    float bRadius = _attachmentRadiis[i + 1];
                    float abLength = aRadius + bRadius;
                    float ang = Mathf.Acos(1f - (abLength * abLength) / (2 * _arcRadius * _arcRadius));
                    angleBetween[i] = ang < _angleBetAttachments * Mathf.Deg2Rad ? _angleBetAttachments * Mathf.Deg2Rad : ang;
                }
                float angRad = 0;
                SetAttachmentPosAroundArc(angRad, angleBetween);
        }

        void SetAttachmentPosAroundArc(float angRad, float[] angleBetween)
        {
            itemCenters = new Vector3[angleBetween.Length];
            for (int i = 0; i < itemCenters.Length; i++)
            {
                itemCenters[i] = MathfPlus.AngToDir(angRad) * _arcRadius;
                angRad += angleBetween[i] * Mathf.Rad2Deg;
            }

            for (int i = 0; i < _attachments.Length; i++)
            {
                _attachments[i].SetPos(itemCenters[i]);
                _attachments[i].setRot(itemCenters[i]);
            }
        }

        public void LoadResourceAttachment()
        {
            if (_loadedAttachment.Length <= 0)
            {
                _loadedAttachment = Resources.LoadAll("Attachments",
                    typeof(Attachment)).Cast<Attachment>().ToArray();
                _attachmentRadiis = new float[_triangleSide];
            }
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            using (new Handles.DrawingScope(transform.localToWorldMatrix))
            {
                Handles.DrawWireArc(default, Vector3.forward, Vector3.right, _arcAngle, _arcRadius);
                Handles.DrawWireArc(default, Vector3.forward, Vector3.right, -_arcAngle, _arcRadius);
            }

            Gizmos.color = Color.green;
            Gizmos.DrawLine(_attachedTarget.transform.position,
                _attachedTarget.transform.position + (_attachedTarget.transform.right * 5));
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(_attachedTarget.transform.position,
                _attachedTarget.transform.position + (_attachedTarget.transform.up * 5));
            Gizmos.color = Color.magenta;
        }
#endif
    }
}