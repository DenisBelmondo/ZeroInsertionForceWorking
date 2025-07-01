using System.Numerics;

namespace Belmondo.ZeroInsertionForce;

public class InputManager
{
    public event Action<InputAction>? InputActionPressed;

    private readonly Queue<InputAction> _inputQueue = [];
    private readonly bool[] _pressedActions = new bool[Enum.GetValues<InputAction>().Length];

    public required Func<InputAction, bool> IsActionPressedDelegate;
    public required Func<Vector2> GetMouseWorldPositionDelegate;
    public required Func<bool> HasRequestedQuitDelegate;
    public Vector2 MoveAxes;
    public Vector2 MouseWorldPosition;

    public void Update()
    {
        _inputQueue.Clear();

        foreach (var ia in Enum.GetValues<InputAction>())
        {
            bool isActionPressed = IsActionPressedDelegate.Invoke(ia);

            if (isActionPressed)
            {
                _inputQueue.Enqueue(ia);
            }

            _pressedActions[(int)ia] = isActionPressed;
        }

        var rightIsPressed = IsActionPressedDelegate.Invoke(InputAction.MoveRight);
        var leftIsPressed = IsActionPressedDelegate.Invoke(InputAction.MoveLeft);
        var downIsPressed = IsActionPressedDelegate.Invoke(InputAction.MoveDown);
        var upIsPressed = IsActionPressedDelegate.Invoke(InputAction.MoveUp);
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

    public bool IsActionPressed(InputAction inputAction) => _pressedActions[(int)inputAction];
    public bool HasRequestedQuit() => HasRequestedQuitDelegate();
}
