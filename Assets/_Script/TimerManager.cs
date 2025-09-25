using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(1001)]
public class TimerManager : MonoBehaviour
{


    private void Start()
    {
        print("TimerManager started, requesting game state in 2 seconds...");
        JsBridge_Send.Instance.RequestGameState();
    }

    void Update()
    {
# if UNITY_EDITOR
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            var jsBridge = FindObjectOfType<JSBridge>();
            Debug.Log("==== JSBridge Test Started ====");

            // 1. Auth state
            jsBridge.OnAuthChanged("{\"authenticated\":true, \"address\":\"0x123abc\"}");

            // 2. Time left
            jsBridge.OnTimeLeftChangedFromJS("{\"timeLeft\":90061000}"); // 1 day, 1 hour, 1 minute, 1 second in ms

            // 3. Wallet connection
            jsBridge.OnWalletConnectionStatusFromJS("no");
            jsBridge.OnWalletConnectionStatusFromJS("0xABCDEF123456");

            // 4. Profile result
            jsBridge.OnProfileResult("{\"ok\":true,\"found\":true,\"profile\":\"TestUser\",\"address\":\"0x123\"}");

            // 5. Payment status
            jsBridge.OnPaymentStatus("{\"paid\":true,\"address\":\"0x123\",\"loading\":false,\"error\":null}");

            // 6. Payment result
            jsBridge.OnPaymentResult("{\"ok\":true,\"status\":\"success\",\"message\":\"Tx confirmed\"}");

            // 7. Submit score
            jsBridge.OnSubmitScore("{\"ok\":true,\"status\":\"\",\"message\":\"score submitted\"}");

            // 8. Balance
            jsBridge.OnBalance("{\"ok\":true,\"balance\":\"0.1234\",\"address\":\"0x123\"}");

            // 9. Leaderboard
            string lbJson = "{\"ok\":true,\"count\":2,\"leaderboard\":[{\"user_address\":\"0x1\",\"score\":100},{\"user_address\":\"0x2\",\"score\":90}],\"player\":{\"user_address\":\"0x1\",\"score\":100}}";
            jsBridge.ONLB(lbJson);

            // 10. Period update
            jsBridge.OnPeriodUpdate("{\"ok\":true,\"periodIndex\":1,\"status\":\"active\",\"remainingHuman\":\"1d 2h 3m\"}");

            Debug.Log("==== JSBridge Test Finished ====");



        }
#endif
    }
}
