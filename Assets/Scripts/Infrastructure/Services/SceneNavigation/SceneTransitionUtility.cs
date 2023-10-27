using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine;

namespace MonsterFactory.Services.SceneManagement
{
   
    public static class SceneTransitionUtility 
    {
        private static readonly SceneTransitionOptions _defaultInOption = new SceneTransitionOptions
        {
            fadeColor = default,
            transitionDuration = 1f,
        };

        private static readonly SceneTransitionOptions _defaultOutOption = new SceneTransitionOptions
        {
            fadeColor = default,
            transitionDuration = 1f,
        };
        
        public static async UniTask TransitionTask(UniTask beginTransitionTask, UniTask tasksToWaitOn,
            UniTask endTransitionTask)
        {
            await beginTransitionTask;
            await tasksToWaitOn;
            await endTransitionTask; 
        }
        
        public static UniTask TransitionFadeTask(Image fadeImage,Color fadeColor, float startValue, float endValue, float duration)
        {
            SetColorFadeImage(fadeColor, fadeImage);
            SetAlphaFadeImage(startValue, fadeImage);
            UniTask fadeImageTask = fadeImage?.DOFade(endValue, duration).Play().ToUniTask() ?? new UniTask();
            return fadeImageTask;
        }

        public static async UniTask CreateFadeInTask(Image fadeImage, bool keepFadeImageEnabled = true)
        {
            EnableFadeImage(true,fadeImage);
            UniTask fadeInImageTask = TransitionFadeTask(fadeImage,_defaultInOption.fadeColor, 0f, 1f, _defaultInOption.transitionDuration);
            await fadeInImageTask;
            EnableFadeImage(keepFadeImageEnabled,fadeImage);
        }

        public static async UniTask CreateFadeOutTask(Image fadeImage,bool keepFadeImageEnabled = true)
        {
            EnableFadeImage(true, fadeImage);
            UniTask fadeOutImageTask = TransitionFadeTask(fadeImage ,_defaultOutOption.fadeColor, 1f, 0f, _defaultOutOption.transitionDuration);
            await fadeOutImageTask;
            EnableFadeImage(keepFadeImageEnabled, fadeImage);
        }

        public static async UniTask CreateBeginSpriteAnimationTask()
        {
            
        }

        public static async UniTask CreateEndSpriteAnimationTask()
        {
            
        }


        #region Fade Image 
        private static void EnableFadeImage(bool isEnable, Image fadeImage)
        {
            if (fadeImage)
            {
                fadeImage.gameObject.SetActive(isEnable);
            }
        }

        private static void SetAlphaFadeImage(float alpha, Image fadeImage)
        {
            if (!fadeImage) return;
            var fadeImageColor = fadeImage.color;
            fadeImageColor.a = alpha;
            SetColorFadeImage(fadeImageColor, fadeImage);
        }

        private static void SetColorFadeImage(Color color, Image fadeImage)
        {
            if (fadeImage != null)
            {
                fadeImage.color = color;
            }
        }

        #endregion
    }
    
    public class SceneTransitionOptions
    {
        public Color fadeColor;
        public float transitionDuration;
    }
}

