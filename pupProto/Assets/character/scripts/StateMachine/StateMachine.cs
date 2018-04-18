using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine {
	public StateNode currentNode;
	//protected Dictionary<string, int> ints;
	//protected Dictionary<string, bool> bools;
	//protected Dictionary<string, float> floats;
	protected Hashtable variables;

	public StateNode AnyState {get; protected set;}

	public StateMachine() {
		variables = new Hashtable();
		AnyState = new StateNode();
		AnyState.stateMachine = this;
	}

	public void addNode(StateNode node) {
		if(currentNode == null) {
			currentNode = node;
		}
		node.stateMachine = this;
	}

	public void setValue(string name, System.Object v) {
		if(variables.ContainsKey(name)) {
			variables[name] = v;
		}
		else {
			variables.Add(name, v);
		}
	}

	public T getValue<T>(string name) {
		return (T)variables[name];
	}

	public void Tick() {
		StateNode next = AnyState.findNext();
		if(!currentNode) return;
		if(!next) {
			next = currentNode.findNext();
		}
		if(next) {
			currentNode.onExit();
			next.onEnter();
			currentNode = next;
			return;
		}
	}
}

public class StateTransition {
	public StateNode current;
	public StateNode next;
	public int priority;
	protected List<Conditions.Condition> conditions;

	public StateTransition(StateNode next, int priority = 0) {
		conditions = new List<Conditions.Condition>();
		this.next = next;
		this.priority = priority;
	}

	public void addCondition(Conditions.Condition condition) {
		condition.stateTransition = this;
		conditions.Add(condition);
	}

	public bool Evaluate() {
		bool result = true;
		foreach(Conditions.Condition c in conditions) {
			result &= c.Evaluate();
		}
		return result;
	}

	public static implicit operator bool(StateTransition n) {
		return n != null;
	}

	public virtual void onTransition() {}
}

public class StateNode {
	public StateMachine stateMachine;
	protected List<StateTransition> transitions;

	public StateNode() {
		transitions = new List<StateTransition>();
	}

	public void addTransition(StateTransition transition) {
		transition.current = this;
		transitions.Add(transition);
	}

	public StateNode findNext() {
		StateNode next = null;
		StateTransition tran = null;
		int maxPriority = -1;
		foreach(StateTransition t in transitions) {
			if(t.priority < maxPriority) continue;
			if(t.Evaluate()) {
				maxPriority = t.priority;
				next = t.next;
				tran = t;
			}
		}
		if(tran) {
			tran.onTransition();
		}
		return next;
	}

	public static implicit operator bool(StateNode n) {
		return n != null;
	}

	public virtual void onEnter() {}
	public virtual void onExit() {}
}

namespace Conditions {
	public abstract class Condition {
		public StateTransition stateTransition;

		public abstract bool Evaluate();
	}

	public class Value<ValueType> : Condition {
		Func<ValueType, bool> func;
		public string value;
		public ValueType constant;
		public Value(Func<ValueType, bool> f, string value) {
			func = f;
			this.value = value;
		}

		public override bool Evaluate() {
			return func(stateTransition.current.stateMachine.getValue<ValueType>(value));
		}
	}

	public class ValueValue<T1, T2> : Condition {
		Func<T1, T2, bool> func;
		public string value1;
		public string value2;
		public ValueValue(Func<T1, T2, bool> f, string value1, string value2) {
			func = f;
			this.value1 = value1;
			this.value2 = value2;
		}

		public override bool Evaluate() {
			return func(stateTransition.current.stateMachine.getValue<T1>(value1), 
			            stateTransition.current.stateMachine.getValue<T2>(value2));
		}
	}	
}

