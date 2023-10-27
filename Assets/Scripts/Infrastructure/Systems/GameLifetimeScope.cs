using VContainer;
using VContainer.Unity;
using MessagePipe;
using MonsterFactory.Services;
using MonsterFactory.Services.SceneManagement;
using MonsterFactory.Events;
using UnityEngine;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField]private SceneTransitionController _sceneTransitionController;
 
    protected override void Configure(IContainerBuilder builder)
    {
        SetupGlobalMessageBrokers(builder);
        SetupServices(builder);
    }

    private void SetupServices(IContainerBuilder builder)
    {
        builder.RegisterInstance(_sceneTransitionController).AsImplementedInterfaces();
        builder.Register<SceneNavigationManager>(Lifetime.Singleton).AsImplementedInterfaces();
    }
    

    private void SetupGlobalMessageBrokers(IContainerBuilder builder)
    {
        MessagePipeOptions options = builder.RegisterMessagePipe();
        builder.RegisterBuildCallback(c => GlobalMessagePipe.SetProvider(c.AsServiceProvider()));
        
        EventRegistrationHelper.RegisterEventClasses(builder,options);
    }
    
}