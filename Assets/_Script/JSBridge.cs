using Newtonsoft.Json;
using OctoberStudio.UI;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Scripting;



public class JSBridge : MonoBehaviour
{
    
    private void Start()
    {
        Debug.Log($"{gameObject.name} in Start, scene: {gameObject.scene.name}, active: {gameObject.activeSelf}");
        StartCoroutine(StartTest());
    }
    IEnumerator StartTest()
    { 
        yield return new WaitForSeconds(2);
        JsBridge_Send.Instance.AskAuthState();
        JsBridge_Send.Instance.RequestBalance();
       
        
    }
    // This method name must match what you send from React: "OnAuthChanged"


    [Preserve]
    public void OnTimeLeftChangedFromJS(string json)
    {
        Debug.Log("OnTimeLeftChangedFromJS raw: " + json);
        OnTimeLeftChanged(json);
    }

    [Preserve]
    public void OnWalletConnectionStatusFromJS(string status)
    {
        Debug.Log("OnWalletConnectionStatusFromJS raw: " + status);
        OnWalletConnectionStatus(status);
    }


    public void OnAuthChanged(string jsonPayload)
    {
        // Parse the JSON string 'jsonPayload' to get your data
        // Example: {"authenticated":true, "address":"0x..."}
        Debug.Log("Auth state received: " + jsonPayload);
    }

    // This method must match "OnTimeLeftChanged"
    public void OnTimeLeftChanged(string json)
    {
        TimeLeftData data = JsonUtility.FromJson<TimeLeftData>(json);
      //  Debug.Log("Time left in ms: " + data.timeLeft);

        long totalSeconds = data.timeLeft / 1000;
        int days = (int)(totalSeconds / 86400);
        int hours = (int)((totalSeconds % 86400) / 3600);
        int minutes = (int)((totalSeconds % 3600) / 60);
        int seconds = (int)(totalSeconds % 60);

       // Debug.Log($"Time Left: {days}d {hours}h {minutes}m {seconds}s");
        LobbyWindowBehavior.Instance.LeaderBoardCounter.text = $"{days}d {hours:D2}:{minutes:D2}:{seconds:D2}";
    }

    public void OnAddTwelveResult(string result)
    {
        Debug.Log("Result from AddTwelve back to unity: " + result);
    }
    // ... other methods for "OnPeriodEndChanged", etc.

    public void OnWalletConnectionStatus(string status)
    {
        Debug.Log("Wallet connection status: " + status);
        if (status == "no")
        {
            //GetComponent<Login>().TryConnectWallet();
            Login.Instance.SetFooterFeedback("Wallet not connected. Trying to connect...");
            Login.Instance.WalletConnectionFailed();

        }
        else
        {
            Login.Instance.walletAddress = status;
            Login.Instance.SetFooterFeedback(status);
            print("Wallet connected, now requesting profile data...");
            Login.Instance.GetProfileData();
            Login.Instance.setWalletState(true);
            Login.Instance.CheckPaymentStatus();
            JsBridge_Send.Instance.RequestBalance();
        }
    }

    public void OnProfileResult(string result)
    {
        Debug.Log("Profile result: " + result);
        ProfileResult profile = JsonConvert.DeserializeObject<ProfileResult>(result);

        if (profile != null)
        {
            if (profile.Ok)
            {
                if (profile.Found)
                {
                    // debugText.text = $"Profile found for {profile.Address}:\n{profile.Profile}";
                    Login.Instance.SetUserName(profile.Profile);
                    Login.Instance.setGameBanner("");
                }
                else
                {
                    //  debugText.text = $"No profile found for {profile.Address}.";
                    Login.Instance.ShowLogingPanel();
                    Login.Instance.SetUserName("Set Display Name");

                }
            }
            else
            {
                Login.Instance.SetFooterFeedback("Error retrieving profile.");
            }
        }
    }


    public void OnSetProfileResult(string result)
    {

        ProfileResult profile = JsonConvert.DeserializeObject<ProfileResult>(result);
        Debug.Log("Set profile result: " + profile.Profile);
        if (profile != null)
        {
            if (profile.Ok)
            {
                Login.Instance.SetUserName(profile.Profile);
            }
        }
        Login.Instance.SetLoginState(result, false);
        Login.Instance.HideLogingPanel();


    }

    public void OnPaymentStatus(string jsonResponse)
    {
        PaymentStatus status = JsonUtility.FromJson<PaymentStatus>(jsonResponse);
        print($"In unity recieved payment status: paid?{status.paid},adress: {status.address} error: {status.error}");
        if (status.loading)
        {
            // Still loading payment status

        }
        else if (!string.IsNullOrEmpty(status.error))
        {
            // Handle error
            Debug.LogError($"Payment check error: {status.error}");
            ShowPaymentUI();
        }
        else
        {
            if (status.paid)
            {
                PaymentSystem.Instance.PaimentAccomplished();
                // Player has paid for the current period
               EnableGameFeatures();
            }
            else
            {
                PaymentSystem.Instance.NotPayed();
                // Player needs to pay
                  ShowPaymentUI();
            }
        }
    }

    public void OnPaymentResult(string feedback)
    {
        TransactionStatus status = JsonUtility.FromJson<TransactionStatus>(feedback);

        JsBridge_Send.Instance.RequestBalance();

        print($"in unity recieving payment result{status.ok}");
        if (status.ok)
        {
            // GetComponent<Login>().HasPaid();
        }
        else
        {
            // GetComponent<Login>().NotPaid();
        }

    }



