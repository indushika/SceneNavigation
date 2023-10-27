# SceneNavigation
Scene Navigation Controller 

• API to load and unload scenes 

• Stack to keep track of the previously loaded scenes, so unloading will take you directly to the previously loaded scene 

• Multiple Scene Transition Options are avaialble; FadeIn, FadeOut, Sprite Animation, etc (you can always custom ones in the SceneTransitionUtility.cs class)

• This plugin requires you to install:

    - CySharp/MessagePipe for the event system: https://github.com/Cysharp/MessagePipe#unity

    - VContainer for dependency injection: https://github.com/hadashiA/VContainer

    - Unitask: https://github.com/Cysharp/UniTask 

    - DOTween: https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676 
    (However for DOTween support, after importing from the DOTWeen assets and define the scripting define symbol UNITASK_DOTWEEN_SUPPORT to enable it)

    - Newton JSON for Unity: Windows->Package Manager, once the Package Manager window opens, go to Add package from git URL, type com.unity.nuget.newtonsoft-json
