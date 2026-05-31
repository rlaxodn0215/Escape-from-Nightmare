using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    public class TitleBackgroundFlicker : MonoBehaviour
    {
        [SerializeField] private Image targetImage;
        [SerializeField, Range(0f, 1f)] private float baseAlpha = 1f;
        [SerializeField, Range(0f, 1f)] private float minOffAlpha = 0.25f;
        [SerializeField, Range(0f, 1f)] private float maxOffAlpha = 0.45f;
        [SerializeField, Range(1f, 20f)] private float minIdleInterval = 4f;
        [SerializeField, Range(1f, 30f)] private float maxIdleInterval = 9f;
        [SerializeField, Range(1, 5)] private int minFlickerCount = 1;
        [SerializeField, Range(1, 5)] private int maxFlickerCount = 3;
        [SerializeField, Range(0.01f, 0.5f)] private float minOffDuration = 0.05f;
        [SerializeField, Range(0.01f, 0.5f)] private float maxOffDuration = 0.16f;
        [SerializeField, Range(0.01f, 0.5f)] private float minOnDuration = 0.04f;
        [SerializeField, Range(0.01f, 0.5f)] private float maxOnDuration = 0.12f;

        private Color originalColor;
        private bool hasOriginalColor;
        private float nextStateTime;
        private int remainingFlickers;
        private bool isOff;

        private void Awake()
        {
            EnsureTargetImage();
        }

        private void OnEnable()
        {
            EnsureTargetImage();
            if (targetImage == null)
            {
                return;
            }

            originalColor = targetImage.color;
            hasOriginalColor = true;
            remainingFlickers = 0;
            isOff = false;
            nextStateTime = Time.unscaledTime + Random.Range(minIdleInterval, maxIdleInterval);
            ApplyAlpha(baseAlpha);
        }

        private void OnDisable()
        {
            if (targetImage != null && hasOriginalColor)
            {
                targetImage.color = originalColor;
            }
        }

        private void Update()
        {
            if (targetImage is null)
            {
                return;
            }

            if (Time.unscaledTime >= nextStateTime)
            {
                AdvanceState();
            }
        }

        private void AdvanceState()
        {
            if (remainingFlickers <= 0 && !isOff)
            {
                remainingFlickers = Random.Range(minFlickerCount, maxFlickerCount + 1);
            }

            if (isOff)
            {
                isOff = false;
                ApplyAlpha(baseAlpha);
                remainingFlickers--;
                nextStateTime = Time.unscaledTime + (remainingFlickers > 0
                    ? Random.Range(minOnDuration, maxOnDuration)
                    : Random.Range(minIdleInterval, maxIdleInterval));
                return;
            }

            isOff = true;
            ApplyAlpha(Random.Range(minOffAlpha, maxOffAlpha));
            nextStateTime = Time.unscaledTime + Random.Range(minOffDuration, maxOffDuration);
        }

        private void ApplyAlpha(float alpha)
        {
            Color color = targetImage.color;
            color.a = Mathf.Clamp01(alpha);
            targetImage.color = color;
        }

        private void EnsureTargetImage()
        {
            if (targetImage == null)
            {
                targetImage = GetComponent<Image>();
            }
        }

        private void OnValidate()
        {
            if (maxOffAlpha < minOffAlpha)
            {
                maxOffAlpha = minOffAlpha;
            }

            if (maxIdleInterval < minIdleInterval)
            {
                maxIdleInterval = minIdleInterval;
            }

            if (maxFlickerCount < minFlickerCount)
            {
                maxFlickerCount = minFlickerCount;
            }

            if (maxOffDuration < minOffDuration)
            {
                maxOffDuration = minOffDuration;
            }

            if (maxOnDuration < minOnDuration)
            {
                maxOnDuration = minOnDuration;
            }
        }
    }
}
