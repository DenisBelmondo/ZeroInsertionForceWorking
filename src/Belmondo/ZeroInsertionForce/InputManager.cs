using System.Numerics;

namespace Belmondo.ZeroInsertionForce;

public sealed class InputManager
{
    public event Action<InputActionType>? InputActionPressed;

    private readonly Queue<InputActionType> _inputQueue = [];
    private readonly bool[] _pressedActions = new bool[Enum.GetValues<InputActionType>().Length];

    public required Func<InputActionType, bool> IsActionPressedDelegate;
    public required Func<Vector2> GetMouseWorldPositionDelegate;
    public required Func<bool> HasRequestedQuitDelegate;
    public Vector2 MoveAxes;
    public Vector2 MouseWorldPosition;

    public void SimulateInputAction(InputActionType inputActionType)
    {
        _pressedActions[(int)inputActionType] = true;
        InputActionPressed?.Invoke(inputActionType);
    }

    public void Update(bool isSimulating)
    {
        _inputQueue.Clear();

        if (!isSimulating)
        {
            foreach (var ia in Enum.GetValues<InputActionType>())
            {
                bool isActionPressed = IsActionPressedDelegate.Invoke(ia);

                if (isActionPressed)
                {
                    _inputQueue.Enqueue(ia);
                }

                _pressedActions[(int)ia] = isActionPressed;
            }
        }

        var rightIsPressed = IsActionPressed(InputActionType.MoveRight);
        var leftIsPressed = IsActionPressed(InputActionType.MoveLeft);
        var downIsPressed = IsActionPressed(InputActionType.MoveDown);
        var upIsPressed = IsActionPressed(InputActionType.MoveUp);
        var horizontalAxis = Convert.ToSingle(rightIsPressed) - Convert.ToSingle(leftIsPressed);
        var verticalAxis = Convert.ToSingle(downIsPressed) - Convert.ToSingle(upIsPressed);

        MoveAxes = new(horizontalAxis, verticalAxis);

        if (GetMouseWorldPositionDelegate is not null)
        {
            MouseWorldPosition = GetMouseWorldPositionDelegate.Invoke();
        }

        foreach (var ia in _inputQueue)
        {
            InputActionPressed?.Invoke(ia);
        }
    }

    public bool IsActionPressed(InputActionType inputAction) => _pressedActions[(int)inputAction];
    public bool HasRequestedQuit() => HasRequestedQuitDelegate();
}
