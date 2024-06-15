using System;
using System.Collections;
using UnityEngine;

namespace GameCode.UI
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeTime = 1;
        
        public static LoadingScreen Instance;

        public bool IsShow { get; private set; }
        
        public event Action FadeAnimationEnded;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            IsShow = gameObject.activeSelf;
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void StartLoading()
        {
            StopAllCoroutines();
            canvasGroup.alpha = 1;
            gameObject.SetActive(true);
            IsShow = true;
        }

        public void EndLoading()
        {
            StopAllCoroutines();
            StartCoroutine(Fade());
        }

        public void EndLoadingInstantly()
        {
            StopAllCoroutines();
            IsShow = false;
            gameObject.SetActive(false);
            FadeAnimationEnded?.Invoke();
        }
        
        IEnumerator Fade()
        {
            float timer = 0;

            while (timer < fadeTime)
            {
                yield return new WaitForEndOfFrame();
                canvasGroup.alpha = fadeTime - timer;
                timer += Time.deltaTime;
            }
            
            IsShow = false;
            gameObject.SetActive(false);
            FadeAnimationEnded?.Invoke();
        }
    }
}