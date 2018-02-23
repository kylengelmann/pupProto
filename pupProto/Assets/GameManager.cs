using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager gameManager;

    public struct GameButtons {
        public string xMove;
        public string yMove;
        public string xDash;
        public string yDash;
        public string jump;
        public string attack;
        public string pause;
    }
    public static GameButtons gameButtons;


    void Awake()
    {
        gameManager = this;

        string platform = "_win";
        if(Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer) {
            platform = "_mac";
        }

        int numControllers = Input.GetJoystickNames().Length;
        string controllerType = "_key";
        if(numControllers > 0) {
            controllerType = "_xbox";
        }
        else {
            platform = "";
        }

        gameButtons.xMove = "Horizontal" + platform + controllerType;
        gameButtons.yMove = "Vertical" + platform + controllerType;
        gameButtons.xDash = "dashHorizontal" + platform + controllerType;
        gameButtons.yDash = "dashVertical" + platform + controllerType;
        gameButtons.jump = "jump" + platform + controllerType;
        gameButtons.attack = "attack" + platform + controllerType;
        gameButtons.pause = "pause" + platform + controllerType;
    }

}
