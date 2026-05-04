using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DummyData", menuName = "Scriptable Objects/DummyData")]
public class DummyData : ScriptableObject
{
    public Rootdummy root;
    [System.Serializable]
    public class Datumdummy
    {
        public string game_name;
        public string image;
        public List<Questiondummy> questions ;
    }
    [System.Serializable]
    public class Rootdummy
    {
        public bool status ;
        public string message ;
        public List<Datumdummy> data ;
        public int statusCode ;
    }
    [System.Serializable]
    public class Questiondummy
    {
        public string id ;
        public string question ;
        public string option_a ;
        public string option_b ;
        public string option_c ;
        public string option_d ;
    }
}