using System.Collections.Generic;
using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using DiceMG.Input;

namespace DiceMG;

public enum GameState
{
    GameOver, 
    NewGame, 
    NewRound, 
    Playing, 
    Paused,
    MainMenu, 
    Rules,
    GameWon
}
public enum ScoreType {Total, Round, Hand}


public enum TurnProcedure { Player1, Player2 }

public class GameManager
{
    
    public List<Player> Players = new List<Player>();
    //public static GameState State;
    //public static TurnProcedure CurrentTurn;
    public EventHandler<GameStateArgs> StateChangedHandler; 
    

    public GameManager(List<Player> players)
    {
        Players = players; 
        
    }

    public GameManager()
    {

    }
    public void ChangeState(GameState newState)
    {
        //State = newState;
        StateChangedHandler?.Invoke(this, new GameStateArgs(newState));
    }
    public void AddRoundScore(Player player) => player.Scores.AddToRoundScore(player.HeldDice);
    public void AddTotalScore(Player player) => player.Scores.AddToTotalScore();

    public string ScoreText(Player player, ScoreType type)
    {
            
            
        if (type == ScoreType.Total) return $"{player.Scores.TotalScore}";
        if (type == ScoreType.Round) return $"{player.Scores.RoundScore}";

        return $"{player.Scores.GetTempScore(player.DiceList)}";

    }



    public void NextRound()
    {
    }

    public void SkipTurn()
    {
    }

    public void FlopTurn()
    {
    }
    public void Update(GameTime gameTime)
    {
        foreach (Player player in Players)
        {
            foreach(Dice die in player.DiceList) 
                ClickedDice(die);
            
        }
           
    }

    private void ClickedDice(Dice die)
    {
        if (Core.Input.Mouse.ButtonPressed(MouseButton.Left))
        {
                
            if (die.Box.Contains(Core.Input.Mouse.Position))
            {
                if (die.State != DieState.held)
                    die.State = DieState.held;
                else
                    die.State = DieState.free;
            }
            Console.WriteLine($"Die Value: {die.Value}, Die State: {die.State}");
        }
    }
    
}

public class GameStateArgs: EventArgs
{
    public GameState NewState { get; set; }

    public GameStateArgs(GameState newState)
    {
        NewState = newState;
    }
}