using System.Collections;
using System.Collections.Generic;
using UnityEngine;using UnityEngine.Timeline;

public static class Debugger
{
    public static bool useLog = true;

    static ILogger logger = new UnityLog();
    public static void Log(object message)
    {
        if (useLog)
        {
            logger.Log(message);
        }
    }
    public static void LogError(object message)
    {
        if (useLog)
        {
            logger.LogError(message);
        }
    }
}

public interface ILogger
{
    public void Log(object message);
    public void LogError(object mesasge);
}

public class UnityLog : ILogger
{
    public void Log(object message)
    {
        Debug.Log(message);
    }

    public void LogError(object message)
    {
        Debug.LogError(message);
    }
}
