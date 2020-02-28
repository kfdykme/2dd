using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour
{
    // Start is called before the first frame update
    public Color mTeamColor;

    public List<GameItem> teamUnits = new List<GameItem>();

    public string teamName;

    public int teamFlag = TEAM_FLAG_C;

    public static int TEAM_FLAG_P = 0;

    public static int TEAM_FLAG_C = 1;

    public List<GameItem> initialedUnits = new List<GameItem>();

    public bool isCom () {
        return teamFlag.Equals(TEAM_FLAG_C);
    }

    void Start()
    { 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
