using System.Collections.Generic;
using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using DiceMG.Input;
using System.Linq;

namespace DiceMG;

public enum GameState
{
    NewGame,        // Initial state - game hasn't started
    RoundStart,     // Beginning of a new round
    RollComplete,   // Dice have been rolled, waiting for player decision
    PlayerScoring,  // Player is selecting dice to score
    RollAgain,      // Player chose to roll remaining dice
    PassTurn,       // Player chose to pass their turn
    BustedRoll,     // Invalid roll - no scoring dice
    PlayerSwitch,   // Transitioning between players
    CheckWinCondition, // Check if someone won
    GameWon,        // Someone reached the goal
    Paused,         // Game paused
    GameOver        // Game ended
}

public enum ScoreType {Total, Round, Hand}


public class GameManager
{
    
    public List<Player> Players = new List<Player>();
    public Player ActivePlayer;
    public int GoalScore = 2000;
    private int PlayerIndex = 0;
    public GameState State { get; private set; }
    
    public EventHandler<GameStateArgs> StateChangedHandler;

    public int _numberofDice;
    public ObjectManager _ObjManager;
    

    public GameManager(List<Player> players)
    {
        Players = players;
        if (players.Count > 0)
        {
            ActivePlayer = Players[0];
            PlayerIndex = 0;
        }
        State = GameState.NewGame;

    }

    public GameManager()
    {
        State = GameState.NewGame;
    }

    public GameManager(int numberofDice, ObjectManager ObjManager)
    {
        State = GameState.NewGame;
        _numberofDice = numberofDice;
        _ObjManager = ObjManager;
    }

    public void AddPlayer(Player player)
    {
        Players.Add(player);
        if (Players.Count == 1)
        {
            ActivePlayer =  player;
            PlayerIndex = 0; 
        }
    }

    /// <summary>
    /// Starts the game - call this when players are ready to start
    /// </summary>
    public void StartGame()
    {
        if (Players.Count < 2)
        {
            Console.WriteLine("Game is not made for one player yet, please add another player'");
            return; 
        }

        PlayerIndex = 0;
        ActivePlayer = Players[PlayerIndex];
        
        //Reset all player scores to 0 
        foreach (var player in Players) player.Scores.ResetGame();

        ChangeState(GameState.RoundStart);
    }

    /// <summary>
    /// State machine update handler
    /// </summary>
    public void ProcessState()
    {
        switch (State)
        {
            case GameState.NewGame:
            // Waiting for StartGame() to be called
            break;

            case GameState.RoundStart:
                // Start a new round for the active player
                NewRound();
                break;

            case GameState.RollComplete:
                // Dice have been rolled, check if valid
                CheckRollValidity();
                break;

            case GameState.PlayerScoring:
                // Player is selecting dice - waiting for Lock or Pass button
                break;

            case GameState.RollAgain:
                // Player locked in some dice and wants to roll again
                RollDiceState();
                break;

            case GameState.PassTurn:
                // Player is passing - add round score to total and switch players
                PassandSwitchPlayer();
                break;

            case GameState.BustedRoll:
                // Invalid roll - lose round score and switch players
                BustedAndSwitchPlayer();
                break;

            case GameState.PlayerSwitch:
                // Transition to next player
                PlayerStateChange();
                break;

            case GameState.CheckWinCondition:
                // Check if current player won
                CheckWinCondition();
                break;

            case GameState.GameWon:
                // Game is over, someone won
                break;

            case GameState.GameOver:
                // Game ended
                break;
        }
    }

    public void NewRound()
    {
        RollDiceState();
    }

    public void CheckRollValidity()
    {
        // Check only active (non-played) dice — played dice from earlier locks must not affect validity
        if (CheckValidRoll(ActivePlayer.ActiveDice)) ChangeState(GameState.PlayerScoring);
        else ChangeState(GameState.BustedRoll);
    }

    public void RollDiceState()
    {
        ActivePlayer.RollDice();
        ChangeState(GameState.RollComplete);
        
    }

    /// <summary>
    /// Player passed - flush held dice into round score, then commit round to total and switch
    /// </summary>
    public void PassandSwitchPlayer()
    {
        if (ActivePlayer.HeldDice.Count > 0)
            AddRoundScore(ActivePlayer);    // flush hand → round
        AddTotalScore(ActivePlayer);        // round → total (resets round internally)
        ActivePlayer.ResetDiceList(_numberofDice, _ObjManager);
        ChangeState(GameState.CheckWinCondition);
    }

    public void BustedAndSwitchPlayer()
    {
        ActivePlayer.Scores.ResetRound();
        ActivePlayer.ResetDiceList(_numberofDice, _ObjManager);
        ChangeState(GameState.PlayerSwitch);
    }

    private void CheckWinCondition()
    {
        if (ActivePlayer.Scores.TotalScore >= GoalScore) ChangeState(GameState.GameWon);
        else ChangeState(GameState.PlayerSwitch);
    }
    
  

    public void PlayerStateChange()
    {
        PlayerIndex++;

        if (PlayerIndex >= Players.Count)
        {
            PlayerIndex = 0;
        }

        ActivePlayer = Players[PlayerIndex];
        ActivePlayer.ResetDiceList(_numberofDice, _ObjManager);

        ChangeState(GameState.RoundStart);
    }

    public bool CheckValidRoll(List<Dice> diceList)
    {
        List<int> diceVals = new List<int>();
        diceList.ForEach(d => diceVals.Add(d.Value));
        diceVals.Sort();


        if (diceVals.Contains(5) || diceVals.Contains(1)) return true;
        if (diceVals.SequenceEqual(new[] {1,2,3,4,5,6}) ||
            diceVals.SequenceEqual(new[] {1,2,3,4,5})    ||
            diceVals.SequenceEqual(new[] {2,3,4,5,6}))   return true;
        if (diceVals.GroupBy(x => x).Any(g => g.Count() >= 3)) return true;
        
        return false; 

    } 
    private static readonly HashSet<GameState> WaitingStates = new()
    {
        GameState.NewGame, GameState.RoundStart, GameState.RollComplete,
        GameState.PlayerScoring, GameState.BustedRoll,
        GameState.GameWon, GameState.GameOver
    };

    /// <summary>
    /// Drives the state machine through auto-transitioning states until a player-input state is reached.
    /// </summary>
    public void ProcessUntilWaiting()
    {
        int guard = 20;
        while (!WaitingStates.Contains(State) && guard-- > 0)
            ProcessState();
    }

    public void ChangeState(GameState newState)
    {
        State = newState;
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
        if (die.State == DieState.played) return;

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