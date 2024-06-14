using System;
using UnityEngine;

namespace SourceCode.Ui.Elements
{
    public class LoadingRotator : MonoBehaviour
    {
        [SerializeField] private RotatePair[] rotatePairs;
        
        private void Update()
        {
            var deltaTime = Time.deltaTime;
            foreach (var rotatePair in rotatePairs)
                rotatePair.ManualUpdate(deltaTime);
        }

        [Serializable]
        private class RotatePair
        {
            [SerializeField] private Transform transform;
            [SerializeField] private float rotationSpeed;

            public void ManualUpdate(float deltaTime)
            {
                transform.Rotate(Vector3.back * (rotationSpeed * deltaTime));
            }
        }
    }
}