using UnityEngine;
using System.Collections;

using System.Collections.Generic;        //Allows us to use Lists. 
using Completed;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{

    public static GameManager instance = null;
    public float turnDelay = .1f;
    public BoardManager boardScript;

    public EventSystem eventSystemScript;

    public int playerFoodPoints = 100;

    [HideInInspector] public bool playersTurn = true;
    [HideInInspector] public bool isMenuing = false;
    [HideInInspector] public bool isMoveing = false;
    private int level = 3;
    private List<GameItem> playerUnits;
    public List<Team> mTeams;

    private int mCurrentTeamPos = -1;

    private int currentTurn = 0;

    public RoundAnimationManager roundAnimationManager;

    public MenuBar menuBar;

    public void UnitDie(GameItem gameItem)
    {
        GameItem.checkIsUnit(gameItem);

        gameItem.Dead();
    }

    public bool GetCanAttack(float x, float y)
    {
        BoardManager.Light light = boardScript.getLightByXY((int)x, (int)y);
        // print(x + "/" + y + ":" + light.status + ":" + light.statusCode);
        return light.statusCode.Equals(BoardManager.Light.STATUS_RED);
    }

    public bool NowTeamInclude(GameItem item)
    {
        bool isInclude = false;
        if (item == null) return isInclude;
        mTeams[mCurrentTeamPos].teamUnits.ForEach(unit =>
        {
            if (unit.Equals(item))
                isInclude = true;
        });
        print("check in include:" + item.id + ", result:" + isInclude);
        return isInclude;
    }

    public void NotifyMoveEndMenu(GameItem gameItem)
    {
        menuBar.notifyMenu(true, new Vector2(0, 0), gameItem);
        isMenuing = true;
    }

    public void notifyMenu(bool status, GameItem focus)
    {
        if (!NowTeamInclude(focus))
        {
            menuBar.notifyMenu(status, new Vector2(0, 0), null);
        }
        else
        {
            menuBar.notifyMenu(status, new Vector2(0, 0), focus);
        }
        isMenuing = status;
    }

    public bool checkNextTurn()
    {
        bool allNext = true;
        mTeams[mCurrentTeamPos].teamUnits.ForEach(unit =>
        {
            if (unit.isWaitNext == false)
            {
                allNext = false;
            }
        });
        if (allNext)
        {
            nextTurn();
        }
        return allNext;
    }

    public GameItem GetOtherTeamUnitByXY(float x, float y)
    {
        return GetOtherTeamUnitByXY((int)x, (int)y);
    }

    public GameItem GetOtherTeamUnitByXY(int x, int y)
    {
        print("GetOtherTeamUnitByXY");
        GameItem unit = getByXY(x, y);
        if (unit != null && unit.gameItemName.Equals(GameItem.TYPE_UNIT))
        {

            if (!NowTeamInclude(unit))
            {
                return unit;
            }
        }
        return null;
    }

    public bool hasUnit(float x, float y)
    {
        return hasUnit((int)x, (int)y);
    }

    public bool hasUnit(int x, int y)
    {
        return getByXY(x, y).gameItemName.Equals(GameItem.TYPE_UNIT);
    }

    public GameItem getByXY(float x, float y)
    {
        return getByXY((int)x, (int)y);
    }

    public GameItem getByXY(int x, int y)
    {
        GameItem target = null;
        target = boardScript.getByXY(x, y);
        playerUnits.ForEach(i =>
        {
            if (i.x == x && i.y == y
                && !i.isDead)
            {
                target = i;
            }
        });
        if (target == null)
        {
            throw new System.Exception("GameManager get a null gameitem from (" + x + "," + y + ");");
        }

        return target;
    }

    public int getMovementCost(float x, float y)
    {
        return getMovementCost((int)x, (int)y);
    }

    public int getMovementCost(int x, int y)
    {
        return getByXY(x, y).moveCoast;
    }

    public void nextTurn()
    {
        mCurrentTeamPos = (mCurrentTeamPos + 1) % mTeams.Count;

        mTeams.ForEach(team =>
        {
            team.teamUnits.ForEach(unit =>
            {
                unit.Refresh();
            });
        });

        playerUnits.ForEach(unit =>
        {
            unit.Refresh(); 
        });
        if (mCurrentTeamPos == 0)
        {

            currentTurn++;
        }
        roundAnimationManager.playTeam(currentTurn, mTeams[mCurrentTeamPos]);
        print("GameManager: next turn - " + currentTurn);
    }

    public void AddUnitTeam(Team team)
    {
        mTeams.Add(team);
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

        DontDestroyOnLoad(gameObject);
        playerUnits = new List<GameItem>();
        boardScript = GetComponent<BoardManager>();
        eventSystemScript = GetComponent<EventSystem>();
        InitGame();
    }

    void InitGame()
    {
        initTeam();
        playerUnits.Clear();
        boardScript.SetupScene(level);

        nextTurn();
    }

    private void initTeam()
    {
        print("init team start");

        mTeams.ForEach(team =>
        {
            print(team.teamName);
            team.teamUnits.ForEach(unit =>
            {
                print(unit.gameObject.tag);
            });
        });
        print("init team end");
    }

    public void GameOver()
    {
        enabled = false;
    }
    // Start is called before the first frame update 

    public void AddUnitToList(GameItem script)
    {
        playerUnits.Add(script);
    }
    public bool checkCursorMove(Vector2 start, int xDir, int yDir)
    {

        Vector2 end = start + new Vector2(xDir, yDir);
        return boardScript.getBlueLightActive(end) || hasUnit(end.x, end.y);
    }

    public List<BoardManager.Light> callUnitMove(GameItem item, bool status)
    {
        isMoveing = status;
        Rigidbody2D rg2D = item.GetComponent<Rigidbody2D>();
        List<BoardManager.Light> lights = boardScript.getLightsCanGo(rg2D, item.movement, new List<BoardManager.Light>());
        List<BoardManager.Light> results = new List<BoardManager.Light>();
        if (!status)
        {
            lights = boardScript.lights;
        }

        List<BoardManager.Light> others = new List<BoardManager.Light>();
        lights.ForEach(light =>
        {
            if (!hasUnit(light.position.x, light.position.y))
            {

                light.notify(status);
                results.Add(light);
            }

            if (status)
            {
                getOtherTeamUnitNextTo(light).ForEach(i =>
                {
                    if (others.Find(p =>
                    {
                        return p.Equals(i);
                    }) == null)
                        others.Add(i);
                });
            }
            else
            {

                light.notify(status, BoardManager.Light.STATUS_BLUE);
            }
        });


        others.ForEach(l =>
        {
            l.notify(status, BoardManager.Light.STATUS_RED);
            // l.Object.GetComponent<SpriteRenderer>().color = Color.black;
        });
        return results;
    }


    public List<BoardManager.Light> getOtherTeamUnitNextTo(BoardManager.Light light)
    {
        List<BoardManager.Light> ls = new List<BoardManager.Light>();
        boardScript.getLightsNextTo(light).ForEach(i =>
        {
            if ((i.position - light.position).sqrMagnitude < 1.5
                && i.position != light.position
                )
            {

                GameItem unit = GetOtherTeamUnitByXY(i.position.x, i.position.y);
                if (unit != null && !unit.isDead)
                    ls.Add(i);
            }
        });
        return ls;
    }

}
