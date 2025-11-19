using Assets.Scripts.Managers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Gameplay.Core.Card
{
    public class CardDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        #region Private Variables

        private Vector2 _startPosition;
        private Vector2 _offset;
        private Vector3 _startScale;
        private bool _isScaling = false;
        private Coroutine _scaleCoroutine;

        [Tooltip("Distance required to trigger the disappear effect.")]
        [SerializeField] private float _disappearDistance = 150f;

        [Tooltip("Duration of the scale transition effect.")]
        [SerializeField] private float _scaleDuration = 1f;

        #endregion

        #region Public Functions

        public void OnBeginDrag(PointerEventData eventData)
        {
            var rt = transform as RectTransform;
            _startPosition = rt.anchoredPosition;
            _startScale = transform.localScale;
            _isScaling = false;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rt.parent as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out _offset
            );

            _offset = rt.anchoredPosition - _offset;
            EventManager.Instance.Trigger(EventName.CardDrag);
        }

        public void OnDrag(PointerEventData eventData)
        {
            var rt = transform as RectTransform;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rt.parent as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint
            );

            rt.anchoredPosition = localPoint + _offset;

            float distance = rt.anchoredPosition.y - _startPosition.y;

            if (!_isScaling && distance >= _disappearDistance)
            {
                _isScaling = true;
                if (_scaleCoroutine != null) StopCoroutine(_scaleCoroutine);
                _scaleCoroutine = StartCoroutine(transform.LerpScaleOverTime(Vector3.zero, _scaleDuration));
            }
            else if (_isScaling && distance < _disappearDistance)
            {
                _isScaling = false;
                if (_scaleCoroutine != null) StopCoroutine(_scaleCoroutine);
                _scaleCoroutine = StartCoroutine(transform.LerpScaleOverTime(_startScale, _scaleDuration));
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            var rt = transform as RectTransform;
            rt.anchoredPosition = _startPosition;

            if (_scaleCoroutine != null) StopCoroutine(_scaleCoroutine);

            transform.localScale = _startScale;
            EventManager.Instance.Trigger(EventName.CardEndDrag, eventData, gameObject);
        }

        #endregion
    }
}