using System.Numerics;

namespace Belmondo.ZeroInsertionForce;

public class InputManager
{
    public event Action<InputActionType>? InputActionPressed;

    private readonly Queue<InputActionType> _inputQueue = [];
    private readonly bool[] _pressedActions = new bool[Enum.GetValues<InputActionType>().Length];

    public required Func<InputActionType, bool> IsActionPressedDelegate;
    public required Func<Vector2> GetMouseWorldPositionDelegate;
    public required Func<bool> HasRequestedQuitDelegate;
    public Vector2 MoveAxes;
    public Vector2 MouseWorldPosition;

    public void Update()
    {
        _inputQueue.Clear();

        foreach (var ia in Enum.GetValues<InputActionType>())
        {
            bool isActionPressed = IsActionPressedDelegate.Invoke(ia);

            if (isActionPressed)
            {
                _inputQueue.Enqueue(ia);
            }

            _pressedActions[(int)ia] = isActionPressed;
        }

        var rightIsPressed = IsActionPressedDelegate.Invoke(InputActionType.MoveRight);
        var leftIsPressed = IsActionPressedDelegate.Invoke(InputActionType.MoveLeft);
        var downIsPressed = IsActionPressedDelegate.Invoke(InputActionType.MoveDown);
        var upIsPressed = IsActionPressedDelegate.Invoke(InputActionType.MoveUp);
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
