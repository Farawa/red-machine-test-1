using System;
using Events;
using UnityEngine;

namespace Connection
{
    public class ColorNodeTarget : MonoBehaviour
    {
        [SerializeField] private Color targetColor;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private ColorNode colorNode;

        public bool IsCompleted => targetColor == colorNode.Color;

        public event Action<ColorNodeTarget, bool> TargetCompletionChangeEvent;


        private void Awake()
        {
            colorNode.ColorChangedEvent += OnColorChanged;
        }

        private void OnDestroy()
        {
            colorNode.ColorChangedEvent -= OnColorChanged;
        }

        private void OnColorChanged(Color currentColor)
        {
            TargetCompletionChangeEvent?.Invoke(this, IsColorsSame(targetColor, currentColor));
        }

        private bool IsColorsSame(Color firstColor, Color secondColor)
        {
            if (Mathf.Abs(firstColor.r - secondColor.r) > 0.05) return false;
            if (Mathf.Abs(firstColor.g - secondColor.g) > 0.05) return false;
            if (Mathf.Abs(firstColor.b - secondColor.b) > 0.05) return false;
            return true;
        }

        private void OnValidate()
        {
            spriteRenderer.color = targetColor;
        }
    }
}
