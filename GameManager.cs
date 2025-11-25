using System.Collections.Generic;
using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

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



public enum TurnProcedure { Player1, Player2 }

public class GameManager
{
    public static Dictionary<int, int> Scores = new Dictionary<int, int>();
    public static Dictionary<int, Player> Players = new Dictionary<int, Player>();
    public static int Round; 
    public static GameState State;
    public static TurnProcedure CurrentTurn;
    public static int NumPlayers = 1;
    public Dictionary<string, int> LevelScores = new Dictionary<string, int>();
    public static string Level = "Short";
    public static int CurrentLevelScore;
    public static Player Winner = null;
    public static bool Multiplayer;
    public EventHandler<GameStateArgs> StateChangedHandler;
    
    public GameManager(int numPlayers, string level)
    {
        Multiplayer = true;
         CurrentTurn = TurnProcedure.Player1;
         NumPlayers = numPlayers;
         Round = 1; 
         ChangeState(GameState.NewGame);

         for (int i = 0; i < numPlayers; i++)
         {
             var player_name = $"Player {i + 1}";
             Players.Add(i+1, new Player(player_name, i+1));
             Scores.Add(i+1, 0);
             Debug.WriteLine($"Player {i+1} added");
         }
         
         GenerateLevels();
         Level = level; 
         CurrentLevelScore = LevelScores[level];
    }

    public GameManager()
    {
        Multiplayer = false;
        CurrentTurn = TurnProcedure.Player1;
        NumPlayers = 1;
        Round = 1; 
        ChangeState(GameState.NewGame);
        Players.Add(1, new Player("Player 1", 1));
        Scores.Add(1, 0);
        Debug.WriteLine($"Player 1 added");
        
        GenerateLevels();
        Level = "Short"; 
        CurrentLevelScore = LevelScores[Level];
    }
    public void ChangeState(GameState newState)
    {
        State = newState;
        StateChangedHandler?.Invoke(this, new GameStateArgs(newState));
    }

    public void GenerateLevels()
    {
        LevelScores.Add("Short", 1000);
        LevelScores.Add("Medium", 2000);
        LevelScores.Add("Long", 3000);
    }

    public void NextRound()
    {
        ChangeTurn();
        if (CurrentTurn == TurnProcedure.Player1) Round++;
        ScoringSystem.AddScore();
    }

    public void SkipTurn()
    {
        ChangeTurn();
        if (CurrentTurn == TurnProcedure.Player1) Round++;
        //ScoringSystem.NewRound();
        ScoringSystem.AddScore(); //need to add temp score and round score to scoring system
    }

    public void FlopTurn()
    {
        ChangeTurn();
        if (CurrentTurn == TurnProcedure.Player1) Round++;
        //ScoringSystem.NewRound(); to reset tempscore and roundscore to 0 
    }
    
    public static void ChangeTurn()
    {
        if (!Multiplayer)
            return;
        if (CurrentTurn == TurnProcedure.Player1)
            CurrentTurn = TurnProcedure.Player2;
        else
            CurrentTurn = TurnProcedure.Player1;

    }

    public void GameWonCheck()
    {
        foreach (var score in Scores)
        {
            if (score.Value >= CurrentLevelScore)
            {
                ChangeState(GameState.GameWon);
                Winner = Players[score.Key];
                return;
            }
        }
    }

    public void NewGame()
    {
        ChangeState(GameState.NewGame);
        ScoringSystem.Reset();
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