using System.Collections;
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
                OnHoverButton(++mCurrentPos % buttons.Length);
            } else if (code == KeyCode.UpArrow) {
                OnHoverButton(--mCurrentPos % buttons.Length);
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
        if (hoverButton == null) {hoverButton = mButtons[0];}
        Image hoverButtonImage = hoverButton.GetComponent<Image>();
        hoverButtonImage.color = Color.white;

        hoverButton = mButtons[pos];
        hoverButtonImage = hoverButton.GetComponent<Image>();
        hoverButtonImage.color = Color.black;
    }

    private List<MenuButton> getUsefulMenuButton(GameItem focus){
        
        List<MenuButton> menuButtons = new List<MenuButton>();
        for (int x = 0; x < buttons.Length; x++) { 
            MenuButton mb = buttons[x].GetComponent<MenuButton>();
             
            if (focus != null) {  
                if (focus.isOpenButton(mb.buttonCode)) { 
                    menuButtons.Add(mb);
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
                instance.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0 -x * 20);
                mButtons.Add(instance);
            }
            OnHoverButton(mCurrentPos);
        }
}
