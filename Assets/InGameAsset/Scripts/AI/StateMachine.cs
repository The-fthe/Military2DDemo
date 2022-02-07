using System;
using System.Collections.Generic;
using UnityEngine;

namespace InGameAsset.Scripts.AI
{
    public class StateMachine
    {
        IState _currentState;
        Dictionary<Type, List<Transition>> _transitions = new Dictionary<Type, List<Transition>>();
        List<Transition> _currentTransition = new List<Transition>();
        List<Transition> _anyTransition = new List<Transition>();
        static List<Transition> EmptyTransition = new List<Transition>(0);

        public IState CurrentState => _currentState;
        public void Tick()
        {
            var transition = GetTransition();
            if(transition != null)
                SetState(transition.To);
              // Debug.Log($"current state {_currentState.GetType().Name}");
            _currentState?.Tick();
        }

        Transition GetTransition()
        {
            foreach (var transition in _anyTransition)
            {
                if (transition.Condition()) return transition;
            }

            foreach (var transition in _currentTransition)
            {
                if (transition.Condition()) return transition;
            }
            return null;
        }

        public void ExitCurrentState()
        {
            _currentState?. Exit();
        }
        public void SetState(IState state)
        {
            if (state == _currentState) return;
            _currentState?.Exit();
            _currentState = state;
            _transitions.TryGetValue(_currentState.GetType(), out _currentTransition);
            if (_currentTransition == null) _currentTransition = EmptyTransition;
            _currentState.Enter();
        }

        public void AddTransition(IState from, IState to, Func<bool> predicate)
        {
            if (_transitions.TryGetValue(from.GetType(), out var transitions) == false)
            {
                transitions = new List<Transition>();
                _transitions[from.GetType()] = transitions;
            }
            transitions.Add(new Transition(to, predicate));
        }

        class Transition
        {
            public Func<bool> Condition { get; }
            public IState To { get; }
            public Transition(IState to, Func<bool> condition)
            {
                To = to;
                Condition = condition;
            }
        }

        public void AddAnyTransition(IState state, Func<bool> predicate)
        {
            _anyTransition.Add(new Transition(state,predicate));
        }
    }
}