using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject
{
    public int playerDamage;

    private Animator animator;
    private Transform target;
    private bool skipMove;

    public override bool OnKeyDownEvent(KeyCode code) {
        return false;
    }
    protected override void Start()
    {
        GameManager.instance.AddUnitToList(GetComponent<GameItem>());
        animator = GetComponent<Animator>(); 
        base.Start();
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void AttemptMove <T> (int xDir, int yDir)
    { 
    }
 

    protected override void OnCantMove<T>(T component)
    {
        //Declare hitPlayer and set it to equal the encountered component.
        Player hitPlayer = component as Player;

        //Call the LoseFood function of hitPlayer passing it playerDamage, the amount of foodpoints to be subtracted.
        hitPlayer.LoseFood(playerDamage);

        //Set the attack trigger of animator to trigger Enemy attack animation.
        animator.SetTrigger("EnemyHit");

    }
}
