using UnityEngine;
using System.Collections;
using System;

public class Timer : GameSingleton<Timer>
{
    public void Request(float time, Action onComplete)
    {
        StartCoroutine("TimerCoroutine", new object[] { time, onComplete });
    }

    private IEnumerator TimerCoroutine(object args)
    {
        object[] argsArray = args as object[];
        float time = (float)argsArray[0];
        Action onComplete = argsArray[1] as Action;

        yield return new WaitForSeconds(time);

        if (onComplete != null)
            onComplete();

        yield return null;
    }
}