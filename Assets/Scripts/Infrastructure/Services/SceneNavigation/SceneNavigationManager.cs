using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MessagePipe;
using MonsterFactory.Events;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace MonsterFactory.Services.SceneManagement
{
    public interface ISceneNavigationManager
    {
        public UniTask LoadScene(SceneName sceneToNavigate, SceneTransitionType sceneTransitionType,
            bool pushCurrentSceneToStack = true);

        public UniTask UnloadScene(SceneTransitionType sceneTransitionType, bool goToRootScene = false);
    }

    public class SceneNavigationManager : MFService, ISceneNavigationManager
    {
        // private readonly ISceneTransitionController sceneTransitionController;
        private readonly LifetimeScope parentLifetimeScope;

        #region Publishers
        private IPublisher<SceneLoadEvent> sceneLoadEventPublisher; 
        private IPublisher<SceneUnloadEvent> sceneUnloadEventPublisher;
        private IPublisher<BeginSceneTransitionEvent> beginSceneTransitionEventPublisher; 
        #endregion
        
        #region Subscribers
        private ISubscriber<SceneLoadPrerequisitesEvent> sceneLoadPrerequisiteEventSubscriber;
        private ISubscriber<SceneUnloadPrerequisiteEvent> sceneUnloadPrerequisiteEventSubscriber;
        private ISubscriber<SceneAssetLoadingEvent> sceneAssetLoadingEventSubscriber;
        #endregion
        
        private IDisposable disposable;
        private DisposableBagBuilder disposableBag; 
        
        private Stack<SceneName> sceneStack = new Stack<SceneName>();
        private SceneName? activeScene = null;

        [Inject]
        public SceneNavigationManager(ISceneTransitionController sceneTransitionController, LifetimeScope parentLifetimeScope, 
            ISubscriber<SceneLoadPrerequisitesEvent> sceneLoadPrerequisiteEventSubscriber, 
               ISubscriber<SceneUnloadPrerequisiteEvent> sceneUnloadPrerequisiteEventSubscriber,
            ISubscriber<SceneAssetLoadingEvent> sceneAssetLoadingEventSubscriber, 
            IPublisher<SceneLoadEvent> sceneLoadEventPublisher,
            IPublisher<SceneUnloadEvent> sceneUnloadEventPublisher,
         IPublisher<BeginSceneTransitionEvent> beginSceneTransitionEventPublisher)
        {
            // this.sceneTransitionController = sceneTransitionController;
            this.parentLifetimeScope = parentLifetimeScope;
            this.sceneLoadPrerequisiteEventSubscriber = sceneLoadPrerequisiteEventSubscriber;
            this.sceneUnloadPrerequisiteEventSubscriber = sceneUnloadPrerequisiteEventSubscriber;
            this.sceneAssetLoadingEventSubscriber = sceneAssetLoadingEventSubscriber;
            this.sceneLoadEventPublisher = sceneLoadEventPublisher;
            this.sceneUnloadEventPublisher = sceneUnloadEventPublisher;
            this.beginSceneTransitionEventPublisher = beginSceneTransitionEventPublisher; 
            
            disposableBag = DisposableBag.CreateBuilder();
            disposable = disposableBag.Build();

        }

        #region API

        public async UniTask LoadScene(SceneName sceneToNavigate, SceneTransitionType sceneTransitionType, bool pushCurrentSceneToStack = true)
        {
            var beginSceneTransitionEvent =
                new BeginSceneTransitionEvent(sceneTransitionType, CreateTasksToWaitOnLoadingScene(sceneToNavigate));
            beginSceneTransitionEventPublisher.Publish(beginSceneTransitionEvent);
            
            if (pushCurrentSceneToStack)
            {
                sceneStack.Push(sceneToNavigate);
            }
            activeScene = sceneToNavigate;

        }

        public async UniTask UnloadScene(SceneTransitionType sceneTransitionType, bool goToRootScene = false)
        {
            if (sceneStack.TryPeek(out SceneName lastScene))
            {
                if (lastScene == activeScene)
                {
                    sceneStack.Pop();
                }
            }
            
            var beginSceneTransitionEvent = new BeginSceneTransitionEvent(sceneTransitionType,
                CreateTasksToWaitOnUnloadingScene(goToRootScene));
            beginSceneTransitionEventPublisher.Publish(beginSceneTransitionEvent);

        }

        #endregion


        #region Implementation

        private UniTask CreateLoadSceneTask(SceneName sceneName)
        {
            UniTask loadSceneTask = SceneManager.LoadSceneAsync((int)sceneName, LoadSceneMode.Additive).ToUniTask();
            return loadSceneTask;
        }

        private UniTask CreateUnloadActiveSceneTask()
        {
            if (activeScene != null)
            {
                UniTask unloadSceneTask = SceneManager.UnloadSceneAsync((int)activeScene.Value).ToUniTask();
                sceneUnloadEventPublisher.Publish(new SceneUnloadEvent(activeScene.Value, unloadSceneTask));

                activeScene = null; 
                
                return unloadSceneTask;
            }

            return new UniTask();
        }

        private async UniTask CreateTasksToWaitOnLoadingScene(SceneName sceneName)
        {
            sceneUnloadPrerequisiteEventSubscriber.Subscribe(async (unloadPrereqEvent) =>
            {
                await unloadPrereqEvent.EventTask;
            }).AddTo(disposableBag);
            await CreateUnloadActiveSceneTask();

            sceneLoadPrerequisiteEventSubscriber.Subscribe(async (loadPrereqEvent) =>
            {
                await loadPrereqEvent.EventTask;
            }).AddTo(disposableBag);
            await CreateLoadSceneTask(sceneName);
            
            sceneAssetLoadingEventSubscriber.Subscribe(async (assetLoadingEvent) =>
            {
                await assetLoadingEvent.AssetLoadingTask;
            }).AddTo(disposableBag);

            disposable = disposableBag.Build(); 
        }

        private async UniTask CreateTasksToWaitOnUnloadingScene(bool goToRoot = false)
        {
            sceneUnloadPrerequisiteEventSubscriber.Subscribe(async (unloadPrereqEvent) =>
            {
                await unloadPrereqEvent.EventTask; 
            }).AddTo(disposableBag);
            await CreateUnloadActiveSceneTask();

            if (!goToRoot)
            {
                if (sceneStack.TryPeek(out SceneName lastActiveScene))
                {
                    sceneLoadPrerequisiteEventSubscriber.Subscribe(async (loadPrereqEvent) =>
                    {
                        await loadPrereqEvent.EventTask; 
                    }).AddTo(disposableBag);
                    await CreateLoadSceneTask(lastActiveScene);
                    
                    activeScene = lastActiveScene;
                }
            }
            
            sceneAssetLoadingEventSubscriber.Subscribe(async (assetLoadingEvent) =>
            {
                await assetLoadingEvent.AssetLoadingTask; 
            }).AddTo(disposableBag);

            disposable = disposableBag.Build(); 
        }
        #endregion
    }



}