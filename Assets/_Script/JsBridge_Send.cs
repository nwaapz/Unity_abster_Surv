using OctoberStudio;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class JsBridge_Send : Singleton<JsBridge_Send>
{


    // --- Your function that calls JS ---

    public void StartTest()
    {
        int testNumber = 5;
        #if UNITY_WEBGL && !UNITY_EDITOR
                                // Ask the bridge to relay this message to React
                                print("Sending to React and this message from inside unity: AddTwelve " + testNumber);
                                Application.ExternalEval("window.handleMessageFromUnity('AddTwelve', '5');");
        #endif
    }

    public void RequestStartSession()
    {
         #if UNITY_WEBGL && !UNITY_EDITOR
                // Ask the bridge to relay this message to React
                print("Sending to React and this message from inside unity: isWalletConnected");
                Application.ExternalEval("window.handleMessageFromUnity('RequestStartSession', '');");

         #endif
    }

    public void SendReplayData()
    {
        if (!PaymentSystem.Instance.WagerGamer || !PaymentSystem.Instance.HasPaid) return;
        int time = (int)Time.time;   
        long unixMillis = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var replayEvent = new ReplayEvent(time.ToString(),(int)ExperienceManager.Instance.Gem);

        PaymentSystem.Instance.AddRePlayEvent(replayEvent);
        string data = JsonUtility.ToJson(PaymentSystem.Instance.replaySubmit);
        print(data);
        if (PaymentSystem.Instance.replaySubmit != null && PaymentSystem.Instance.WagerGamer)
        {
            

#if UNITY_WEBGL && !UNITY_EDITOR
    string escapedData = data.Replace("\\", "\\\\").Replace("\"", "\\\"");
    string js = $"window.handleMessageFromUnity('SubmitReplay', \"{escapedData}\");";
    Application.ExternalEval(js);
#endif
        }
        else
        {
            print("submit replay null");
        }
    }

    public void AskAuthState()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
                // Ask the bridge to relay this message to React
                print("Sending to React and this message from inside unity: isWalletConnected");
                Application.ExternalEval("window.handleMessageFromUnity('isWalletConnected', '');"); 
#endif
    }

    public void TryConnectToPrivyWallet()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
                // Ask the bridge to relay this message to React
                print("Sending to React and this message from inside unity: connectToPrivyWallet");
                Application.ExternalEval("window.handleMessageFromUnity('tryconnect', '');");
#endif
    }

    public void GetProfileData()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
                            // Ask the bridge to relay this message to React
                            print("Sending to React and this message from inside unity: getProfileData");
                            Application.ExternalEval("window.handleMessageFromUnity('RequestProfile', '');");
#endif
    }


    public void SetNewProfileName(string newName)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
                // Ask the bridge to relay this message to React
                print("Sending to React and this message from inside unity: SetNewProfileName " + newName);
                Application.ExternalEval("window.handleMessageFromUnity('SetNewProfileName', '" + newName + "');");    
    
#endif
    }

    public void CheckPaimentStatus()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
                // Ask the bridge to relay this message to React
                print("Sending to React and this message from inside unity: CheckPaymentStatus ");
                Application.ExternalEval("window.handleMessageFromUnity('CheckPaymentStatus', '');");    
    
#endif
    }
    public void TryPayForGame()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
                        // Ask the bridge to relay this message to React
                        print("Sending to React and this message from inside unity: CheckPaymentStatus ");
                        Application.ExternalEval("window.handleMessageFromUnity('TryPayForGame', '');");    
    
#endif

    }

    public void SetScore(string score)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
    print($"Sending to React: SubmitScore {score}");
    Application.ExternalEval($"window.handleMessageFromUnity('SubmitScore', \"{score}\");");
#endif
    }

    public void RequestLeaderBoard()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
    print($"Sending to React: request leaderboard");
    Application.ExternalEval($"window.handleMessageFromUnity('RequestLeaderBoard','');");
#endif
    }

    public void RequestGameState()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
    print($"Sending to React: request leaderboard");
    Application.ExternalEval($"window.handleMessageFromUnity('RequestGameState','');");
#endif
    }

    public void RequestBalance()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        print($"Sending to React: RequestBalance");
        Application.ExternalEval($"window.handleMessageFromUnity('RequestBalance','');");
        #endif
    }

}

