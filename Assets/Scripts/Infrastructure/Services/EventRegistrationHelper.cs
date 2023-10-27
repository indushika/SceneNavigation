using MessagePipe;
using VContainer;

namespace MonsterFactory.Events
{
    public static partial class EventRegistrationHelper
    {
        private static IContainerBuilder builder;
        private static MessagePipeOptions options;
        
        private static void RegisterEvent<TtypedEvent>() where TtypedEvent : MFBaseEvent
        {
            builder.RegisterMessageBroker<TtypedEvent>(options);
        }
    }

}