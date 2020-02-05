using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton : MonoBehaviour
{
    public string buttonName;

    public int buttonCode;
    // Start is called before the first frame update
    public static int CODE_MOVE = 0;
    public static int CODE_WAIT = 1;

    public static int CODE_NEXT_TURN = 9;
    public void  onClick() {
        if (buttonCode == CODE_MOVE) {
            CursorP.instance.callUnitMove();
        } else if (buttonCode == CODE_WAIT) {
            CursorP.instance.callUnitWait();
        } else if (buttonCode == CODE_NEXT_TURN) {
            GameManager.instance.nextTurn();
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
