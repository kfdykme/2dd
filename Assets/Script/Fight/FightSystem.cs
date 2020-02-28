using UnityEngine;
using System.Collections;

using System.Collections.Generic;        //Allows us to use Lists. 
using Completed;
using UnityEngine.UI;
public class FightSystem : MonoBehaviour
{
    public static FightSystem instance = null;

    public List<FightGroup> fightGroups;

    public class FightGroup {
        public GameItem actUnit;
        public GameItem defUnit;

        public FightGroup(GameItem actUnit, GameItem defUnit) {
            this.actUnit = actUnit;
            this.defUnit = defUnit;
        }
    }

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

        InitFightSystem();
    }

    private void InitFightSystem() { 
        fightGroups =  new List<FightGroup>();
    }


    public void NotifyNextFigth() {
        fightGroups.ForEach(fg => {
            DoFight(fg.actUnit, fg.defUnit);
        });
        fightGroups.Clear();
    }

    private void DoFight(GameItem actUnit, GameItem defUnit) {
        
        actUnit.isWaitNext = true;
        // attack 
        defUnit.currentHP -= actUnit.attack - defUnit.defance;
        // check is alive
        if (defUnit.currentHP <= 0) {
            GameManager.instance.UnitDie(defUnit);
            return;
        }

        actUnit.currentHP -= defUnit.attack - actUnit.defance;
        
        if (actUnit.currentHP <= 0) {
            GameManager.instance.UnitDie(actUnit); 
        }

    }

    public void fight(GameItem actUnit, GameItem defUnit) {
        GameItem.checkIsUnit(actUnit);
        GameItem.checkIsUnit(defUnit);

        fightGroups.Add(new FightGroup(actUnit, defUnit)); 
    }
}