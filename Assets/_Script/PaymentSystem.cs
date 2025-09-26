using OctoberStudio.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PaymentSystem : Singleton<PaymentSystem>
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    public ReplaySubmit replaySubmit;

    public bool WagerGamer { get; set; }    
    public bool HasPaid { get; private set; }
    

   public void LetIn()
    {
        replaySubmit = new ReplaySubmit(PlayerPrefs.GetString("sessionId"),Login.Instance.walletAddress);
        WagerGamer = true; 
        JsBridge_Send.Instance.RequestStartSession();
    }

   public void NotIn()
    {
        WagerGamer = false;
    }

   public void PaimentAccomplished()
   {
        HasPaid = true;
        LobbyWindowBehavior.Instance.EnableWagerPlay();
   }

    public void NotPayed()
    {
        HasPaid = false;
        LobbyWindowBehavior.Instance.DisableWagerPlay();
    }

    public void AddRePlayEvent(ReplayEvent replayEvent)
    {
        if (replaySubmit != null)
        {
            if(replaySubmit.replay != null)
            replaySubmit.replay.Add(replayEvent);
        }
    }

}
[Serializable]
public class ReplayEvent
{
    public string  time;   // must be >= 0
    public int score;  // must be >= 0

    public ReplayEvent(string t, int s)
    {
        time = t;
        score = s;
    }
}

[Serializable]
public class ReplaySubmit
{
    public string sessionId;
    public string userAddress;
    public List<ReplayEvent> replay = new List<ReplayEvent>();

    public ReplaySubmit(string session, string addr)
    {
        sessionId = session;
        userAddress = addr;
    }
}