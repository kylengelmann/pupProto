using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kick : Attack {
    
    Animator anim;

	protected override void Awake()
	{
        base.Awake();
        anim = GetComponent<Animator>();
	}

    protected override void onStartAttack() {
        anim.ResetTrigger("kickinTrig");
        anim.SetTrigger("kickinTrig");
    }

	public void __startHit(){
		startHit();
	}

	public void __endHit(){
		endHit();
	}

	public void __endAttack() {
		endAttack();
	}
}
