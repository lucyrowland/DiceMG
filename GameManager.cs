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



public enum TurnState { Player1, Player2 }

public class GameManager
{
    public static Dictionary<int, int> Scores = new Dictionary<int, int>();
    public static Dictionary<int, Player> Players = new Dictionary<int, Player>();
    public static int Round; 
    public static GameState State;
    public static TurnState CurrentTurn;
    public static int NumPlayers = 1;
    public Dictionary<string, int> LevelScores = new Dictionary<string, int>();
    public static string Level = "Short";
    public static int CurrentLevelScore;
    public static Player Winner = null;
    public static bool Multiplayer;

    public GameManager(int numPlayers, string level)
    {
        Multiplayer = true;
         CurrentTurn = TurnState.Player1;
         NumPlayers = numPlayers;
         Round = 1; 
         State = GameState.NewGame;

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
        CurrentTurn = TurnState.Player1;
        NumPlayers = 1;
        Round = 1; 
        State = GameState.NewGame;
        Players.Add(1, new Player("Player 1", 1));
        Scores.Add(1, 0);
        Debug.WriteLine($"Player 1 added");
        
        GenerateLevels();
        Level = "Short"; 
        CurrentLevelScore = LevelScores[Level];
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
        if (CurrentTurn == TurnState.Player1) Round++;
        ScoringSystem.AddScore();
    }

    public void SkipTurn()
    {
        ChangeTurn();
        if (CurrentTurn == TurnState.Player1) Round++;
        //ScoringSystem.NewRound();
        ScoringSystem.AddScore(); //need to add temp score and round score to scoring system
    }

    public void FlopTurn()
    {
        ChangeTurn();
        if (CurrentTurn == TurnState.Player1) Round++;
        //ScoringSystem.NewRound(); to reset tempscore and roundscore to 0 
    }
    
    public static void ChangeTurn()
    {
        if (!Multiplayer)
            return;
        if (CurrentTurn == TurnState.Player1)
            CurrentTurn = TurnState.Player2;
        else
            CurrentTurn = TurnState.Player1;

    }

    public static void GameWonCheck()
    {
        foreach (var score in Scores)
        {
            if (score.Value >= CurrentLevelScore)
            {
                State = GameState.GameWon;
                Winner = Players[score.Key];
                return;
            }
        }
    }

    public void NewGame()
    {
        State = GameState.NewGame;
        ScoringSystem.Reset();
    }
}