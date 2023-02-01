using UnityEditor;

public static class FunctionRunner
{
    [MenuItem( "Edit/Run _F5", priority = 140 )]
    private static void Run()
    {
        EditorApplication.isPlaying = true;
    }

    [MenuItem( "Edit/Run _F5", validate = true )]
    private static bool CanRun()
    {
        return !EditorApplication.isPlaying;
    }

    [MenuItem( "Edit/Stop #_F5", priority = 141 )]
    private static void Stop()
    {
        EditorApplication.isPlaying = false;
    }

    [MenuItem( "Edit/Stop #_F5", validate = true )]
    private static bool CanStop()
    {
        return EditorApplication.isPlaying;
    }

    [MenuItem("Edit/Pause_ _F11", priority = 141)]
    private static void Pause()
    {
        // 1フレーム進める
        EditorApplication.isPaused = !EditorApplication.isPaused;
    }

    [MenuItem("Edit/Step_ _F12", priority = 141)]
    private static void Step()
    { 
        // 1フレーム進める
		EditorApplication.isPaused = true;
		EditorApplication.Step();
	}
}