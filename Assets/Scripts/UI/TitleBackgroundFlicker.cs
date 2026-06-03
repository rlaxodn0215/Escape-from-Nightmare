using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
	// 타이틀 배경 이미지의 알파 값을 불규칙하게 바꿔 깜빡이는 효과를 만듭니다.
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

		// 대상 이미지가 비어 있으면 같은 오브젝트의 이미지 컴포넌트를 사용합니다.
		private void Awake()
		{
			EnsureTargetImage();
		}

		// 원래 색상을 저장하고 다음 깜빡임 시간을 예약합니다.
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

		// 비활성화될 때 이미지 색상을 원래 상태로 되돌립니다.
		private void OnDisable()
		{
			if (targetImage != null && hasOriginalColor)
			{
				targetImage.color = originalColor;
			}
		}

		// 예약된 시간이 되면 켜짐과 꺼짐 상태를 다음 단계로 진행합니다.
		private void Update()
		{
			if (targetImage == null)
			{
				return;
			}

			if (Time.unscaledTime >= nextStateTime)
			{
				AdvanceState();
			}
		}

		// 깜빡임 횟수와 켜짐/꺼짐 시간을 갱신합니다.
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

		// 인스펙터에서 최소값보다 작은 최대값이 들어가지 않도록 보정합니다.
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
