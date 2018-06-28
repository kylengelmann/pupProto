using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace inputTypes
{
	public enum axisType
	{
		moveX,
		moveY,
		dashX,
		dashY
	}
	
	public enum buttonType
	{
		a,
		b,
		x,
		y
	}

}

public class inputHandler : MonoBehaviour {
	
	public safeAction onCheckInput = new safeAction();
	
	public struct button
	{
		public bool isDown;
		public bool isPressed;
		public bool isUp;
	}
	
	public Dictionary<inputTypes.axisType, float> axes = new Dictionary<inputTypes.axisType, float>
	{
		{inputTypes.axisType.moveX, 0f},
		{inputTypes.axisType.moveY, 0f},
		{inputTypes.axisType.dashX, 0f},
		{inputTypes.axisType.dashY, 0f}
	};
	
	public Dictionary<inputTypes.buttonType, button> butts = new Dictionary<inputTypes.buttonType, button>
	{
		{inputTypes.buttonType.a, new button()},
		{inputTypes.buttonType.b, new button()},
		{inputTypes.buttonType.x, new button()},
		{inputTypes.buttonType.y, new button()},
	};
	
	private Dictionary<inputTypes.buttonType, KeyCode> buttonKeyCodes;

    string platform = "";
	string controller = "_key";
	
	string axisSuffix = "_key";
	
	// Use this for initialization
	void Start () {
		RuntimePlatform plat = Application.platform;
		if(plat == RuntimePlatform.OSXPlayer || plat == RuntimePlatform.OSXEditor)
		{
			platform = "_mac";
		}
		else
		{
			platform = "_win";
		}
	}
	
	// Update is called once per frame
	void Update () {
		checkControllers();

		foreach (inputTypes.axisType type in axes.Keys.ToList())
		{
			axes[type] = Input.GetAxisRaw(axisNames[type] + axisSuffix);
		}

		foreach (inputTypes.buttonType type in butts.Keys.ToList())
		{
			butts[type] = new button
			{
				isDown = Input.GetKeyDown(buttonKeyCodes[type]),
				isPressed = Input.GetKey(buttonKeyCodes[type]),
				isUp = Input.GetKeyUp(buttonKeyCodes[type])
			};
		}
		
		onCheckInput.Invoke();
		
	}
	
	void checkControllers()
	{
		string[] controllers = Input.GetJoystickNames();
		if(controllers.Length > 1)
		{
			if(controllers[0].Contains("xbox"))
			{
				controller = "_xbox";
				if(platform == "_mac")
				{
					buttonKeyCodes = xboxMacButtons;
				}
				else if(platform == "_win")
				{
					buttonKeyCodes = xboxWinButtons;
				}
			}
			else
			{
				controller = "_key";
				buttonKeyCodes = keyboardButtons;
			}
		}
		else
		{
			controller = "_key";
			buttonKeyCodes = keyboardButtons;
		}
		axisSuffix = controller + (controller == "_key" ? "" : platform);
	}
	
	
	
	Dictionary<inputTypes.axisType, string> axisNames = new Dictionary<inputTypes.axisType, string>
	{
		{inputTypes.axisType.moveX, "Horizontal"},
		{inputTypes.axisType.moveY, "Vertical"},
		{inputTypes.axisType.dashX, "dashHorizontal"},
		{inputTypes.axisType.dashY, "dashVertical"}
	};

	Dictionary<inputTypes.buttonType, KeyCode> keyboardButtons = new Dictionary<inputTypes.buttonType, KeyCode>
	{
		{inputTypes.buttonType.a, KeyCode.Space},
		{inputTypes.buttonType.b, KeyCode.J},
		{inputTypes.buttonType.x, KeyCode.K},
		{inputTypes.buttonType.y, KeyCode.L},
	};
	
	Dictionary<inputTypes.buttonType, KeyCode> xboxMacButtons = new Dictionary<inputTypes.buttonType, KeyCode>
	{
		{inputTypes.buttonType.a, KeyCode.Space},
		{inputTypes.buttonType.b, KeyCode.J},
		{inputTypes.buttonType.x, KeyCode.K},
		{inputTypes.buttonType.y, KeyCode.L},
	};
	
	Dictionary<inputTypes.buttonType, KeyCode> xboxWinButtons = new Dictionary<inputTypes.buttonType, KeyCode>
	{
		{inputTypes.buttonType.a, KeyCode.Space},
		{inputTypes.buttonType.b, KeyCode.J},
		{inputTypes.buttonType.x, KeyCode.K},
		{inputTypes.buttonType.y, KeyCode.L},
	};
}
