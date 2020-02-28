using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamContainor : MonoBehaviour
{
    public static TeamContainor instance;


    public List<Team> teams = new List<Team>();

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

    public List<GameItem> GetOtherTeamUnits(Team team) {
        List<Team> otherTeams = new List<Team>();


        teams.ForEach(t => {
            if (!t.teamName.Equals(team.teamName)) {
                otherTeams.Add(t);
            }
        });

        List<GameItem> otherUnits = new List<GameItem>();
        otherTeams.ForEach(ot => {
            print("Team" + ot.teamName + " has " + ot.initialedUnits.Count + " Units");
            ot.initialedUnits.ForEach(tu => {
                otherUnits.Add(tu);
            });
        });

        print("GetOtherTeamUnits from " + team.teamName + " :" + otherUnits.Count + "/" + teams.Count + "/" + otherTeams.Count);
        return otherUnits;
    }

    public float DistanceBetween(GameItem unit, GameItem other) {
        return (other.GetPosition() - unit.GetPosition()).sqrMagnitude;
    }

    public List<GameItem> GetCanAttack(GameItem gameItem) {
        List<GameItem> resultGameItemList = new List<GameItem>();
        GetOtherTeamUnits(gameItem.team)
        .ForEach(unit => {
            print("Distance between is :" + gameItem.id+ "|" + gameItem.GetPosition() +"|" + " -> " + unit.id + "|" + unit.GetPosition() +"|" + " = " + DistanceBetween(gameItem, unit));
            if (DistanceBetween(gameItem, unit) < 1.5
            && !unit.isDead) {
                resultGameItemList.Add(unit);
            }
        });
        print("GetCanAttack from " + gameItem.id + " is :" + resultGameItemList.Count);
        return resultGameItemList;
    }

}