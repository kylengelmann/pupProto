using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager current;

    private inputHandler inputHandler;
    
    public gameContext context;

    void Awake()
    {
        current = this;
        inputHandler = GetComponent<inputHandler>();
        context = new gameContext(inputHandler);
        inputHandler.onCheckInput += () =>
        {
            context.input.onCheckInput.Invoke();
        };
    }
}
