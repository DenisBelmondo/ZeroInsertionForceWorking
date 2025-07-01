using Belmondo.ZeroInsertionForce;
using Raylib_cs;
using static Raylib_cs.Raylib;

internal class Program
{
    private const double TICK_RATE_SECONDS = 1.0 / 30.0;
    private const int MAX_FRAME_SKIP = 30;

    private static void Main()
    {
        RaylibRenderer renderer = new();
        RaylibAudioPlayer audioPlayer = new();

        InputManager inputManager = new()
        {
            IsActionPressedDelegate = IsInputActionPressed,
            GetMouseWorldPositionDelegate = renderer.GetMouseWorldPosition,
            HasRequestedQuitDelegate = () => WindowShouldClose(),
        };

        World world = new()
        {
            InputManager = inputManager,
            AudioPlayer = audioPlayer,
        };

        renderer.Initialize();
        RaylibAudioPlayer.Initialize();

        double previousTime = GetTime();
        double lag = default;

        while (!inputManager.HasRequestedQuit())
        {
            double currentTime = GetTime();
            double deltaTime = currentTime - previousTime;

            lag += deltaTime;
            previousTime = currentTime;

            inputManager.Update();

            while (lag >= TICK_RATE_SECONDS)
            {
                world.Update(TICK_RATE_SECONDS);
                lag -= TICK_RATE_SECONDS;
            }

            renderer.Render(in world, deltaTime);
        }

        audioPlayer.Dispose();
        renderer.Dispose();
    }

    private static bool IsInputActionPressed(InputAction inputAction) => inputAction switch
    {
        InputAction.MoveUp => IsKeyDown(KeyboardKey.W),
        InputAction.MoveRight => IsKeyDown(KeyboardKey.D),
        InputAction.MoveDown => IsKeyDown(KeyboardKey.S),
        InputAction.MoveLeft => IsKeyDown(KeyboardKey.A),
        InputAction.AttackPrimary => IsKeyDown(KeyboardKey.Space),
        InputAction.SpeedChange => IsKeyDown(KeyboardKey.LeftShift) || IsKeyDown(KeyboardKey.RightShift),
        _ => false,
    };
}
