using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System;
using System.Diagnostics.Contracts;

namespace DiceMG;

public class Score
{
    public List<Dice> DiceSet; 
    public static Dictionary<List<int>, int> ScoreReference;
    public static Dictionary<int, int> ExtraDiceScore;
    
    public int TotalScore = 0; 
    public int RoundScore = 0;
    
    public Score()
    {
        ScoreReference = new Dictionary<List<int>, int>();
        ScoreReference.Add([1,1,1],1000);
        ScoreReference.Add([2,2,2],200);
        ScoreReference.Add([3,3,3],300);
        ScoreReference.Add([4,4,4],400);
        ScoreReference.Add([5,5,5],500);
        ScoreReference.Add([6,6,6],600);
        ScoreReference.Add([1,2,3,4,5],500);
        ScoreReference.Add([2,3,4,5,6],600);
        ScoreReference.Add([1,2,3,4,5,6],1000);
        ScoreReference.Add([1],100);
        ScoreReference.Add([5],50);
        
        ExtraDiceScore = new Dictionary<int, int>();
        ExtraDiceScore.Add(1,1000);
        ExtraDiceScore.Add(2,200);
        ExtraDiceScore.Add(3,300);
        ExtraDiceScore.Add(4,400);
        ExtraDiceScore.Add(5,500);
        ExtraDiceScore.Add(6,600);
    }
    

    public static List<int> AllDiceValues(List<Dice> dice) => dice.Select(d => d.Value).ToList();
    public static List<int> HeldDiceValues(List<Dice> dice) => dice.Where(d => d.State == DieState.held).Select(d => d.Value).ToList();
    
    
    public void ResetGame()
    {
        TotalScore = 0;
        RoundScore = 0;
    }
    public void ResetRound()
    {
        RoundScore = 0;
    }

    public void AddToRoundScore(List<Dice> PlayerDiceSet)
    {
        RoundScore += GetTempScore(PlayerDiceSet); 
    }

    public void AddToTotalScore()
    {
        RoundScore += TotalScore;
        ResetRound();
    }
    public int GetRoundScore() => RoundScore;

    public int GetTotalScore() => TotalScore;

    private bool ContainsAll(List<int> list, params int[] values)
    {
        return values.All(v => list.Contains(v));
    }

    public bool PossibleScore(List<Dice> PlayerDiceSet)
    {
        var _heldDiceVals = HeldDiceValues(PlayerDiceSet);
        if (_heldDiceVals.Contains(1) || _heldDiceVals.Contains(5)) return true;
        if (ShowScore(_heldDiceVals) > 0) return true; 
        if (_heldDiceVals.GroupBy(x => x).Any(g => g.Count() >= 3)) return true;
        if (_heldDiceVals.Count == 6 && (ContainsAll(_heldDiceVals, 1, 2, 3, 4, 5) || ContainsAll(_heldDiceVals, 2, 3, 4, 5, 6))) return true;
            
        return false;
    }

    public int GetTempScore(List<Dice> PlayerDiceSet)
    {
        return ShowScore(HeldDiceValues(PlayerDiceSet));
    }
    
    // ===================== showing score equations - dont fuck with this ============================ //
    public static int ShowScore(List<int> dice)
    {
        static bool ListsEqual(List<int> a, List<int> b)
        {
            if (a.Count != b.Count) return false;
            for (int i = 0; i < a.Count; i++)
            {
                if (a[i] != b[i]) return false;
            }
            return true;
        }
        int score = 0;
        
        // Sort the dice for consistent comparison
        var sortedDice = dice.OrderBy(d => d).ToList();
        
        // Check if the sorted dice match any key in ScoreReference
        foreach (var kvp in ScoreReference)
        {
            var sortedKey = kvp.Key.OrderBy(k => k).ToList();
            if (ListsEqual(sortedDice, sortedKey))
            {
                score = kvp.Value;
                return score;
            }
        }
        
        // If no exact match, check for extra dice scenario
        if (dice.Count > 3 && dice.Distinct().Count() == 1)
        {
            var extra_dice = dice.Count - 3;
            var diceValue = dice[0];
            
            // Add base triple score
            var baseTriple = new List<int> { diceValue, diceValue, diceValue }.OrderBy(d => d).ToList();
            foreach (var kvp in ScoreReference)
            {
                var sortedKey = kvp.Key.OrderBy(k => k).ToList();
                if (ListsEqual(baseTriple, sortedKey))
                {
                    score = kvp.Value;
                    break;
                }
            }
            
            // Add extra dice bonus
            while (extra_dice > 0)
            {
                score += ExtraDiceScore[diceValue];
                extra_dice--;
            }
            
            Debug.WriteLine($"Score with extra dice: {score}");
        }
        
        return score;
    }
    // ===================== end of dont fuck with zone ============================ //
}
