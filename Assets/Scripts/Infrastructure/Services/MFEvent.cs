using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MessagePipe;
using MonsterFactory.Services.SceneManagement;
using VContainer;

namespace MonsterFactory.Events
{
    public static partial class EventRegistrationHelper
    {
        public static void RegisterEventClasses(IContainerBuilder builder, MessagePipeOptions options)
        {
            EventRegistrationHelper.builder = builder;
            EventRegistrationHelper.options = options;
            
            //Register Event types here
            RegisterEvent<SceneEvent>();
            RegisterEvent<SceneLoadEvent>();
            RegisterEvent<SceneUnloadEvent>();
            RegisterEvent<SceneLoadPrerequisitesEvent>();
            RegisterEvent<SceneUnloadPrerequisiteEvent>();
            RegisterEvent<SceneAssetLoadingEvent>();
            RegisterEvent<BeginSceneTransitionEvent>();
            
            
            EventRegistrationHelper.builder = null;
            EventRegistrationHelper.options = null;
        }
    }
}

namespace MonsterFactory.Events
{

    public class MFBaseEvent
    {
    }

    #region  Scene Navigation
    public class SceneEvent : MFBaseEvent
    {
        private readonly SceneName sceneName;
        public SceneName SceneName => sceneName; 
        public SceneEvent(SceneName sceneName)
        {
            this.sceneName = sceneName; 
        }
    }
    
    public class SceneLoadEvent : SceneEvent
    {
        private UniTask eventTask; 
        public UniTask EventTask => eventTask; 
        public SceneLoadEvent(SceneName sceneName, UniTask eventTask) : base(sceneName)
        {
            this.eventTask = eventTask; 
        }
    }

    public class SceneUnloadEvent : SceneEvent
    {
        private UniTask eventTask; 
        public UniTask EventTask => eventTask; 
        public SceneUnloadEvent(SceneName sceneName, UniTask eventTask) : base(sceneName)
        {
            this.eventTask = eventTask; 
        }
    }
    
    public class SceneLoadPrerequisitesEvent : SceneLoadEvent 
    {
        public SceneLoadPrerequisitesEvent(SceneName sceneName, UniTask eventTask) : base(sceneName, eventTask)
        {
        }
    }
    
    public class SceneUnloadPrerequisiteEvent : SceneUnloadEvent
    {
        public SceneUnloadPrerequisiteEvent(SceneName sceneName, UniTask eventTask) : base(sceneName, eventTask)
        {
        }
    }

    public class SceneAssetLoadingEvent : MFBaseEvent
    {
        private readonly UniTask assetLoadingTask;
        public UniTask AssetLoadingTask => assetLoadingTask; 
        public SceneAssetLoadingEvent(UniTask assetLoadingTask)
        {
            this.assetLoadingTask = assetLoadingTask; 
        }
    }

    public class BeginSceneTransitionEvent : MFBaseEvent
    {
        private readonly SceneTransitionType sceneTransitionType;
        private readonly UniTask tasksToWaitOn; 
        public SceneTransitionType SceneTransitionType => sceneTransitionType;
        public UniTask TasksToWaitOn => tasksToWaitOn; 
        public BeginSceneTransitionEvent(SceneTransitionType sceneTransitionType, UniTask tasksToWaitOn)
        {
            this.sceneTransitionType = sceneTransitionType;
            this.tasksToWaitOn = tasksToWaitOn; 
        }
    }
    #endregion


    
}

