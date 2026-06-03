using UnityEngine;

namespace EscapeFromNightmare
{
		// 유니티 컴포넌트용 싱글톤 기반 클래스입니다.
	// 같은 타입의 인스턴스가 여러 개 생기면 첫 번째 인스턴스만 유지합니다.
	public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		// 현재 활성화된 싱글톤 인스턴스를 저장합니다.
		private static T instance;

		// 외부 코드에서 싱글톤 인스턴스에 접근할 때 사용하는 진입점입니다.
		public static T Instance
		{
			get
			{
				// 캐시가 비어 있으면 현재 씬에서 해당 컴포넌트를 한 번 찾아옵니다.
				if (instance == null) instance = FindFirstObjectByType<T>();

				return instance;
			}
		}

		// 파생 클래스에서 인스턴스 존재 여부를 확인할 때 사용할 수 있는 보조 속성입니다.
		private bool HasInstance => instance != null;

		// 참이면 씬 전환 후에도 유지하고, 거짓이면 현재 씬 오브젝트처럼 제거됩니다.
		protected virtual bool DontDestroy => true;

		// 인스턴스를 등록하고 중복 오브젝트를 제거합니다.
		protected virtual void Awake()
		{
			// 이미 다른 인스턴스가 등록되어 있으면 현재 오브젝트를 중복으로 판단합니다.
			if (instance != null && instance != this)
			{
				Destroy(gameObject);
				return;
			}

			// 현재 컴포넌트를 싱글톤 인스턴스로 등록합니다.
			instance = this as T;

			// 전역 매니저라면 씬 전환 시에도 유지합니다.
			if (DontDestroy) DontDestroyOnLoad(gameObject);
		}

		// 등록된 인스턴스가 제거될 때 정적 참조를 비웁니다.
		protected virtual void OnDestroy()
		{
			if (instance == this) instance = null;
		}
	}
}
