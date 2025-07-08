namespace Belmondo;

class Singleton<T> where T : new()
{
    private static T? _instance;

    public static T Instance
    {
        get
        {
            _instance ??= new();
            return _instance;
        }

        private set => _instance = value;
    }
}
