using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    [RequireComponent(typeof(Button))]
    public class PuzzleNumberButton : MonoBehaviour
    {
        [SerializeField] private int digit;
        [SerializeField] private PuzzleNumberCodeUIBase target;

        private Button button;

        public int Digit
        {
            get { return digit; }
        }

        private void Awake()
        {
            CacheReferences();
        }

        private void OnEnable()
        {
            CacheReferences();

            if (button != null)
            {
                button.onClick.RemoveListener(HandleClick);
                button.onClick.AddListener(HandleClick);
            }
        }

        private void OnDisable()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(HandleClick);
            }
        }

        private void Reset()
        {
            CacheReferences();
        }

        public void SetTarget(PuzzleNumberCodeUIBase newTarget)
        {
            target = newTarget;
        }

        private void HandleClick()
        {
            if (target == null)
            {
                target = GetComponentInParent<PuzzleNumberCodeUIBase>();
            }

            if (target == null)
            {
                Debug.LogWarning("PuzzleNumberButton target is missing: " + name, this);
                return;
            }

            target.AppendDigit(digit);
        }

        private void CacheReferences()
        {
            digit = Mathf.Clamp(digit, 0, 9);

            if (button == null)
            {
                button = GetComponent<Button>();
            }

            if (target == null)
            {
                target = GetComponentInParent<PuzzleNumberCodeUIBase>();
            }
        }
    }
}
