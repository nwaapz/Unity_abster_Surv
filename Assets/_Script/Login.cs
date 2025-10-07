using UnityEngine;
using UnityEngine.UI;

public class Login : Singleton<Login>
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] TMPro.TMP_InputField UsernameInput;
    [SerializeField] GameObject LogintPanel;
    [SerializeField] TMPro.TextMeshProUGUI FeedBack,userName, walletState,footerFeedback, GamePlayBanner,UserBalance;
   // [SerializeField] Button PaymentBtn;
    [SerializeField] ScoreManager scoreManager;
    public string walletAddress {get; set; } = "";
    public string ProfileName { get; set; } = "";




    bool WalletConnected = false;   
    bool IsProfileSet = false;


    public void SetUserBalance(string value)
    {
        UserBalance.text = value;
    }
    public bool IsWalletConnected()
    {
        return WalletConnected;
    }

    public void setWalletState(bool state)
    {
               WalletConnected = state;
        if (state)
        {
            GamePlayBanner.text = "";
            walletState.text = "connected";
        }
        else
        {
            walletState.text = "not-set";
            walletAddress = string.Empty;

        }
    }

    public void setGameBanner(string text)
    {
        GamePlayBanner.text = text; 
    }

    public void TryConnectWallet()
    {
              // if (WalletConnected) return;
        JsBridge_Send.Instance.TryConnectToPrivyWallet();
        walletState.text = "connecting...";
    }

    public void WalletConnectionFailed()
    {
        WalletConnected = false;
        walletState.text = "not-set";
        walletAddress= string.Empty;
        IsProfileSet = false;
        ProfileName = string.Empty;
        userName.text = "not-set";
    }

    public void SetFooterFeedback(string message)
    {
        print("Footer feedback: " + message);   
        footerFeedback.text = message;
    }   

    public void ShowLogingPanel()
    {
        if (!IsWalletConnected())
        {

            TryConnectWallet();
            return;
        }
        LogintPanel.SetActive(true);
    }

    public void HideLogingPanel()
    {
        LogintPanel.SetActive(false);
    }

    public void SetUserName(string  name)
    {
        userName.text = name;
        IsProfileSet = true;    
        ProfileName = name; 
    }

    bool Halt;
    public void Cancel()
    {
        if(Halt) return;    
        FeedBack.text = "";
        HideLogingPanel();  
    }

    public void OK()
    {
        if (Halt) return;
        string username = UsernameInput.text;
        if (string.IsNullOrEmpty(username))
        {
            FeedBack.text = ("Username cannot be empty.");
            return;
        }
        SetLoginState("Registering your name in...", true);
        JsBridge_Send.Instance.SetNewProfileName(username);
    }

    public void LockPanel()
    {
        Halt = true;
    }


    public void UnlockPanel()
    {
        Halt = false;
    }


    public void SetLoginState(string message,bool locked)
    {
        FeedBack.text = message;

        if(locked) LockPanel();
        else UnlockPanel();
    }


    public void GetProfileData()
    {
        JsBridge_Send.Instance.GetProfileData();
        userName.text = "fetching profile...";
    }

    public void GameOpen()
    {
        scoreManager.ShowScoreDisplay();    
    }

    public void warnConnectWallet()
    {
        GamePlayBanner.text = "You Need to connect your wallet first..";
    }

    public void WarnSetProfile()
    {
        GamePlayBanner.text = "You Need to set your profile before joining game..";
    }

    public void Play()
    {
        if(!WalletConnected)
        {
            warnConnectWallet();
            return;
        }
        if(!IsProfileSet)
        {
            WarnSetProfile();
            return;
        }
        if(!paid)
        {
            GamePlayBanner.text = "You need to pay to play the game.";
            return;
        }

        GameOpen(); 
    }


    public void CheckPaymentStatus()
    {
       JsBridge_Send.Instance.CheckPaimentStatus();
    }


    bool paid;

    public void HasPaid()
    {
      //  PaymentState.text = "paid"; 
     //   PaymentBtn.onClick.RemoveAllListeners();
        paid = true;
    }

    public void PaymentStarted()
    {
     //   PaymentState.text = "processing";
      //  PaymentBtn.onClick.RemoveAllListeners();
    }

    public void PaymentCanceled()
    {
        NotPaid();
    }

    public void NotPaid()
    {
      //  PaymentState.text = "Pay";
       // PaymentBtn.onClick.RemoveAllListeners();
      //  PaymentBtn.onClick.AddListener(Pay);  
        paid = false;   
    }


    public void Pay()
    {
      //  PaymentBtn.onClick.RemoveAllListeners();
        JsBridge_Send.Instance.TryPayForGame();
    }

}
