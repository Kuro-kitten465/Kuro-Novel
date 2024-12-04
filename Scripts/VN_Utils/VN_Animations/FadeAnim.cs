using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KuroNovel.Utils
{
    public class FadeAnim : IVNAnimation
    {
        public float FadeDuration = 2f;

        private VNAnimationType m_AnimationType;
        private GameObject obj;

        public void ExecuteAnimation(VNAnimationType animationType, GameObject obj, Action onComplete = null)
        {
            m_AnimationType = animationType;
            this.obj = obj;

            if (VNAnimationType.FadeIn == m_AnimationType)
            {
                AnimationManager.Instance.StartCoroutine(Fade(0f, 1f, onComplete));
            }
            else
            {
                AnimationManager.Instance.StartCoroutine(Fade(1f, 0f, onComplete));
            }
        }

        private IEnumerator Fade(float startAlpha, float endAlpha, Action onComplete)
        {
            if (obj == null) yield break;

            var canvasGroup = obj.GetComponent<CanvasGroup>();
            var img = obj.GetComponent<Image>();
            float timeElapsed = 0f;

            if (canvasGroup != null) canvasGroup.alpha = startAlpha;
            if (img != null) img.color = new Color(1f, 1f, 1f, startAlpha);

            while (timeElapsed < FadeDuration)
            {
                float alpha = Mathf.Lerp(startAlpha, endAlpha, timeElapsed / FadeDuration);
                if (canvasGroup != null) canvasGroup.alpha = alpha;
                if (img != null) img.color = new Color(1f, 1f, 1f, alpha);

                timeElapsed += Time.deltaTime;
                yield return null;
            }

            // Ensure final alpha is set
            if (canvasGroup != null) canvasGroup.alpha = endAlpha;
            if (img != null) img.color = new Color(1f, 1f, 1f, endAlpha);

            onComplete?.Invoke();
        }
    }
}
