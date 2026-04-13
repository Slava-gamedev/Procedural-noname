using System;
using System.Collections.Generic;

public interface IEventBus
{
    void Subscribe<TEventArgs>(Action<TEventArgs> action) where TEventArgs : IGameEventArgs;
    void Unsubscribe<TEventArgs>(Action<TEventArgs> action) where TEventArgs : IGameEventArgs;
    void InvokeEvent<TEventArgs>(TEventArgs eventArguments) where TEventArgs : IGameEventArgs;
}

public class EventBus : IEventBus
{
    private readonly Dictionary<Type, Delegate> _eventsDictionary = new Dictionary<Type, Delegate>();

    public void Subscribe<TEventArgs>(Action<TEventArgs> action) where TEventArgs : IGameEventArgs
    {
        Type type = typeof(TEventArgs);

        if (!_eventsDictionary.TryAdd(type, action))
        {
            _eventsDictionary[type] = Delegate.Combine(_eventsDictionary[type], action);
        }
    }

    public void Unsubscribe<TEventArgs>(Action<TEventArgs> action) where TEventArgs : IGameEventArgs
    {
        Type type = typeof(TEventArgs);

        if (_eventsDictionary.ContainsKey(type))
        {
            Delegate currentDelegate = Delegate.Remove(_eventsDictionary[type], action);

            if(currentDelegate == null)
            {
                _eventsDictionary.Remove(type);
            }
            else
            {
                _eventsDictionary[type] = currentDelegate;
            }
        }
    }

    public void InvokeEvent<TEventArgs>(TEventArgs eventArguments) where TEventArgs : IGameEventArgs
    {
        Type type = typeof(TEventArgs);
        if(_eventsDictionary.TryGetValue(type, out Delegate delegateAction))
        {
            (delegateAction as Action<TEventArgs>)?.Invoke(eventArguments);
        }
    }
}
