using MessagePipe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;

public class MessagePipeDemo : VContainer.Unity.IStartable
{
    readonly IPublisher<int> publisher;
    readonly ISubscriber<int> subscriber; 

    public MessagePipeDemo(IPublisher<int> publisher, ISubscriber<int> subscriber)
    {
        this.publisher = publisher;
        this.subscriber = subscriber; 
    }

    void IStartable.Start()
    {
        var d = DisposableBag.CreateBuilder();
        subscriber.Subscribe(x => Debug.Log("S1:" + x)).AddTo(d); 
        subscriber.Subscribe(x => Debug.Log("S2:" + x)).AddTo(d);

        publisher.Publish(10); 
        publisher.Publish(20); 
        publisher.Publish(30);

        var disposable = d.Build();
        disposable.Dispose(); 
    }

}
