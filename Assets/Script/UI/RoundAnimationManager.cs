using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundAnimationManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Text roundText;
    public Image backgroundImage;

    public int textStartX =0;
    public int textEndX = 0;

    public int time1 = 0;
    public int time2 = 0;
    public int time3 = 0;
    public Text currentTurnText;

    public void play(int turn) {
        roundText.text = "Round " + turn;
        currentTurnText.text = "Round: " + turn;

        StartCoroutine(PlayAnimation());
    }

    protected IEnumerator PlayAnimation() {
        backgroundImage.gameObject.SetActive(true);
        roundText.gameObject.SetActive(true);
        int a = 0;
        RectTransform backgroundImageTransform = backgroundImage.GetComponent<RectTransform>();
        int width = 744;
        int textWidth1 = textEndX;
        int textWidth2 =  0-textStartX;
        while (a++ < time1) {
           roundText.GetComponent<RectTransform>().anchoredPosition = new Vector3(textStartX + (a/time1*textWidth2), 0, 0);
           
           backgroundImageTransform.anchoredPosition = new Vector3(width * -1 + (a * width/time1), 0, 0);
           yield return null;
        }
        while (a++ < time1 + time2) { 
           yield return null;
        } 
        while (a++ <  time1 + time2 + time3) { 
           roundText.GetComponent<RectTransform>().anchoredPosition = new Vector3(0 + (a/time3*textWidth1), 0, 0);
           
           backgroundImageTransform.anchoredPosition = new Vector3(width * -1 + (a * width/time3), 0, 0);
           yield return null;
        }
        backgroundImage.gameObject.SetActive(false);
        roundText.gameObject.SetActive(false);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
