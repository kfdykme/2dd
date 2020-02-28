using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamContainor : MonoBehaviour
{
    public static TeamContainor instance;


    public List<Team> teams;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
 
    }

    public void AddTeam(Team team)
    {
        teams.Add(team);
    }

}