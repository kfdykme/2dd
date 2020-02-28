using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class GameItem : MonoBehaviour
{
    public string gameItemName = NAME_UNSET;

    public static string NAME_UNSET = "unset";

    public static string TYPE_UNIT = "unit";

    public static string TYPE_FLOOR = "floor";

    public static string TYPE_ITEM = "item";

    public bool isWaitToMove = false;

    public bool isWaitNext = false;

    public bool isDead = false;

    public bool isCanMove = true;


    public int movement = 3;

    public int moveCoast = 1;

    private Rigidbody2D rb2D;
    private float inverseMovetime;

    public float moveTime = 0.1f;

    public int x = -1;

    public int y = -1;

    public string id = "";

    private List<int> unitButtonCodes = new List<int>();

    public static List<int> nullButtonCodes = new List<int>();

    public List<Completed.BoardManager.Light> lightsCanGo = new List<Completed.BoardManager.Light>();

    private UnitFlag unitFlag;

    public Color teamColor;

    public int HP;

    public int currentHP;

    public int attack;

    public int mona;

    public int defance;

    public int monadef;

    public int c;

    public int d;

    // 攻击射程 
    public int range = 1;

    // 所属队伍
    public Team team;

    public class GameItemException : System.Exception
    {
        public GameItemException() { }
        public GameItemException(string message) : base(message) { }
        public GameItemException(string message, System.Exception inner) : base(message, inner) { }
        protected GameItemException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    public static int ACTION_STATUS_GO = 1;


    public int GetActionStatus() {
        return ACTION_STATUS_GO;
    }

    public Vector2 GetPosition() {
        return new Vector2(x,y);
    }

    public void Dead()
    {

        gameObject.SetActive(false);
        isDead = true;
    }


    public static void checkIsUnit(GameItem item)
    {
        if (!item.gameItemName.Equals(TYPE_UNIT))
            throw new GameItemException("This is not an unit:" + item.GetType());
    }

    // override object.Equals
    public override bool Equals(object obj)
    {

        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        return ((GameItem)obj).id.Equals(id);
    }

    // override object.GetHashCode
    public override int GetHashCode()
    {
        // TODO: write your implementation of GetHashCode() here
        throw new System.NotImplementedException();
        return base.GetHashCode();
    }

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        inverseMovetime = 1f / moveTime;

        //
        unitButtonCodes.Add(MenuButton.CODE_MOVE);
        unitButtonCodes.Add(MenuButton.CODE_WAIT);

        nullButtonCodes.Add(MenuButton.CODE_NEXT_TURN);

        if (gameItemName.Equals(TYPE_UNIT))
        {
            InitUnit();
        }
    }

    public void InitUnit()
    {
        unitFlag = GetComponent<Transform>().GetChild(0).GetComponent<UnitFlag>();
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        x = (int)transform.position.x;
        y = (int)transform.position.y;

        if (gameItemName.Equals(TYPE_UNIT))
        {
            UpdateUnit();
        }
    }

    private void UpdateUnit()
    {
        unitFlag.transform.position = new Vector2(unitFlag.xOffset + x, unitFlag.yOffset + y);
        unitFlag.GetComponent<SpriteRenderer>().color = teamColor;
    }


    public static bool isOpenNullButton(int buttonCode)
    {
        return nullButtonCodes.IndexOf(buttonCode) != -1;
    }

    private List<int> GetUnitButtonCodes()
    {
        List<int> codes = new List<int>();
        print("GetUnitButtonCodes from " + id);
        if (isDead) return codes;
        if (isCanMove)
        {
            codes.Add(MenuButton.CODE_MOVE);
        }

        if (!isWaitNext)
        {
            codes.Add(MenuButton.CODE_WAIT);
        }


        if (CheckCanAttack())  {
            print("GameItem :" + id +   "add menu button :" + MenuButton.CODE_ATTACK);
            codes.Add(MenuButton.CODE_ATTACK);
        }

        return codes;
    }


    private int movementSave = 0;
    /**
     * @description: 将移动能力暂时变为零
     * @param {type} 
     * @return: 
     */
    public void WaitToAttack() {
        movementSave = movement;
        movement = 0;
    }

    public void RefreshMovement() {
        movement = movementSave;
    }

    private bool CheckCanAttack() {
        return TeamContainor.instance.GetCanAttack(this).Count != 0;
    }

    public bool isOpenButton(int buttonCode)
    {
        if (gameItemName.Equals(TYPE_UNIT))
        {
            if (isWaitNext)
            {
                return nullButtonCodes.IndexOf(buttonCode) != -1;
            }
            return GetUnitButtonCodes().IndexOf(buttonCode) != -1;
        }
        else if (gameItemName.Equals(TYPE_FLOOR))
        {
            return nullButtonCodes.IndexOf(buttonCode) != -1;
        }
        else
        {
            return false;
        }
    }
    public bool callUnitMove(bool status)
    {
        if (gameItemName.Equals(TYPE_UNIT))
        {
            isWaitToMove = status;
            lightsCanGo = GameManager.instance.callUnitMove(this, status);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool OrderMove(Vector2 end)
    {
        print("GameItem from :" + GetPosition() + " OrderMove to :" + end);
        print("Transform.position is:" + transform.position);
        if (GameManager.instance.hasUnit(end.x, end.y))
        {
            return false;
        }
        Vector3 end3 = new Vector3(end.x, end.y, 0);

        List<List<Vector2>> lists = pathTo(GetPosition(), end, movement);
        
        lists.Sort((a, b) => a.Count - b.Count);
        
        if (lists.Count == 0) {
            print("Paths from " + GetPosition() + " to " + end + "is 0");
            return false;
        }
        StartCoroutine(SmoothMovement(lists[0], 1));

        return true;
    }

    public bool OrderMove(Vector2 end, GameItem fightWith)
    {

        FightSystem.instance.fight(
            this,
            fightWith
        );
        if (end.x == x && end.y == y)
        {
            callUnitMove(false);
            NotifyMoveEnd();
            return false;
        }
        else
        {
            return OrderMove(end);
        }


    }

    public List<Vector2> GetNextTo(Vector2 start, List<Completed.BoardManager.Light> lists)
    {
        List<Vector2> tlist = new List<Vector2>();
        lists.ForEach(item =>
        {
            if ((item.position - start).sqrMagnitude < 1.5 &&
            (item.position != start))
            {
                tlist.Add(item.position);
            }
        });
        return tlist;
    }

    public List<List<Vector2>> pathTo(Vector2 start, Vector2 end, int tmovement)
    {
        List<List<Vector2>> paths = new List<List<Vector2>>();

        List<Vector2> newL = new List<Vector2>();
        if ((end - start).sqrMagnitude < 1.5)
        {
            newL.Add(start);
            newL.Add(end);
            paths.Add(newL);
        }
        else if (tmovement > 1)
        {
            GetNextTo(start, lightsCanGo).ForEach(next =>
            {

                pathTo(next, end, tmovement - GameManager.instance.getMovementCost(next.x, next.y)).ForEach(np =>
                {
                    newL = new List<Vector2>();
                    newL.Add(start);
                    newL.AddRange(np);
                    paths.Add(newL);
                });
            });
        }
        return paths;
    }

    public void Refresh() {
        
        isWaitNext = false;
        isCanMove = true;
    }

    private void NotifyMoveEnd()
    {
        isCanMove = false;
        FightSystem.instance.NotifyNextFigth();
        if (!isDead)
            GameManager.instance.NotifyMoveEndMenu(this);
        if (team.teamFlag.Equals(Team.TEAM_FLAG_C))
            AiSystem.instance.NotifyAiAction();
    }


    protected IEnumerator SmoothMovement(List<Vector2> path, int pos)
    {
        Vector3 end = new Vector3(path[pos].x, path[pos].y, 0);
        callUnitMove(false);
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMovetime * Time.deltaTime);
            rb2D.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }

        if (pos + 1 < path.Count)
        {

            StartCoroutine(SmoothMovement(path, pos + 1));
        }
        else
        {
            NotifyMoveEnd();
        }
    }
}
