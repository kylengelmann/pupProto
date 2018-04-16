using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombo : Combo {
	public Attack standard;
	public Attack endCombo;

	// Use this for initialization
	protected override void Start () {
		base.Start();
		AttackNode node1 = new AttackNode(standard);
		AttackNode node2 = new AttackNode(standard);
		AttackNode node3 = new AttackNode(endCombo);
		StateTransition t12 = new StateTransition(node2);
		t12.addCondition(new Conditions.Value<AttackDir>((AttackDir a) => {return a == AttackDir.none;}, "queued"));
		StateTransition t23 = new StateTransition(node3);
		t23.addCondition(new Conditions.Value<AttackDir>((AttackDir a) => {return a == AttackDir.none;}, "queued"));
		StateTransition t31 = new StateTransition(node1);
		t31.addCondition(new Conditions.Value<AttackDir>((AttackDir a) => {return a == AttackDir.none;}, "queued"));
		node1.addTransition(t12);
		node2.addTransition(t23);
		node3.addTransition(t31);
		stateMachine.addNode(node1);
		stateMachine.addNode(node2);
		stateMachine.addNode(node3);
		stateMachine.setValue("reset", false);
		StateTransition reset = new StateTransition(node1);
		reset.addCondition(new Conditions.Value<bool>((bool b) => {return b;}, "reset"));
		stateMachine.AnyState.addTransition(reset);
	}
}
