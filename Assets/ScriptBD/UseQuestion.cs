using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "UseQuestion", menuName = "Scriptable Objects/UseQuestion")]
public class UseQuestion : ScriptableObject
{
    public List<QuestionNo> use_questions;

    public void ResizeGameObjectList(int targetSize, string gameName)
    {
        if (string.IsNullOrEmpty(gameName))
        {
            Debug.LogWarning("Game name is empty or null; using default name 'UnknownGame'.");
            gameName = "UnknownGame";
        }

// Check if an entry with the given gameName already exists
        bool gameExists = false;
        for (int i = 0; i < use_questions.Count; i++)
        {
            if (use_questions[i].GameName == gameName)
            {
                gameExists = true;
          //      Debug.Log($"Game {gameName} already exists at index {i}, preserving its q_no list.");
                break;
            }
        }

// Add new entry only if the gameName doesn't exist and targetSize is positive
        if (!gameExists && targetSize > -1)
        {
            use_questions.Add(new QuestionNo
            {
                GameName = gameName,
                q_no = new List<int>()
            });
         //   Debug.Log($"Added new entry for game {gameName}.");
        }
    }
}

[System.Serializable]
public class QuestionNo
{
    public string GameName;
    public List<int> q_no;
}

