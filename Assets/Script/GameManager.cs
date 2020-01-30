﻿using UnityEngine;
using System.Collections;

using System.Collections.Generic;        //Allows us to use Lists. 
using Completed;

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

    private int currentTurn = 1;


    public MenuBar menuBar;

    public void notifyMenu(bool status, GameItem focus)
    {
        menuBar.notifyMenu(status, new Vector2(0, 0), focus);
        isMenuing = status;
    }

    public bool checkNextTurn()
    {
        bool allNext = true;
        playerUnits.ForEach(unit =>
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

    public GameItem getByXY(int x, int y) {
        GameItem target = null;
        target = boardScript.getByXY(x,y);
        playerUnits.ForEach(i => {
            if (i.x ==x && i.y ==y) {
                target = i;
            }
        });
        return target;
    }

    public void nextTurn()
    {
        currentTurn++;
        playerUnits.ForEach(unit =>
        {
            unit.isWaitNext = false;
        });
        print("GameManager: next turn - " + currentTurn);
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
        playerUnits.Clear();
        boardScript.SetupScene(level);
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
        return boardScript.getBlueLightActive(end);
    }

    public void callUnitMove(GameItem item, bool status)
    {
        isMoveing = status;
        Rigidbody2D rg2D = item.GetComponent<Rigidbody2D>();
        List<BoardManager.Light> lights = boardScript.getLightsCanGo(rg2D, item.movement);
        if (!status)
        {
            lights = boardScript.lights;
        }
        lights.ForEach(light =>
        {
            light.Object.SetActive(status);
        });
    }

}