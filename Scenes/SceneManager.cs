using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;
using DiceMG.Scenes;

namespace DiceMG;

public class SceneManager
{
    public static Scene CurrentScene { get; private set; }

    public static Dictionary<GameState, Scene> SceneDict { get; set; }
    public static GameState CurrentState { get; set; }

    public SceneManager()
    {
        SceneDict = new Dictionary<GameState, Scene>();
        Subscribe(Core.GM);
    }

    public SceneManager(Dictionary<GameState, Scene> scenes)
    {
        SceneDict = scenes;
        Subscribe(Core.GM);
    }

    /// <summary>
    /// Transitions to a new scene, disposing the current one.
    /// </summary>
    public static void ChangeScene(Scene newScene)
    {
        CurrentScene?.Dispose();
        GC.Collect();
        CurrentScene = newScene;
        CurrentScene?.Initialize();
    }

    /// <summary>
    /// Transitions to the scene mapped to a game state in the dictionary.
    /// </summary>
    public static void ChangeScene(GameState state)
    {
        if (SceneDict.TryGetValue(state, out Scene scene))
            ChangeScene(scene);
    }

    public static Scene GetScene(GameState state) => SceneDict[state];

    public static void AddScene(GameState state, Scene scene)
    {
        SceneDict[state] = scene;
    }

    public static void RemoveScene(GameState state)
    {
        SceneDict.Remove(state);
    }

    public static void ClearScenes()
    {
        SceneDict.Clear();
    }

    public void SetState(GameState state) => CurrentState = state;
    public static GameState GetState() => CurrentState;

    public void Subscribe(GameManager gm)
    {
        gm.StateChangedHandler += HandleStateChange;
    }

    private void HandleStateChange(object sender, GameStateArgs e)
    {
        SetState(e.NewState);
    }
}
