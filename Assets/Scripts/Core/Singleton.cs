using UnityEngine;

namespace EscapeFromNightmare
{
	// Unity 컴포넌트용 제네릭 싱글톤 기반 클래스입니다.
	// T에는 실제 매니저 타입을 넣어 사용합니다. 예: GameManager : Singleton<GameManager>
	// 같은 타입의 인스턴스가 씬에 여러 개 생기면 첫 번째 인스턴스만 유지하고 나머지는 제거합니다.
	public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		// 현재 활성화된 싱글톤 인스턴스를 저장하는 정적 참조입니다.
		// 타입 T마다 별도의 정적 필드가 만들어지므로 GameManager, AudioManager 등은 서로 독립적으로 관리됩니다.
		private static T instance;

		// 외부 코드에서 싱글톤 인스턴스에 접근할 때 사용하는 프로퍼티입니다.
		// 아직 캐시된 인스턴스가 없다면 현재 씬에서 해당 타입의 컴포넌트를 찾아 저장합니다.
		public static T Instance
		{
			get
			{
				// Unity 6000 기준 API인 FindFirstObjectByType을 사용해 씬 안의 첫 인스턴스를 찾습니다.
				// 씬에 해당 컴포넌트가 없으면 null이 반환됩니다.
				if (instance == null) instance = FindFirstObjectByType<T>();

				return instance;
			}
		}

		// 이 클래스 내부에서 인스턴스가 이미 존재하는지 확인하기 위한 보조 프로퍼티입니다.
		// 현재 구현에서는 외부 공개가 필요 없으므로 private으로 둡니다.
		private bool HasInstance => instance != null;

		// 씬 전환 시 이 오브젝트를 파괴하지 않고 유지할지 결정합니다.
		// 기본값은 true이며, 파생 클래스에서 override하여 false로 바꾸면 씬 전환 시 일반 오브젝트처럼 사라집니다.
		protected virtual bool DontDestroy => true;

		// Unity 생명주기에서 오브젝트가 초기화될 때 호출됩니다.
		// 싱글톤 인스턴스를 등록하고, 중복 인스턴스가 생긴 경우 중복 오브젝트를 제거합니다.
		protected virtual void Awake()
		{
			// 이미 다른 인스턴스가 등록되어 있다면 현재 오브젝트는 중복이므로 제거합니다.
			// 이 처리로 씬 전환이나 프리팹 중복 배치 때문에 매니저가 두 번 동작하는 문제를 막습니다.
			if (instance != null && instance != this)
			{
				Destroy(gameObject);
				return;
			}

			// 현재 컴포넌트를 싱글톤 인스턴스로 등록합니다.
			// this는 MonoBehaviour이므로 T 타입으로 캐스팅해 저장합니다.
			instance = this as T;

			// 씬이 바뀌어도 유지해야 하는 매니저라면 Unity의 DontDestroyOnLoad 영역으로 이동시킵니다.
			if (DontDestroy) DontDestroyOnLoad(gameObject);
		}

		// Unity 생명주기에서 오브젝트가 파괴될 때 호출됩니다.
		// 현재 파괴되는 오브젝트가 등록된 싱글톤이라면 정적 참조를 비워 다음 인스턴스를 찾을 수 있게 합니다.
		protected virtual void OnDestroy()
		{
			if (instance == this) instance = null;
		}
	}
}
