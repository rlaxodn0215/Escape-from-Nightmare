using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
	public class UITitleWindow : MonoBehaviour
	{
		[SerializeField] private Button startButton;
		[SerializeField] private Button settingButton;
		[SerializeField] private Button quitButton;
		[SerializeField] private UIAudioSettingsView settingsView;

		private GameManager gameManager = null;

		private void Awake()
		{
			CacheReferences();
		}

		private void OnEnable()
		{
			RegisterEvents();
		}

		private void OnDisable()
		{
			UnregisterEvents();
		}

		private void CacheReferences()
		{
			startButton = startButton != null ? startButton : FindChildButton("StartButton");
			settingButton = settingButton != null ? settingButton : FindChildButton("SettingButton");
			quitButton = quitButton != null ? quitButton : FindChildButton("QuitButton");

			if (gameManager == null)
			{
				gameManager = GameManager.Instance;
			}

			if (gameManager == null)
			{
				gameManager = FindFirstObjectByType<GameManager>();
			}

			if (settingsView == null)
			{
				settingsView = FindSceneObject<UIAudioSettingsView>();
			}
		}

		private void RegisterEvents()
		{
			if (startButton != null)
			{
				startButton.onClick.AddListener(HandleStartButtonClicked);
			}
			else
			{
				Debug.LogWarning("UITitleWindow could not find StartButton.", this);
			}

			if (settingButton != null)
			{
				settingButton.onClick.AddListener(HandleSettingButtonClicked);
			}
			else
			{
				Debug.LogWarning("UITitleWindow could not find SettingButton.", this);
			}

			if (quitButton != null)
			{
				quitButton.onClick.AddListener(HandleQuitButtonClicked);
			}
			else
			{
				Debug.LogWarning("UITitleWindow could not find QuitButton.", this);
			}
		}

		private void UnregisterEvents()
		{
			if (startButton != null)
			{
				startButton.onClick.RemoveListener(HandleStartButtonClicked);
			}

			if (settingButton != null)
			{
				settingButton.onClick.RemoveListener(HandleSettingButtonClicked);
			}

			if (quitButton != null)
			{
				quitButton.onClick.RemoveListener(HandleQuitButtonClicked);
			}
		}

		private void HandleStartButtonClicked()
		{
			CacheGameManagerIfNeeded();
			if (gameManager == null)
			{
				Debug.LogWarning("UITitleWindow could not find GameManager for StartButton.", this);
				return;
			}

			gameManager.StartGame();
		}

		private void HandleSettingButtonClicked()
		{
			if (settingsView != null)
			{
				settingsView.Open();
				return;
			}

			Debug.LogWarning("UITitleWindow could not find SettingsView object.", this);
		}

		private void HandleQuitButtonClicked()
		{
			CacheGameManagerIfNeeded();
			if (gameManager == null)
			{
				Debug.LogWarning("UITitleWindow could not find GameManager for QuitButton.", this);
				return;
			}

			gameManager.QuitGame();
		}

		private void CacheGameManagerIfNeeded()
		{
			if (gameManager != null)
			{
				return;
			}

			gameManager = GameManager.Instance;
			if (gameManager == null)
			{
				gameManager = FindFirstObjectByType<GameManager>();
			}
		}

		private Button FindChildButton(string objectName)
		{
			Transform child = FindChildTransform(objectName);
			return child != null ? child.GetComponent<Button>() : null;
		}

		private Transform FindChildTransform(string objectName)
		{
			Transform[] children = GetComponentsInChildren<Transform>(true);
			foreach (Transform child in children)
			{
				if (child.name == objectName)
				{
					return child;
				}
			}

			return null;
		}

		private static T FindSceneObject<T>() where T : Component
		{
			T[] objects = Resources.FindObjectsOfTypeAll<T>();
			foreach (T sceneObject in objects)
			{
				if (sceneObject.gameObject.scene.IsValid() && sceneObject.gameObject.scene.isLoaded)
				{
					return sceneObject;
				}
			}

			return null;
		}
	}
}
