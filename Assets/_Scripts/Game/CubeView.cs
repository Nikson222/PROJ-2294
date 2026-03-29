using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using DG.Tweening;

namespace _Scripts.Game
{
    public class CubeView : MonoBehaviour
    {
        public CubeType Type { get; private set; }
        public Vector2Int GridPosition { get; private set; }
        public bool IsMoving { get; private set; }

        public event Action<Vector2Int, Vector2Int, CubeView> OnMoved;

        private Image _image;
        [SerializeField] private ParticleSystem particles;

        [Inject(Id = "CubeContainer")]
        private Transform _cubeContainer;

        private const float DefaultMoveDuration = 0.2f;
        private const Ease DefaultMoveEasing = Ease.OutCubic;
        private const float DefaultBouncePartDuration = 0.1f;

        private void Awake() => _image = GetComponent<Image>();

        public void Initialize(CubeType type, Sprite sprite, Vector2Int gridPosition)
        {
            Type = type;
            GridPosition = gridPosition;
            _image.sprite = sprite;
            _image.color = Color.white;
        }

        public void PlayParticlesEffect(Vector3 position)
        {
            if (particles != null)
            {
                particles.Play();
            }
        }

        public void HideVisuals()
        {
            if (_image != null)
            {
                _image.enabled = false;
            }
        }

        public Vector2 GetVisualDimensions()
        {
            RectTransform rt = GetComponent<RectTransform>();
            if (rt != null)
                return rt.rect.size;
                
            return new Vector2(50, 50);
        }

        private IEnumerator DoAnimateToWorldPosition(Vector3 worldTarget, float duration = DefaultMoveDuration, bool instant = false, Ease easing = DefaultMoveEasing)
        {
            if (transform.parent != _cubeContainer)
            {
                transform.SetParent(_cubeContainer);
                transform.SetAsLastSibling();
            }

            if (instant)
            {
                transform.position = worldTarget;
            }
            else
            {
                var tween = transform.DOMove(worldTarget, duration).SetEase(easing);
                yield return tween.WaitForCompletion();
            }
        }

        public IEnumerator MoveTo(Vector2Int targetPos, RectTransform targetCell)
        {
            if (IsMoving)
                yield break;

            IsMoving = true;
            Vector2Int start = GridPosition;

            yield return DoAnimateToWorldPosition(targetCell.position);

            transform.SetParent(targetCell);
            transform.localPosition = Vector3.zero;
            GridPosition = targetPos;

            OnMoved?.Invoke(start, targetPos, this);
            IsMoving = false;
        }

        public IEnumerator MoveToWorldOffset(Vector3 worldTargetPosition, bool instant = false)
        {
            yield return AnimateToWorldPosition(worldTargetPosition, DefaultBouncePartDuration, instant);
        }

        private IEnumerator AnimateToWorldPosition(Vector3 worldTarget, float duration = DefaultMoveDuration, bool instant = false, Ease easing = DefaultMoveEasing)
        {
            if (IsMoving && !instant)
                yield break;
            
            IsMoving = true;
            yield return DoAnimateToWorldPosition(worldTarget, duration, instant, easing);
            IsMoving = false;
        }

        public IEnumerator ScaleDownAndDestroy()
        {
            var tween = transform.DOScale(Vector3.zero, 0.2f);
            yield return tween.WaitForCompletion();
            Destroy(gameObject);
        }
    }
}
