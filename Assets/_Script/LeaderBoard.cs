using System;
using System.Collections.Generic;
using UnityEngine;

public class LeaderBoard : MonoBehaviour
{
    public List<LeaderBoardVisualItem> leaderBoardVisualItems;
    public LeaderBoardVisualItem player;
    public GameObject LeaderBoarParent;
    
    
    public void UpdateRowsFromData(LeaderBoardData data)
    {
        print(data);
        int maxRows = leaderBoardVisualItems.Count;
        int returned = (data.leaderboard == null) ? 0 : data.leaderboard.Count;
        int count = Mathf.Min(maxRows, returned);
        print(count+" is the count to be in visual");
        // Fill UI rows with actual data
        for (int i = 0; i < count; i++)
        {
            var item = data.leaderboard[i];
            leaderBoardVisualItems[i].SetVisual(item.profile_name ?? item.user_address, item.rank > 0 ? item.rank : (i + 1), item.score);
            leaderBoardVisualItems[i].gameObject.SetActive(true);
            print(item.user_address);
        }

        // Hide or reset extra slots
        for (int i = count; i < maxRows; i++)
        {
            leaderBoardVisualItems[i].SetVisual("...", 0, 0);
            // or leaderBoardVisualItems[i].SetVisual("...", i+1, 0);
        }

        // Player card is handled by ONLB (only if data.player != null)
    }



    public void SetPlayerCard(LeaderBoardItem _player)
    {
        if (player == null || player == null) return;
        player.SetVisual("YOU", _player.rank, _player.score);
    }

}



[Serializable]
public class LeaderBoardItem
{
    public string user_address;
    public string profile_name;
    public int score;
    public int level;
    public int rank; 
}

[Serializable]
public class LeaderBoardData
{
    public bool ok;
    public int count;
    public List<LeaderBoardItem> leaderboard;
    public LeaderBoardItem player;
}




