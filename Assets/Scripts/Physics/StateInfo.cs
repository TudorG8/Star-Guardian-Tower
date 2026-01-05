using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPropertyDrawers;

[System.Serializable]
public class StateInfo {
	public enum State {
		CanDoAction, DoingAction, CantDoAction, Waiting
	}
	[System.Serializable]
	public class RoutineState {
		[SerializeField][ReadOnly] State state;
		[SerializeField] Coroutine routine;

		public State     GetState { get { return state  ; } set { state   = value;} }
		public Coroutine Routine  { get { return routine; } set { routine = value;} }

		public void Reset(MonoBehaviour parent) {
			state = State.CanDoAction;
			if (routine != null) {
				parent.StopCoroutine (routine);
			}
		}
	}
	[SerializeField] RoutineState attacking    ;
	[SerializeField] RoutineState jumping      ;
	[SerializeField] RoutineState dashing      ;
	[SerializeField] RoutineState takingDamage ;
	[SerializeField] RoutineState wallSliding  ;
	[SerializeField] RoutineState holdingOnEdge;

	public RoutineState Attacking      { get { return attacking    ; } set { attacking     = value;} }
	public RoutineState Jumping        { get { return jumping      ; } set { jumping       = value;} }
	public RoutineState Dashing        { get { return dashing      ; } set { dashing       = value;} }
	public RoutineState TakingDamage   { get { return takingDamage ; } set { takingDamage  = value;} }
	public RoutineState WallSliding    { get { return wallSliding  ; } set { wallSliding   = value;} }
	public RoutineState HoldingOnEdge  { get { return holdingOnEdge; } set { holdingOnEdge = value;} }

	public void Reset(MonoBehaviour monobehaviour) {
		attacking    .Reset (monobehaviour);
		jumping      .Reset (monobehaviour);
		dashing      .Reset (monobehaviour);
		takingDamage .Reset (monobehaviour);
		wallSliding  .Reset (monobehaviour);
		holdingOnEdge.Reset (monobehaviour);
	}
}