using OctoberStudio.UI;
using UnityEngine;

public class PaymentSystem : Singleton<PaymentSystem>
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    public bool WagerGamer { get; set; }    
   public bool HasPaid { get; private set; }
    

   public void LetIn()
    {
        WagerGamer = true;  
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

    

}
