using UnityEngine;

namespace OctoberStudio.Drop
{
    public class CoinDropBehavior : DropBehavior
    {
        [SerializeField] int amount;

        private static float leftoverDifference;

        public override void OnPickedUp()
        {
            base.OnPickedUp();

            gameObject.SetActive(false);

            var gold = amount * PlayerBehavior.Player.GoldMultiplier + leftoverDifference;
            var clampedGold = Mathf.FloorToInt(gold);
            leftoverDifference = gold - clampedGold;
            if(PaymentSystem.Instance.WagerGamer)
            GameController.TempGold.Deposit(clampedGold);
        }
    }
}