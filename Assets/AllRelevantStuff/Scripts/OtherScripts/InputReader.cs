public class InputReader
{
    private static PlayerControlInput _actions;

    public static PlayerControlInput Actions
    {
        get
        {
            if (_actions == null)
            {
                _actions = new PlayerControlInput();
                _actions.InGame.Enable();
            }
            return _actions;
        }
    }
}
