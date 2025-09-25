using TMPro;
using UnityEngine;

public class LeaderBoardVisualItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI level, name, score;

    public void SetVisual(string  _name, int _level, int _score)
    {
        level.text = _level.ToString();
        name.text = _name;  
        score.text = _score.ToString();
        print($"setting visual , name is:{_name}, level is: {_level}, score is :{_score}");
    }

}
