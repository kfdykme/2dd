using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Completed;
public class AiSystem : MonoBehaviour
{
    public static AiSystem instance;

    public List<Team> teams;

    public List<GameItem> actionUnits;
    public List<GameItem> targetUnits;
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

        InitAiSystem();
    }

    

    public void InitAiSystem() {
        teams = TeamContainor.instance.teams;
    }

    public void AiTurn(Team team) { 
        List<Team> otherTeams = new List<Team>();

        teams.ForEach(t => {
            if (!t.teamName.Equals(team.teamName)) {
                otherTeams.Add(t);
            }
        });

        List<GameItem> otherUnits = new List<GameItem>();
        otherTeams.ForEach(ot => {
            ot.initialedUnits.ForEach(tu => {
                otherUnits.Add(tu);
            });
        });

        targetUnits = otherUnits;

        actionUnits = team.initialedUnits;
 
        NotifyAiAction();
    }

    public void NotifyAiAction() {
        if (actionUnits.Count == 0 ) {
            return;
        }
         AiUnitAction(actionUnits[0], targetUnits);
         actionUnits.RemoveAt(0);
    }

    
    private void AiUnitAction(GameItem unit, List<GameItem> otherUnits) {
        
        GameItem closestUnit = GetClosestUnit(unit, otherUnits);

        int actionStatus = unit.GetActionStatus();
        
        ActionUnit(unit, closestUnit, actionStatus);
    }

    private void ActionUnit(GameItem unit, GameItem target, int actionStatus) {
        print("Action unit :" + unit.id + " -> " + target.id + " ," + actionStatus);
        if (actionStatus.Equals(GameItem.ACTION_STATUS_GO)) {
            print(unit.id);    
            List<BoardManager.Light> lightsCanGo = GameManager.instance.GetLightsCanGo(unit);
            unit.lightsCanGo = lightsCanGo;
            //get target lights
            float min = 9999;
            Vector2 targetPosition = new Vector2(0,0);
            lightsCanGo.ForEach(l => {
                    float sqrRemainingDistance = (target.GetPosition() - l.position).sqrMagnitude;

                    if (sqrRemainingDistance < min) {
                        min = sqrRemainingDistance;
                        targetPosition = l.position;
                    }
            });

            //order move unit
            unit.OrderMove(targetPosition);
        }
    }

    private GameItem GetClosestUnit(GameItem unit, List<GameItem> otherUnits) {
        float min = 9999;
        GameItem closestUnit = null;
        // print(otherUnits.Count);
        otherUnits.ForEach(u => {
            float sqrRemainingDistance = (unit.GetPosition() - u.GetPosition()).sqrMagnitude;
            if (min > sqrRemainingDistance) {
                min = sqrRemainingDistance;
                closestUnit = u;
            }
        });

        if (closestUnit == null) {
            throw new System.Exception("closest unit is null");
        }
        return closestUnit;
    }
}
