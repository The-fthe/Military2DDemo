using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Event")]
public class GameEvent : ScriptableObject
{
    static HashSet<GameEvent> _listenedEvents = new HashSet<GameEvent>();
    
    HashSet<GameEventListener> _gameEventListeners = new HashSet<GameEventListener>();

    public void Register(GameEventListener gameEventListener)
    {
        _gameEventListeners.Add(gameEventListener);
        _listenedEvents.Add(this);
    }

    public void Deregister(GameEventListener gameEventListener)
    {
        _gameEventListeners.Remove(gameEventListener);
        if(_gameEventListeners.Count== 0)
            _listenedEvents.Remove(this);
    }

    [ContextMenu("Invoke")]
    public void Invoke()
    {
        foreach (var gameEventListener in _gameEventListeners)
        {
            gameEventListener.RaiseEvent();
        }
    }

    public static void RaiseEvent(string eventName)
    {
        foreach (var gameEvent in _listenedEvents)
        {
            if(gameEvent.name == eventName)
              gameEvent.Invoke();
        }
    }
}