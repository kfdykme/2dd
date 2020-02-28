using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CursorP : MovingObject
{

    public static CursorP instance = null;
    private GameItem currentFocus = null;

    public Text tx;

    public Image currentFocusSpriteRender;

    private Vector3 mPositionBeforeCursorMove;
    private GameItem moveItem;

    private Rigidbody2D rg2d;

    public Text textUnitInfoAttack;
    public Text textUnitInfoDefance;

    public Text textUnitInfoMovement;

    public GameItem lastFocus;

    private Vector2 lastPosition;
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

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        rg2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        GameItem focus = getFocus();
        if (focus != null)
        {

            currentFocusSpriteRender.sprite = focus.GetComponent<SpriteRenderer>().sprite;

            tx.text = focus.gameObject.tag;
            textUnitInfoAttack.text = focus.attack + "";
            textUnitInfoDefance.text = focus.defance + "";
            textUnitInfoMovement.text = focus.movement + "";
        }
    }

    public void MoveCursorTo (GameItem gameItem) {
        print("Move Cursor To" + gameItem.GetPosition());
        Vector2 dis = gameItem.GetPosition() - new Vector2(transform.position.x,transform.position.y);
        
        lastPosition = transform.position;
        AttemptMove<Wall>((int)dis.x, (int)dis.y);
        lastFocus = null;
        
    }

    public override bool OnKeyDownEvent(KeyCode code)
    {
        int horizontal = 0;
        int vertical = 0;

        switch (code)
        {
            case KeyCode.A:
                return onA();
            case KeyCode.B:
                onB();
                return true;
            default:
                if (code == KeyCode.LeftArrow)
                {
                    horizontal = -1;
                }
                else if (code == KeyCode.RightArrow)
                {
                    horizontal = 1;
                }
                else if (code == KeyCode.DownArrow)
                {
                    vertical = -1;
                }
                else if (code == KeyCode.UpArrow)
                {
                    vertical = 1;
                }

                if (horizontal != 0 || vertical != 0)
                {
                    lastPosition = transform.position;

                    if (check(horizontal, vertical))
                        AttemptMove<Wall>(horizontal, vertical);
                }
                return true;

        }
    }


    public GameItem getFocus()
    {
        GameItem focus;
        if (lastPosition.x != transform.position.x
        || lastPosition.y != transform.position.y
        || lastFocus == null) {
            focus = GameManager.instance.getByXY((int)Math.Round(rg2d.position.x), (int)Math.Round(rg2d.position.y));
            // print("focus change to :" + focus.GetPosition());
            }
        else {
            focus = lastFocus; 
        }
            
        lastFocus = focus;
        if (focus == null)
        {
            print("focus == null");
        }
        return focus;
    }

    private bool check(int x, int y)
    {
        if (checkMoveItem())
        {
            return GameManager.instance.checkCursorMove(transform.position, x, y);
        }
        else
        {
            return true;
        }
    }
    protected override void OnCantMove<T>(T component)
    {
    }


    public void CallUnitAttack()
    {
        GameItem attackItem = getFocus();
        attackItem.WaitToAttack();
        callUnitMove();

        attackItem.RefreshMovement();
    }

    public void callUnitMove()
    {
        moveItem = getFocus();
        moveItem.isWaitToMove = true;
        mPositionBeforeCursorMove = moveItem.GetComponent<Rigidbody2D>().position;
        bool result = moveItem.callUnitMove(true);
    }

    public void callUnitWait()
    {
        moveItem = getFocus();
        moveItem.Wait();
        print(moveItem.id + " call wait");
    }

    private bool shouldShowMenu()
    {
        return !GameManager.instance.isMenuing && !checkMoveItem();
    }

    private bool checkMoveItem()
    {
        return moveItem != null && moveItem.isWaitToMove;
    }

    private bool moveUnit(Vector2 position)
    {

        if (!moveItem.OrderMove(position))
        {
            return false;
        }
        moveItem = null;
        return true;
    }

    private bool moveFightUnit(Vector2 position, GameItem gameItem)
    {

        if (!moveItem.OrderMove(position, gameItem))
        {
            return false;
        }
        moveItem = null;
        return true;
    }
    private bool moveUnit()
    {
        return moveUnit(rg2d.position);
    }
    private bool onA()
    {
        if (GameManager.instance.GetCanAttack(transform.position.x, transform.position.y))
        {
            print(lastPosition + "/" + transform.position.x + "," + transform.position.y);

            moveFightUnit(lastPosition, GameManager.instance.getByXY(transform.position.x, transform.position.y));


        }
        else if (checkMoveItem())
        {
            return moveUnit();
        }
        else if (shouldShowMenu())
        {
            GameManager.instance.notifyMenu(true, getFocus());
            return true;
        }

        return false;
    }

    private void onB()
    {
        Rigidbody2D rg2d = GetComponent<Rigidbody2D>();
        if (GameManager.instance.isMenuing)
        {
            GameManager.instance.notifyMenu(false, null);
        }
        else if (GameManager.instance.isMoveing)
        {
            rg2d.MovePosition(Vector3.MoveTowards(rg2d.position, mPositionBeforeCursorMove, 100f));
            moveItem.callUnitMove(false);
            GameManager.instance.notifyMenu(true, moveItem);
        }
    }
}
