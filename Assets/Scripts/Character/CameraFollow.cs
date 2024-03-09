using LevelGeneration.LevelsGenerators;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Character
{
	public class CameraFollow : MonoBehaviour
	{
		public static CameraFollow Instance { private set; get; }

		[SerializeField] private LevelGeneratorBase _levelGenerator;
		[SerializeField] private Vector3 positionOffset;

		public Transform Target;
		public float FollowSpeed = 2f;
		public float shakeDuration = 0f;
		public float shakeAmount = 0.1f;
		public float decreaseFactor = 1.0f;

		private Transform _camTransform;
		private Vector3 _originalPos;

		void Awake()
		{
			if (Instance)
			{
				Destroy(gameObject);
				return;
			}
			Instance = this;

			Cursor.visible = false;
			if (_camTransform == null)
				_camTransform = GetComponent(typeof(Transform)) as Transform;

			_levelGenerator.OnPlayerSpawned += Init;
		}

		private void Init(Player player)
		{
			Target = player.CharacterCenter;
			_camTransform.position = Target.position;
		}
		
		void OnEnable()
		{
			_originalPos = _camTransform.localPosition;
		}

		private void Update()
		{
			if(Target == null) return;
			
			transform.position = Vector3.Slerp(transform.position, Target.position + positionOffset, FollowSpeed * Time.deltaTime);

			if (shakeDuration > 0)
			{
				_camTransform.localPosition = _originalPos + Random.insideUnitSphere * shakeAmount;

				shakeDuration -= Time.deltaTime * decreaseFactor;
			}
		}

		public void ShakeCamera()
		{
			_originalPos = _camTransform.localPosition;
			shakeDuration = 0.2f;
		}
	}
}