using TMPro;
using UnityEngine;

public class LeaderBoardManager : Singleton<LeaderBoardManager>
{
    public  LeaderBoard leaderBoard;    
    public  TextMeshProUGUI LeaderBoardFeedBack,TimerText;
    
    
    bool pending;    
    public void TryGetLeaderBoard()
    {
        Debug.Log("trying open leader board in unity");
        if (pending)
        {
            Debug.Log("leaderBoard Pending, ignore");
            return;
        }
        Debug.Log("not pending leader board - sending to js");
        pending = true;
        LeaderBoardFeedBack.text = "Loading...";
        JsBridge_Send.Instance.RequestLeaderBoard();

    }

    public void CloseLeaderBoard()
    {
        print("closing leaderboard");
        leaderBoard.LeaderBoarParent.SetActive(false);
        pending = false;
    }

    public void ShowLeaderBoard()
    {

        leaderBoard.LeaderBoarParent.gameObject.SetActive(true);
        leaderBoard.gameObject.SetActive(true);
        print(leaderBoard.LeaderBoarParent.gameObject.activeInHierarchy +"is leaderboard active");
        print(leaderBoard.gameObject.activeInHierarchy + "is container active");
    }


    public void ResetLeaderBoardBtn()
    {
        LeaderBoardFeedBack.text = "Leader Board";
        pending = false;
    }





}
