using System;

public class characterEventHandler {
    public moveEvents move = new moveEvents();
    public dashEvents dash = new dashEvents();
    public wallEvents wall = new wallEvents();
    public jumpEvents jump = new jumpEvents();
    public characterEvents character = new characterEvents();
    public combatEvents combat = new combatEvents();
}


public class safeAction
{
    Action action;

    public void Invoke()
    {
        if(action != null) action();
    }

    public static safeAction operator+ (safeAction sa, Action a)
    {
        sa.action += a;
        return sa;
    }

    public static safeAction operator- (safeAction sa, Action a)
    {
        sa.action -= a;
        return sa;
    }
}

public class safeAction<T>
{
    Action<T> action;

    public void Invoke(T arg)
    {
        if (action != null) action(arg);
    }

    public static safeAction<T> operator +(safeAction<T> sa, Action<T> a)
    {
        sa.action += a;
        return sa;
    }

    public static safeAction<T> operator -(safeAction<T> sa, Action<T> a)
    {
        sa.action -= a;
        return sa;
    }
}

public class safeAction<T1, T2>
{
    Action<T1, T2> action;

    public void Invoke(T1 arg1, T2 arg2)
    {
        if (action != null) action(arg1, arg2);
    }

    public static safeAction<T1, T2> operator +(safeAction<T1, T2> sa, Action<T1, T2> a)
    {
        sa.action += a;
        return sa;
    }

    public static safeAction<T1, T2> operator -(safeAction<T1, T2> sa, Action<T1, T2> a)
    {
        sa.action -= a;
        return sa;
    }
}
