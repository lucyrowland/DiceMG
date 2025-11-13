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
    public Dictionary<int, int> Scores = new Dictionary<int, int>();
    public Dictionary<int, Player> Players = new Dictionary<int, Player>();
    public int Round; 
    public GameState State;
    private TurnState CurrentTurn;
    public int NumPlayers = 1;
    public Dictionary<string, int> LevelScores = new Dictionary<string, int>();
    public string Level = "Short";
    public int CurrentLevelScore;
    public Player Winner = null;

    public GameManager(int numPlayers, string level)
    {
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
    
    public void ChangeTurn()
    {
        if (CurrentTurn == TurnState.Player1)
            CurrentTurn = TurnState.Player2;
        else
            CurrentTurn = TurnState.Player1;
    }

    public void GameWonCheck()
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