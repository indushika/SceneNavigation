using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MessagePipe;
using MonsterFactory.Events;
using UnityEngine.UI;
using VContainer;

namespace MonsterFactory.Services.SceneManagement
{
    public interface ISceneTransitionController
    {
    }

    public class SceneTransitionController : MonoBehaviour, ISceneTransitionController
    {
        [SerializeField] private Image fadeImage;

        [Inject] private ISubscriber<BeginSceneTransitionEvent> beginSceneTransitionEventSubscriber;
        private IDisposable disposable; 
        
        private bool isNavigating;
        
        private void Start()
        {
            EnableFadeImage(false);
            var dispose = beginSceneTransitionEventSubscriber.Subscribe(TransitionTask);
            disposable = DisposableBag.Create(dispose); 
        }
        
        #region Fade Image Accessors

        private void EnableFadeImage(bool isEnable)
        {
            if (fadeImage)
            {
                fadeImage.gameObject.SetActive(isEnable);
            }
        }
        #endregion


        #region Implementation
 
        private void TransitionTask(BeginSceneTransitionEvent beginSceneTransitionEvent)
        {
            if (isNavigating)
            {
                UniTask.WaitUntil(() => isNavigating == false);
            }

            isNavigating = true;
            
            switch (beginSceneTransitionEvent.SceneTransitionType)      
            {
                case SceneTransitionType.Default:
                    break;
                
                case SceneTransitionType.FadeIn:
                    var beginFadeInTaskTask = SceneTransitionUtility.CreateFadeInTask(fadeImage);
                    UniTask endFadeInTask = UniTask.Create(async () =>
                    {
                        EnableFadeImage(false);
                    });
                    SceneTransitionUtility.TransitionTask(beginFadeInTaskTask, beginSceneTransitionEvent.TasksToWaitOn, endFadeInTask);
                    break;
                
                case SceneTransitionType.FadeOut:
                    var beginFadeOutTask = SceneTransitionUtility.CreateFadeOutTask(fadeImage);
                    UniTask endFadeOutTask = UniTask.Create(async () =>
                    {
                        EnableFadeImage(false);
                    });
                    SceneTransitionUtility.TransitionTask(beginFadeOutTask, beginSceneTransitionEvent.TasksToWaitOn, endFadeOutTask);
                    break;
                
                case SceneTransitionType.FadeInOut:
                    var beginFadeInOutTask = SceneTransitionUtility.CreateFadeInTask(fadeImage);
                    var endFadeInOutTask = SceneTransitionUtility.CreateFadeOutTask(fadeImage);
                    SceneTransitionUtility.TransitionTask(beginFadeInOutTask, beginSceneTransitionEvent.TasksToWaitOn,
                        endFadeInOutTask);
                    break;
                
                case SceneTransitionType.SpriteAnimation:
                    var beginSpriteAnimationTask = SceneTransitionUtility.CreateBeginSpriteAnimationTask();
                    var endSpriteAnimationTask = SceneTransitionUtility.CreateEndSpriteAnimationTask();
                    SceneTransitionUtility.TransitionTask(beginSpriteAnimationTask,
                        beginSceneTransitionEvent.TasksToWaitOn, endSpriteAnimationTask);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }

            isNavigating = false; 
        }

        #endregion
    }
}