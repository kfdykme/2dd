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
  
    private Vector3 mPositionBeforeCursorMove ;
    private GameItem moveItem;

    private Rigidbody2D rg2d;
 
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else if (instance != this)
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

    void Update() {
        if (getFocus() != null) {

            currentFocusSpriteRender.sprite = getFocus().GetComponent<SpriteRenderer>().sprite;
        
            tx.text = getFocus().gameObject.tag;
        }
    }

    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     GameItem gameItem = other.GetComponent<GameItem>();
    //     currentFocus = gameItem;   
    //     if (moveItem == null) {

    //         currentFocusSpriteRender.sprite = other.GetComponent<SpriteRenderer>().sprite;
    //         tx.text = other.tag;    
    //     }
    // }
 
    public override  bool OnKeyDownEvent(KeyCode code) {
         int horizontal = 0;    
        int vertical = 0;      
           
        switch(code) {
            case KeyCode.A:
                 onA();
                return true;
            case KeyCode.B:
                 onB();
                return true;
            default:
                if (code == KeyCode.LeftArrow)
                {
                    horizontal = -1;
                } else if (code == KeyCode.RightArrow)
                {
                    horizontal = 1;
                } else if (code == KeyCode.DownArrow) {
                    vertical = -1;
                } else if (code == KeyCode.UpArrow){
                    vertical = 1;
                }
                
                if (horizontal != 0 || vertical != 0)
                {   
                    if (check(horizontal, vertical))
                        AttemptMove<Wall>(horizontal, vertical);
                }
                return true;
            
        }
    }
 
    
    public GameItem getFocus() {
        GameItem focus = GameManager.instance.getByXY((int)Math.Round(rg2d.position.x),(int)Math.Round(rg2d.position.y));

        if (focus == null) {
            print ("focus == null");
        }
        return focus;
    }
 
    private bool check(int x, int y) {
        if (checkMoveItem()) {
            return GameManager.instance.checkCursorMove(transform.position,x, y);
        } else {
            return true;
        }
    }
    protected override void OnCantMove<T>(T component)
    {
    }
 
     public void callUnitMove() { 
        moveItem = getFocus();
        moveItem.isWaitToMove = true;
        mPositionBeforeCursorMove = moveItem.GetComponent<Rigidbody2D>().position;
        bool result = moveItem.callUnitMove(true);
    }

    private bool shouldShowMenu() {
        return !GameManager.instance.isMenuing && !checkMoveItem();
    }

    private bool checkMoveItem() {
        return moveItem != null && moveItem.isWaitToMove ;
    }
    
    private void moveUnit() {
       
        moveItem.OrderMove(rg2d.position); 
        moveItem = null;
    }
    private void onA () {
        
        if (checkMoveItem()) {
            moveUnit();
        } else if (shouldShowMenu()) {
            GameManager.instance.notifyMenu(true, getFocus());
        } 
    }

    private void onB () {
        Rigidbody2D rg2d = GetComponent<Rigidbody2D>();
        if (GameManager.instance.isMenuing) {
            GameManager.instance.notifyMenu(false, null);
        }  else if (GameManager.instance.isMoveing) {
            rg2d.MovePosition(Vector3.MoveTowards(rg2d.position, mPositionBeforeCursorMove, 100f));
            moveItem.callUnitMove(false);
            GameManager.instance.notifyMenu(true, moveItem);
        }
    }
}
