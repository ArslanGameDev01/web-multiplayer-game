using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionHandler : MonoBehaviour
{
    public bool isCorrect;
    public Image state;
    public List<GameObject> star;
    public List<GameObject> streak;
    private GamePlayHandler gamePlayHandler;

    public void SetCorrect(bool correct, GamePlayHandler handler)
    {
        isCorrect = correct;
        gamePlayHandler = handler;
        state.sprite = GamePlayHandler.instance.defaultSprite;
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
        for (int i = 0; i < GamePlayHandler.instance.optButton.Count; i++)
        {
            GamePlayHandler.instance.optButton[i].interactable = false;
        }
        
        if (isCorrect)
        {
            GamePlayHandler.instance.score++;
            for (int i = 0; i < star.Count; i++)
            {
                star[i].SetActive(true);
            }

            state.sprite = GamePlayHandler.instance.correctState;
            //state.SetNativeSize();
            state.gameObject.SetActive(true);
            GamePlayHandler.instance.continiousRightQuestion++;
            GamePlayHandler.instance.threeRightQuestion++;
            GamePlayHandler.instance.sevenRightQuestion++;
            if (GamePlayHandler.instance.continiousRightQuestion > GamePlayHandler.instance.streak)
            {
                for (int i = 0; i < star.Count; i++)
                {
                    streak[i].SetActive(true);
                }
            }

            if (GamePlayHandler.instance.is3XBoosterActive)
            {
                GamePlayHandler.instance.score += 2;
                GamePlayHandler.instance.tripleDownPoints += 3;
                GamePlayHandler.instance.is3XBoosterActive = false;
                GamePlayHandler.instance.boosterBtn.SetActive(false);
            }

            if (GamePlayHandler.instance.threeRightQuestion == 3)
            {
                GamePlayHandler.instance.score += 3;
                GamePlayHandler.instance.streakPoints += 3;
                GamePlayHandler.instance.threeRightQuestion = 0;
            }

            if (GamePlayHandler.instance.sevenRightQuestion == 7)
            {
                GamePlayHandler.instance.score += 10;
                GamePlayHandler.instance.sevanPoints += 10;
                GamePlayHandler.instance.sevenRightQuestion = 0;
                GamePlayHandler.instance.totalSevenInRow++;
            }
            GamePlayHandler.instance.rightQuestion++;
        }
        else
        {
            GamePlayHandler.instance.threeRightQuestion = 0;
            GamePlayHandler.instance.continiousRightQuestion = 0;
            GamePlayHandler.instance.sevenRightQuestion = 0;
            GamePlayHandler.instance.wrongQuestion++;
            if (GamePlayHandler.instance.is3XBoosterActive)
            {
                GamePlayHandler.instance.is3XBoosterActive = false;
                GamePlayHandler.instance.boosterBtn.SetActive(false);
            }

            state.sprite = GamePlayHandler.instance.incorrectState;
            state.gameObject.SetActive(true);
            Invoke("ShowCorrect",.25f);
        }
        gamePlayHandler.LoadNextQuestionWithDelay();
    }

    public void ShowCorrect()
    {
        for (int i = 0; i < GamePlayHandler.instance.optButton.Count; i++)
        {
            if (GamePlayHandler.instance.optionHandlers[i].isCorrect)
            {
                GamePlayHandler.instance.optionHandlers[i].state.sprite = GamePlayHandler.instance.correctState;
                GamePlayHandler.instance.optionHandlers[i].state.gameObject.SetActive(true);
            }
        }
        
        
    }

    public void ResetOption()
    {
        for (int i = 0; i < GamePlayHandler.instance.optButton.Count; i++)
        {
            GamePlayHandler.instance.optionHandlers[i].isCorrect = false;
        }
    }
}