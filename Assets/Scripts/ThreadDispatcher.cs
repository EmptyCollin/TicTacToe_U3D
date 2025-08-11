using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;


public class ThreadDispatcher : MonoBehaviour
{
    private static readonly ConcurrentQueue<Action> _actions = new ConcurrentQueue<Action>();
    private static ThreadDispatcher _instance;

    void Awake() => _instance = this;

    public static void Enqueue(Action action) => _actions.Enqueue(action);

    void Update()
    {
        while (_actions.TryDequeue(out var action))
        {
            action?.Invoke();
        }
    }
}

