using UnityEngine;

public class ScreenSleepOptions : MonoBehaviour
{
    private void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
}