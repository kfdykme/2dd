using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSystem : MonoBehaviour
{
    public EventableBehaviour[] eventCusomers;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void doEvent(KeyCode code) {
        bool isDone = false;
        int index = 0;
        while(!isDone && index < eventCusomers.Length) { 
            isDone = eventCusomers[index++].OnKeyDownEvent(code);

            if (isDone) {

                // print("event cent :" + (index-1));
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
          
        if (Input.GetKeyDown(KeyCode.A)) {
            doEvent(KeyCode.A);
        } else if (Input.GetKeyDown(KeyCode.B)) {
            doEvent(KeyCode.B);
        } else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            doEvent(KeyCode.LeftArrow);
        } else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
           doEvent(KeyCode.RightArrow);
        } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            doEvent(KeyCode.DownArrow);
        } else if (Input.GetKeyDown(KeyCode.UpArrow)){
           doEvent(KeyCode.UpArrow);
        }
 
    }
}
