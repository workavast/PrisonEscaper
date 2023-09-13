using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Character
{

	public class CameraFollow : MonoBehaviour
	{
		[SerializeField] private Vector3 positionOffset;

		public float FollowSpeed = 2f;
		public Transform Target;
		
		public float shakeDuration = 0f;
		public float shakeAmount = 0.1f;
		public float decreaseFactor = 1.0f;

		private Transform _camTransform;

		Vector3 originalPos;

		void Awake()
		{
			Cursor.visible = false;
			if (_camTransform == null)
			{
				_camTransform = GetComponent(typeof(Transform)) as Transform;
			}

		}

		private void Start()
		{
			if(Target == null)
				Target = Player.Instance.CharacterCenter;

			_camTransform.position = Target.position;
		}

		void OnEnable()
		{
			originalPos = _camTransform.localPosition;
		}

		private void Update()
		{
			if(Target == null) return;
			
			transform.position = Vector3.Slerp(transform.position, Target.position + positionOffset, FollowSpeed * Time.deltaTime);

			if (shakeDuration > 0)
			{
				_camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

				shakeDuration -= Time.deltaTime * decreaseFactor;
			}
		}

		public void ShakeCamera()
		{
			originalPos = _camTransform.localPosition;
			shakeDuration = 0.2f;
		}
	}
}