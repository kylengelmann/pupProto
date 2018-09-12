using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class contextInput {
	
	inputHandler handler;
	
	public contextInput(inputHandler handler)
	{
		this.handler = handler;
	}
	
	public float getAxis(inputTypes.axisType axis)
	{
		return handler.axes[axis];
	}
	
	public bool getButtonDown(inputTypes.buttonType butt)
	{
		return handler.butts[butt].isDown;
	}
	
	public bool getButtonPressed(inputTypes.buttonType butt)
	{
		return handler.butts[butt].isPressed;
	}
	
	public bool getButtonUp(inputTypes.buttonType butt)
	{
		return handler.butts[butt].isUp;
	}
	
	public safeAction onCheckInput = new safeAction();
}
