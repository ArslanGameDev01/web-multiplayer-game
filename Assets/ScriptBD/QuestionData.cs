using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestionData", menuName = "Scriptable Objects/QuestionData")]
public class QuestionData : ScriptableObject
{
    public bool isFirstTime = false;
    public int gameNumber;
    public string gameName="";
    public Root root;
}

[System.Serializable]
public class Agerange
{
    public string id;
    public string min_age;
    public string max_age;
}

[System.Serializable]
public class Category
{
    public string id;
    public string title;
}

[System.Serializable]
public class Data
{
    public List<Game> games;
    public List<Category> categories;
    public List<Agerange> ageranges;
}

[System.Serializable]
public class Game
{
    public string game_name;
    public string image;
    public string pre_post_panel_image;
    public List<Category> categories;
    public List<Agerange> ageranges;
    public int sponser;
    public string pre_msg;
    public string post_msg;
    public string powered_by;
    public string sponsor_link;
    public string short_description;
    public List<Question> questions;
}

[System.Serializable]
public class Question
{
    public string id;
    public string question;
    public string option_a;
    public string option_b;
    public string option_c;
    public string option_d;
    public string image;
}

[System.Serializable]
public class Root
{
    public bool status;
    public string message;
    public Data data;
    public int statusCode;
}