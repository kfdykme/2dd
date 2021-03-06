﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MenuBar : EventableBehaviour
{
    public GameObject[] buttons;
 
    public GameObject hoverButton;
    
    private Transform menuHolder;

    

    private List<GameObject> mButtons = new List<GameObject>();
    private int mCurrentPos = 0;
    // Start is called before the first frame update
    void Start()
    { 

    }
    

    public override bool OnKeyDownEvent(KeyCode code){
        if (!gameObject.active) {
            return false;
        } else {
            if (code == KeyCode.DownArrow) {
                if (mCurrentPos +1< mButtons.Count)
                    mCurrentPos++;
                OnHoverButton(mCurrentPos);
            } else if (code == KeyCode.UpArrow) {
                if (mCurrentPos > 0)
                    mCurrentPos--;
                
                OnHoverButton(mCurrentPos);
            } else if (code == KeyCode.A) {
                mButtons[mCurrentPos].GetComponent<MenuButton>().onClick();
                notifyMenuFalse();
            } else if (code == KeyCode.B) {
                notifyMenuFalse();
            }
            return true;
        }
    }
    private void notifyMenuFalse() {
        GameManager.instance.notifyMenu(false, null);
    }
    public void notifyMenu(bool status, Vector2 source, GameItem focus) {
        mCurrentPos = 0;
        
        OnHoverButton(mCurrentPos);
        print("MenuBar: On notifyMenu");
        Vector2 end = source + new Vector2(1,1); 
        GetComponent<RectTransform>().anchoredPosition = end;
        gameObject.SetActive(status); 
        if (getUsefulMenuButton(focus).Count > 0 ) {
            if (status) {
                MenuSetup(focus);
            }
        }
    }
 

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnHoverButton(int pos) {
        //1 reset current hovered button
        if (pos <0 || pos >= mButtons.Count)
        return;

        if (hoverButton == null) {hoverButton = mButtons[0]; return;}
        
        Image hoverButtonImage = hoverButton.GetComponent<Image>();
        hoverButtonImage.color = Color.white;

        hoverButton = mButtons[pos]; 
        hoverButtonImage = hoverButton.GetComponent<Image>();
        Color outColor; 
        ColorUtility.TryParseHtmlString("#00bcd4", out  outColor);
        hoverButtonImage.color = outColor;
    }

    private List<MenuButton> getUsefulMenuButton(GameItem focus){
        
        List<MenuButton> menuButtons = new List<MenuButton>();
        for (int x = 0; x < buttons.Length; x++) { 
            MenuButton mb = buttons[x].GetComponent<MenuButton>();
            
            if (focus != null) {  
                if (focus.isOpenButton(mb.buttonCode)) { 
                    menuButtons.Add(mb);
                    print("MenuBar add button :" + mb.buttonName + "/" + mb.buttonCode);
                }
            } else {
                if (GameItem.isOpenNullButton(mb.buttonCode)) {
                    menuButtons.Add(mb);
                }
            }
        }    
        return menuButtons;
    }

    void MenuSetup(GameItem focus)
        { 
           
            List<MenuButton> menuButtons = getUsefulMenuButton(focus);
           
            mButtons.ForEach(o => {
                Destroy(o);
            });
             mButtons.Clear();
            menuHolder = this.transform;

            for (int x = 0; x < menuButtons.Count; x++)
            {
                GameObject toInstantiate = menuButtons[x].gameObject;

                GameObject instance = Instantiate(toInstantiate,
                 new Vector3(0,0, 0f),
                 Quaternion.identity);
                instance.transform.SetParent(menuHolder); 
                instance.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0 -x * 25);
                mButtons.Add(instance);
            }
            OnHoverButton(mCurrentPos);
        }
}
