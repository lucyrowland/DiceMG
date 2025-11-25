using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DiceMG.Scenes;

namespace DiceMG;

public class SceneManager
{
    public static Dictionary<GameState, Scene> SceneDict { get; set; }
    public static GameState CurrentState { get; set; }
    
    public SceneManager()
    {
        SceneDict = new Dictionary<GameState, Scene>();
        Subscribe(Core.GM);
    }

    public SceneManager(Dictionary<GameState, Scene> scenes)
    {
        scenes.Add(GameState.NewGame, new TitleScene());
        SceneDict = scenes;
        Subscribe(Core.GM);
    }
    
    public static Scene GetScene(GameState state) => SceneDict[state];
    
    public static void ChangeScene(GameState state)
    {
        Core.ChangeScene(SceneDict[state]);
        Core.TransitionScene();
        
    }
    public static void AddScene(GameState state, Scene scene)
    {
        SceneDict.Add(state, scene);
    }

    public static void RemoveScene(GameState state)
    {
        SceneDict.Remove(state);
        Core.TransitionScene();
    }
    public static void ClearScenes()
    {
        SceneDict.Clear();
    }
    public void SetState(GameState state) => CurrentState = state;
    public static GameState GetState() => CurrentState;
    
    
    public void Subscribe(GameManager GM)
    {
        GM.StateChangedHandler += HandleStateChange;
    }
   
    // The handler method signature must match the delegate: (object sender, T args)
    private void HandleStateChange(object sender, GameStateArgs e)
    {
        SetState(e.NewState);
    }

}