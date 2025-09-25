using UnityEngine;

[DefaultExecutionOrder(1001)]
public class TimerManager : MonoBehaviour
{
    

    private void Start()
    {
         print("TimerManager started, requesting game state in 2 seconds...");
        JsBridge_Send.Instance.RequestGameState();
    }


}
