using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    // --------------------------------------------------------------

    [SerializeField]
    Text m_Player1ScoreText;

    [SerializeField]
    Text m_Player2ScoreText;

    // --------------------------------------------------------------

    int m_Player1Score = 0;
    int m_Player2Score = 0;

    // --------------------------------------------------------------

    void OnEnable()
    {
        DeathTrigger.OnPlayerDeath += OnUpdateScore;
    }

    void OnDisable()
    {
        DeathTrigger.OnPlayerDeath -= OnUpdateScore;
    }

    void OnUpdateScore(int playerNum)
    {
        if(playerNum == 1)
        {
            m_Player2Score += 1;
            m_Player2ScoreText.text = "" + m_Player2Score;
        }
        else if(playerNum == 2)
        {
            m_Player1Score += 1;
            m_Player1ScoreText.text = "" + m_Player1Score;
        }
    }
}
