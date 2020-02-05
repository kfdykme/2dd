using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameItem : MonoBehaviour
{
    public string gameItemName = NAME_UNSET;

    public static string NAME_UNSET = "unset";

    public static string TYPE_UNIT = "unit";

    public static string TYPE_FLOOR = "floor";

    public static string TYPE_ITEM = "item";

    public bool isWaitToMove = false;

    public bool isWaitNext = false;

    public int movement = 3;

    public int moveCoast = 1;
    
    private Rigidbody2D rb2D;
    private float inverseMovetime;
    
    public float moveTime = 0.1f;

    public int x  = -1;

    public int y  = -1;

    private List<int> unitButtonCodes = new List<int>();
    public static List<int> nullButtonCodes = new List<int>();
    
    public List<Completed.BoardManager.Light> lightsCanGo = new List<Completed.BoardManager.Light>();
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        inverseMovetime = 1f / moveTime;

        //
        unitButtonCodes.Add(MenuButton.CODE_MOVE);
        unitButtonCodes.Add(MenuButton.CODE_WAIT);

        nullButtonCodes.Add(MenuButton.CODE_NEXT_TURN); 
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        x = (int)transform.position.x;
        y = (int)transform.position.y;
    }
 

    public static bool isOpenNullButton(int buttonCode) {
        return nullButtonCodes.IndexOf(buttonCode) != -1;
    }
    public bool isOpenButton(int buttonCode) {
        if (gameItemName.Equals(TYPE_UNIT)) {
            if (isWaitNext) {
                return nullButtonCodes.IndexOf(buttonCode) != -1;
            }
            return unitButtonCodes.IndexOf(buttonCode) != -1;
        } else if (gameItemName.Equals(TYPE_FLOOR)) {
return nullButtonCodes.IndexOf(buttonCode) != -1;
        } else {
            return false;
        }
    }
    public bool callUnitMove(bool status) {
        if (gameItemName.Equals(TYPE_UNIT)) {
            isWaitToMove = status;
            lightsCanGo = GameManager.instance.callUnitMove(this, status);
            return true;
        } else {
            return false;
        }
    }

    public bool OrderMove(Vector2 end) {
        if (GameManager.instance.hasUnit(end.x, end.y)){
            return false;
        }
        Vector3 end3 = new Vector3(end.x, end.y, 0);

        List<List<Vector2>> lists = pathTo(transform.position,end, movement);
        lists.Sort((a, b) => a.Count - b.Count); 
        StartCoroutine(SmoothMovement(lists[0],1));
        isWaitNext = true;
        GameManager.instance.checkNextTurn();
        return true;
    } 

    public List<Vector2> GetNextTo(Vector2 start, List<Completed.BoardManager.Light> lists) {
        List<Vector2> tlist= new List<Vector2>();
        lists.ForEach(item => {
            if ((item.position - start).sqrMagnitude < 1.5 && 
            (item.position != start)) {
                tlist.Add(item.position);
            }
        }); 
        return tlist;
    }

    public List<List<Vector2>> pathTo(Vector2 start, Vector2 end, int tmovement) {
        List<List<Vector2>> paths = new List<List<Vector2>>();
        
        List<Vector2> newL = new List<Vector2>();
        if ((end - start).sqrMagnitude < 1.5) {
            newL.Add(start);
            newL.Add(end);
            paths.Add(newL);
        } else if (tmovement >1) { 
            GetNextTo(start, lightsCanGo).ForEach(next => { 
                
                pathTo(next, end, tmovement-GameManager.instance.getMovementCost(next.x, next.y)).ForEach(np => {
                    newL = new List<Vector2>();
                    newL.Add(start);
                    newL.AddRange(np);
                    paths.Add(newL);
                });
            });
        }
        return paths;
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

        if (pos+1 < path.Count) {

             StartCoroutine(SmoothMovement(path,pos +1));
        }
    }
}
