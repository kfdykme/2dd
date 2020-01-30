using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMoveManager : MonoBehaviour
{
    public static UnitMoveManager instance = null;

    public CursorP cursorP; 

    
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

   
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
