using System;

public class OneStat
{
    private int _value;

    public int Value
    {
        get => _value;
        set
        {
            _value = value;
            ValueChanged?.Invoke(_value);
        }
    }

    public event Action<int> ValueChanged;


    public OneStat(int value, string name = "")
    {
        _value = value;
    }
}