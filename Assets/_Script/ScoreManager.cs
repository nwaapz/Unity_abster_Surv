using System;
using UnityEngine;

public class ScoreManager : Singleton<ScoreManager>
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] TMPro.TMP_InputField scoreInputField;
    [SerializeField] GameObject scoreDisplay,JsBridge;
    [SerializeField] TMPro.TextMeshProUGUI Feedback;
    public void SetScore()
    {
        int score;
        print("Setting score: " + scoreInputField.text);
        if (int.TryParse(scoreInputField.text, out score))
        {
            print("Valid score input: " + score);   
           // JsBridge.GetComponent<JsBridge_Send>().SetScore(score.ToString());

        }
        else
        {
            print("Invalid score input: " + scoreInputField.text);  
            Feedback.text=("Invalid score input. Please enter a valid integer.");
        }
    }

    public void OnScoreSetConfirmation(string message)
    {
        print(message); 
        Feedback.text = message;
    }

    public void ShowScoreDisplay()
    {
        scoreDisplay.SetActive(true);
    }
    
    public void HideScoreDisplay()
    {
        scoreDisplay.SetActive(false);
    }   


}
