﻿public class abilitiesManager {

	characterEventHandler events;
	public void init(characterEventHandler events)
	{
		this.events = events;
		events.dash.onDashStart += onDashStart;
		events.dash.onDashEnd += onDashEnd;
        events.physicsAnimation.onEnable += onPhysicsAnimate;
        events.physicsAnimation.onDisable += onStopPhysicsAnimate;
	}
	
	void onDashStart()
	{
		events.move.setActive.Invoke(false);
		events.jump.setActive.Invoke(false);
		events.wall.setActive.Invoke(false);
	}
	
	void onDashEnd()
	{
		events.move.setActive.Invoke(true);
		events.jump.setActive.Invoke(true);
		events.wall.setActive.Invoke(true);
	}

    void onPhysicsAnimate()
    {
        events.move.setActive.Invoke(false);
        events.jump.setActive.Invoke(false);
        events.wall.setActive.Invoke(false);
        events.dash.setAcive.Invoke(false);
    }

    void onStopPhysicsAnimate()
    {
        events.move.setActive.Invoke(true);
        events.jump.setActive.Invoke(true);
        events.wall.setActive.Invoke(true);
        events.dash.setAcive.Invoke(true);
    }
}