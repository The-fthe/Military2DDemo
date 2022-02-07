using InGameAsset.Scripts.Player;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class OnScreenAim : OnScreenControl, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public BoolGameData isDraggingData;
    [SerializeField] Image[] _directionImage;
    
       public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData == null)
                throw new System.ArgumentNullException(nameof(eventData));

            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponentInParent<RectTransform>(), eventData.position, eventData.pressEventCamera, out m_PointerDownPos);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData == null)
                throw new System.ArgumentNullException(nameof(eventData));
            if(isDraggingData != null)
                isDraggingData.RunTimeValue = true;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponentInParent<RectTransform>(), eventData.position, eventData.pressEventCamera, out var position);
            var delta = position - m_PointerDownPos;

            delta = Vector2.ClampMagnitude(delta, movementRange);
            ((RectTransform)transform).anchoredPosition = m_StartPos + (Vector3)delta;
            
            var newPos = new Vector2(delta.x / movementRange, delta.y / movementRange);
            DisplayFaceDir(CheckDir(newPos));
            SendValueToControl(newPos);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            ((RectTransform)transform).anchoredPosition = m_StartPos;
            HandleResePosition();
            SendValueToControl(Vector2.zero);
        }

        public void SetCouldFireToTrue()
        {
            isDraggingData.RunTimeValue = true;
        }

        private void Start()
        {
            HandleResePosition();
            m_StartPos = ((RectTransform)transform).anchoredPosition;
        }

        public float movementRange
        {
            get => m_MovementRange;
            set => m_MovementRange = value;
        }

        [FormerlySerializedAs("movementRange")]
        [SerializeField]
        private float m_MovementRange = 50;

        [InputControl(layout = "Vector2")]
        [SerializeField]
        private string m_ControlPath;

        private Vector3 m_StartPos;
        private Vector2 m_PointerDownPos;

        protected override string controlPathInternal
        {
            get => m_ControlPath;
            set => m_ControlPath = value;
        }

        void DisplayFaceDir(FaceDir faceDir)
        {
            var dirColor = _directionImage[0].color;
            var showColor = new Color(dirColor.r, dirColor.g, dirColor.b, 1);
            var hideColor = new Color(dirColor.r, dirColor.g, dirColor.b, 0);
            foreach (var image in _directionImage)
            {
                image.color = hideColor;
            }
            switch (faceDir)
            {
                case FaceDir.UpLeft:
                    _directionImage[0].color = showColor;
                    break;
                case FaceDir.UpRight:
                    _directionImage[1].color = showColor;
                    break;
                case FaceDir.DownRight:
                    _directionImage[2].color = showColor;
                    break;
                case FaceDir.DownLeft:
                    _directionImage[3].color = showColor;
                    break;
                case FaceDir.Center:
                    foreach (var image in _directionImage)
                    {
                        image.color = hideColor;
                    }
                    break;
            }
        }

        void HandleResePosition()
        {
            var dirColor = _directionImage[0].color;
            var hideColor = new Color(dirColor.r, dirColor.g, dirColor.b, 0);
            foreach (var image in _directionImage)
            {
                image.color = hideColor;
            }
        }
        
        FaceDir CheckDir(Vector2 dir)
        {
            if (dir.x >0 && dir.y >0)
                return FaceDir.UpRight;
            if( dir.x <0 && dir.y> 0)
                return FaceDir.UpLeft;
            if (dir.x < 0 && dir.y < 0)
                return FaceDir.DownLeft;
            if (dir.x > 0 && dir.y < 0)
                return FaceDir.DownRight;
            return FaceDir.Center;
        }

        enum FaceDir
        {
            UpLeft,UpRight,DownLeft,DownRight,Center
        }
}


