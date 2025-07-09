using Belmondo.ZeroInsertionForce;
using Raylib_cs;
using static Raylib_cs.Raylib;

internal sealed class Program
{
    private const double TICK_RATE_SECONDS = 1.0 / 30.0;
    private const int MAX_FRAME_SKIP = 30;

    private RaylibRenderer? _gameRenderer;
    private RaylibAudioPlayer? _audioPlayer;
    private InputManager? _inputManager;

    private static void Main() => new Program().Run();

    private Program() { }

    private void Run()
    {
        using var renderer = new RaylibRenderer();
        using var audioPlayer = new RaylibAudioPlayer();

        var commandSequence = new CommandSequence();

        _gameRenderer = renderer;
        _audioPlayer = audioPlayer;

        _inputManager = new()
        {
            IsActionPressedDelegate = IsInputActionPressed,
            GetMouseWorldPositionDelegate = renderer.GetMouseWorldPosition,
            HasRequestedQuitDelegate = () => WindowShouldClose(),
        };

        World world = new()
        {
            AudioPlayer = _audioPlayer,
            InputManager = _inputManager,
        };

        Game game = new()
        {
            World = world,
        };

        renderer.Initialize();
        RaylibAudioPlayer.Initialize();

        var isSimulating = false;
        var frame = 0;

        double previousTime = GetTime();
        double currentTime = default;
        double lag = default;

        _inputManager.InputActionPressed += inputAction =>
        {
            if (inputAction == InputActionType.Replay)
            {
                world.Reset();
                isSimulating = true;
                frame = 0;
                return;
            }

            if (!isSimulating)
            {
                commandSequence.Record(frame, () =>
                {
                    _inputManager.SimulateInputAction(inputAction);
                });
            }
        };

        while (!_inputManager.HasRequestedQuit())
        {
            currentTime = GetTime();

            double deltaTime = currentTime - previousTime;

            lag += deltaTime;
            lag = Math.Min(lag, MAX_FRAME_SKIP * TICK_RATE_SECONDS);
            previousTime = currentTime;

            _inputManager.Update(isSimulating);

            while (lag >= TICK_RATE_SECONDS)
            {
                if (isSimulating)
                {
                    commandSequence.TryExecuteAllAt(frame);
                }

                world.Update(TICK_RATE_SECONDS);
                lag -= TICK_RATE_SECONDS;
                frame += 1;
            }

            _gameRenderer.Render(in game, deltaTime);
        }
    }

    private static bool IsInputActionPressed(InputActionType inputAction) => inputAction switch
    {
        InputActionType.MoveUp => IsKeyDown(KeyboardKey.W),
        InputActionType.MoveRight => IsKeyDown(KeyboardKey.D),
        InputActionType.MoveDown => IsKeyDown(KeyboardKey.S),
        InputActionType.MoveLeft => IsKeyDown(KeyboardKey.A),
        InputActionType.AttackPrimary => IsKeyDown(KeyboardKey.Space),
        InputActionType.SpeedChange => IsKeyDown(KeyboardKey.LeftShift) || IsKeyDown(KeyboardKey.RightShift),
        InputActionType.Replay => IsKeyPressed(KeyboardKey.Backspace),
        _ => false,
    };
}
