using UnityEngine;
using UnityEngine.InputSystem;

public static class InputReader
{
    private static PlayerControlInput _actions;

    public static PlayerControlInput Actions
    {
        get
        {
            if (_actions == null)
            {
                _actions = new PlayerControlInput();
                LoadOverrides();
            }
            return _actions;
        }
    }

    private static void LoadOverrides()
    {
        var rebinds = PlayerPrefs.GetString("rebinds");
        if (!string.IsNullOrEmpty(rebinds))
            _actions.LoadBindingOverridesFromJson(rebinds);
    }

   
}
