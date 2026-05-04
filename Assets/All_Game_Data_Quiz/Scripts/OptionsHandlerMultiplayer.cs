using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsHandlerMultiplayer : MonoBehaviour
{
    public bool isCorrect;
    public Image state;
    public List<GameObject> star;
    public List<GameObject> streak;
    private TriviaManager multiplayerHandler;

    public void SetCorrect(bool correct, TriviaManager handler)
    {
        // Debug.Log(handler.gameObject.name);
        // Debug.Log("correct== "+ correct);
        isCorrect = correct;
        multiplayerHandler = handler;
        state.sprite =  TriviaManager.Instance.defaultSprite;
        // state.SetNativeSize();
        state.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        for (int i = 0; i < star.Count; i++)
        {
            star[i].SetActive(false);
        }

        for (int i = 0; i < star.Count; i++)
        {
            streak[i].SetActive(false);
        }
    }

    public void AnswerBtnClick()
    {
        for (int i = 0; i < TriviaManager.Instance.optButton.Count; i++)
        {
            TriviaManager.Instance.optButton[i].interactable = false;
        }

        if (isCorrect)
        {
            TriviaPlayer.LocalPlayer.Score ++;
            TriviaManager.Instance.score++;
            
            for (int i = 0; i < star.Count; i++)
            {
                star[i].SetActive(true);
            }

            state.sprite = TriviaManager.Instance.correctState;
            //state.SetNativeSize();
            state.gameObject.SetActive(true);
            TriviaManager.Instance.continiousRightQuestion++;
            TriviaManager.Instance.threeRightQuestion++;
            TriviaManager.Instance.sevenRightQuestion++;
            if (TriviaManager.Instance.continiousRightQuestion > TriviaManager.Instance.streak)
            {
                for (int i = 0; i < star.Count; i++)
                {
                    streak[i].SetActive(true);
                }
            }

            if (TriviaManager.Instance.is3XBoosterActive)
            {
                TriviaPlayer.LocalPlayer.Score += 2;
                    
                TriviaManager.Instance.score += 2;
                TriviaManager.Instance.tripleDownPoints += 3;
                TriviaManager.Instance.is3XBoosterActive = false;
                TriviaManager.Instance.boosterBtn.SetActive(false);
            }

            if (TriviaManager.Instance.threeRightQuestion == 3)
            {
                TriviaPlayer.LocalPlayer.Score += 3;
                
                TriviaManager.Instance.score += 3;
                TriviaManager.Instance.streakPoints += 3;
                TriviaManager.Instance.threeRightQuestion = 0;
            }

            if (TriviaManager.Instance.sevenRightQuestion == 7)
            {
                TriviaPlayer.LocalPlayer.Score += 10;
                
                TriviaManager.Instance.score += 10;
                TriviaManager.Instance.sevanPoints += 10;
                TriviaManager.Instance.sevenRightQuestion = 0;
                TriviaManager.Instance.totalSevenInRow++;
            }

            TriviaManager.Instance.rightQuestion++;
        }
        else
        {
            TriviaManager.Instance.threeRightQuestion = 0;
            TriviaManager.Instance.continiousRightQuestion = 0;
            TriviaManager.Instance.sevenRightQuestion = 0;
            TriviaManager.Instance.wrongQuestion++;
            if (TriviaManager.Instance.is3XBoosterActive)
            {
                TriviaManager.Instance.is3XBoosterActive = false;
                TriviaManager.Instance.boosterBtn.SetActive(false);
            }

            state.sprite = TriviaManager.Instance.incorrectState;
            state.gameObject.SetActive(true);
            Invoke("ShowCorrect", .25f);
        }
        TriviaManager.Instance.LoadNextQuestionWithDelay();
    }

    public void ShowCorrect()
    {
        for (int i = 0; i <     TriviaManager.Instance.optButton.Count; i++)
        {
            if (TriviaManager.Instance.optionHandlers[i].isCorrect)
            {
                TriviaManager.Instance.optionHandlers[i].state.sprite = TriviaManager.Instance.correctState;
                TriviaManager.Instance.optionHandlers[i].state.gameObject.SetActive(true);
            }
        }


    }

    public void ResetOption()
    {
        for (int i = 0; i < TriviaManager.Instance.optButton.Count; i++)
        {
            TriviaManager.Instance.optionHandlers[i].isCorrect = false;
        }
    }
}