    private void ShowPaymentUI()
    {
        Login.Instance.NotPaid();
    }

    private void EnableGameFeatures()
    {
        Login.Instance.HasPaid();
    }

    public void OnSubmitScore(string feedback)
    {
        print("In unity recieved score submission feedback: " + feedback);
        TransactionStatus status = JsonUtility.FromJson<TransactionStatus>(feedback);
        if (status.ok)
        {
            ScoreManager.Instance.OnScoreSetConfirmation("Score submitted successfully!");
            ScoreManager.Instance.ShowScoreDisplay();
        }
        else
        {
            ScoreManager.Instance.OnScoreSetConfirmation("Failed to submit score: " + status.message);
        }
    }


    


    public void OnPeriodUpdate(string result)
    {
        print("In unity recieved period update: " + result);
        PeriodResponse period = JsonUtility.FromJson<PeriodResponse>(result);
        if (period != null && period.ok)
        {
            // Update your UI with period info
            Debug.Log($"Period {period.periodIndex}: {period.status}, ends in {period.remainingHuman}");
            
        }
        else
        {
            Debug.LogError("Failed to parse period update or period not ok.");
        }
    }

    public void OnPeriodEndChanged(string result)
    {
        print(result + " on period change result in unity");
    }

    public void OnBalance(string json)
    {
        Debug.Log("Balance response: " + json);
        var data = JsonUtility.FromJson<BalanceData>(json);
        if (data.ok)
        {
            Debug.Log($"Balance update: {data.balance} ETH (address: {data.address})");
            Login.Instance.SetUserBalance(data.balance);
        }
        else
        {
            Debug.LogWarning($"Balance fetch failed: {data.error}");
        }
        // json looks like: {"ok":true,"address":"0x..","balance":"0.1234"}
    }


    public void OnStartSession(string jsonPayload)
    {
        Debug.Log("Received start session from React: " + jsonPayload);

        // Parse JSON
        StartSessionData data = JsonUtility.FromJson<StartSessionData>(jsonPayload);

        // Save sessionId for later (e.g., replay submission)
        PlayerPrefs.SetString("sessionId", data.sessionId);
        PlayerPrefs.Save();

        print($"Session ID received: {data.sessionId}");
        // Optionally, you can use other session metadata if needed
    }



    
    public void ONLB(string json)
    {
        Debug.Log("ONLB received: " + json);

        try
        {
            LeaderBoardData data = JsonUtility.FromJson<LeaderBoardData>(json);

            if (data == null)
            {
                Debug.LogWarning("ONLB: parsed data null");
                return;
            }

            // Debug info to confirm what was parsed
            int lbCount = (data.leaderboard == null) ? 0 : data.leaderboard.Count;
            Debug.Log($"ONLB parsed: ok={data.ok}, count={data.count}, leaderboard.Length={lbCount}, player={(data.player != null ? data.player.user_address : "null")}");

            if (!data.ok)
            {
                Debug.LogWarning("ONLB: ok=false or backend error");
                return;
            }

            // If leaderboard is null or empty, make sure UpdateRowsFromData can handle it
            LeaderBoardManager.Instance.leaderBoard.UpdateRowsFromData(data);
            LeaderBoardManager.Instance.ShowLeaderBoard();

            if (data.player != null)
            {
                print("player data is not null");

                LeaderBoardManager.Instance.leaderBoard.SetPlayerCard(data.player);
            }
            else
            {
                Debug.Log("ONLB: player is null (backend may not have included player)");
            }

            LeaderBoardManager.Instance.ResetLeaderBoardBtn();
        }
        catch (Exception ex)
        {
            Debug.LogError("ONLB parse error: " + ex);
        }
    }
}



[Serializable]
public class StartSessionData
{
    public string sessionId;
    public string seed;  // optional, could be used for randomness in survival game


}
    [Serializable]
public class ProfileResult
{
    [JsonProperty("ok")]
    public bool Ok { get; set; }

    [JsonProperty("address")]
    public string Address { get; set; }

    [JsonProperty("profile")]
    public string Profile { get; set; }

    [JsonProperty("found")]
    public bool Found { get; set; }
}

[System.Serializable]
public class PaymentStatus
{
    public bool paid;
    public string address;
    public bool loading;
    public string error;
}

[Serializable]
public class BalanceData
{
    public bool ok;
    public string balance;
    public string address;
    public string error;

}


[Serializable]
public class TransactionStatus
{
    public bool ok;
    public string status;
    public string message;
}

[Serializable]
public class PeriodResponse
{
    public bool ok;
    public int periodIndex;
    public long periodStart;        // ms since epoch
    public long periodEnd;          // ms since epoch
    public long durationMs;
    public long remainingMs;
    public int remainingSec;
    public string remainingHuman;
    public float progressPercent;   // 0..100
    public string status;
    public string lastPayoutTx;
    public List<Payout> payouts;
}
[Serializable]
public class Payout
{
    public string user;
    public float amount;   // backend seems to send numeric value
}

[System.Serializable]
public class PeriodData
{
    public long timeLeft;
    public long periodEnd;
}
[System.Serializable]
public class TimeLeftData
{
    public long timeLeft; // in milliseconds
}