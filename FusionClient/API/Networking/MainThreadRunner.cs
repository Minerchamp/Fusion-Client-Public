namespace FusionClient;

using FusionClient.Core;

#region Usings

using System;
using System.Collections.Generic;
#endregion

public static class MainThreadRunner
{
    private static readonly List<Action> _actions = new();

    public static void Enqueue(Action action)
    {
        _actions.Add(action);
    }

    public static void Update()
    {
        if (_actions.Count > 0)
        {
            lock (_actions)
            {
                for (int i = 0; i < _actions.Count; i++)
                {
                    try
                    {
                        _actions[i]();
                    } catch (Exception e)
                    {
                        Logs.Error("MainThreadRunner", e);
                    }
                }
                _actions.Clear();
            }
        }
    }
}
