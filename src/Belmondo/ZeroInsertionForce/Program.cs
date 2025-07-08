using Belmondo.ZeroInsertionForce;
using Raylib_cs;
using static Raylib_cs.Raylib;

internal class Program
{
    private const double TICK_RATE_SECONDS = 1.0 / 30.0;
    private const int MAX_FRAME_SKIP = 30;

    private RaylibRenderer? _gameRenderer;
    private RaylibAudioPlayer? _audioPlayer;
    private InputManager? _inputManager;

    private static void Main() => new Program().Run();

    private void Run()
    {
        var renderer = new RaylibRenderer();
        var audioPlayer = new RaylibAudioPlayer();

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

        double previousTime = GetTime();
        double lag = default;

        while (!_inputManager.HasRequestedQuit())
        {
            double currentTime = GetTime();
            double deltaTime = currentTime - previousTime;

            lag += deltaTime;
            lag = Math.Min(lag, MAX_FRAME_SKIP * TICK_RATE_SECONDS);
            previousTime = currentTime;

            _inputManager.Update();

            while (lag >= TICK_RATE_SECONDS)
            {
                world.Update(TICK_RATE_SECONDS);
                lag -= TICK_RATE_SECONDS;
            }

            _gameRenderer.Render(in game, deltaTime);
        }

        audioPlayer.Dispose();
        renderer.Dispose();
    }

    private static bool IsInputActionPressed(InputActionType inputAction) => inputAction switch
    {
        InputActionType.MoveUp => IsKeyDown(KeyboardKey.W),
        InputActionType.MoveRight => IsKeyDown(KeyboardKey.D),
        InputActionType.MoveDown => IsKeyDown(KeyboardKey.S),
        InputActionType.MoveLeft => IsKeyDown(KeyboardKey.A),
        InputActionType.AttackPrimary => IsKeyDown(KeyboardKey.Space),
        InputActionType.SpeedChange => IsKeyDown(KeyboardKey.LeftShift) || IsKeyDown(KeyboardKey.RightShift),
        _ => false,
    };
}
