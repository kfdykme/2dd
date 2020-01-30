using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EventableBehaviour : MonoBehaviour
{
    public abstract bool OnKeyDownEvent(KeyCode code) ;
}
