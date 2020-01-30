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
    
    private Rigidbody2D rb2D;
    private float inverseMovetime;
    
    public float moveTime = 0.1f;

    public int x  = -1;

    public int y  = -1;

    private List<int> unitButtonCodes = new List<int>();
    public static List<int> nullButtonCodes = new List<int>();

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
            GameManager.instance.callUnitMove(this, status);
            return true;
        } else {
            return false;
        }
    }

    public bool OrderMove(Vector2 end) {
        Vector3 end3 = new Vector3(end.x, end.y, 0);
        StartCoroutine(SmoothMovement(end3));
        isWaitNext = true;
        GameManager.instance.checkNextTurn();
        return true;
    }
 

     protected IEnumerator SmoothMovement(Vector3 end)
    {
        callUnitMove(false);
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMovetime * Time.deltaTime);
            rb2D.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
    }
}
