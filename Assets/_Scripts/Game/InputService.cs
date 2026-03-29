using System;
using UnityEngine;
using Zenject;

namespace _Scripts.Game
{
    public class InputService : ITickable
    {
        public event Action<Vector2Int> OnSwipe;

        private Vector2 _startPos;
        private const float Threshold = 30f;

        public void Tick()
        {
            if (Input.GetMouseButtonDown(0))
                _startPos = Input.mousePosition;

            if (Input.GetMouseButtonUp(0))
            {
                var delta = (Vector2)Input.mousePosition - _startPos;
                if (delta.magnitude < Threshold) return;

                var abs = new Vector2(Mathf.Abs(delta.x), Mathf.Abs(delta.y));

                if (abs.x > abs.y)
                    OnSwipe?.Invoke(delta.x > 0 ? Direction.Right : Direction.Left);
                else
                    OnSwipe?.Invoke(delta.y < 0 ? Direction.Down : Direction.Up);
            }
        }
    }
}