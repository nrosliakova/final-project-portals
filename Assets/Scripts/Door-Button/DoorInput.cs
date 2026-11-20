using UnityEngine;

namespace Door_Button
{
    public class DoorInput : MonoBehaviour, IActivatable
    {
        [SerializeField] private SlidingDoor door;
        [SerializeField] private Renderer indicator;
        [SerializeField] private Color activeColor = Color.green;
        [SerializeField] private Color inactiveColor = Color.red;

        public bool IsActive { get; private set; }

        private MaterialPropertyBlock propertyBlock;

        private void Awake() => UpdateIndicator();

        public void SetActive(bool active)
        {
            if (IsActive == active) return;

            IsActive = active;
            UpdateIndicator();
            door?.RefreshInputs();
        }

        private void UpdateIndicator()
        {
            Color color = IsActive ? activeColor : inactiveColor;

            if (indicator != null)
            {
                propertyBlock ??= new MaterialPropertyBlock();
                indicator.GetPropertyBlock(propertyBlock);
                propertyBlock.SetColor("_BaseColor", color);
                indicator.SetPropertyBlock(propertyBlock);
            }
            
        }
    }
}