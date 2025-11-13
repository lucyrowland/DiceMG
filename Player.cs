using System.Collections.Generic;
using Microsoft.Xna.Framework;


namespace DiceMG;


public class Player
{
    public string Name;
    public RollingTray Tray;
    public List<Dice> DiceSet;
    public int Score;

    public float _trayx;
    public float _trayy;
    public Vector2 _traysize; 
    
    public int PNumber;
    
    public Player(string name, int p_num)
    {
        Name = name;
        DiceSet = new List<Dice>();
        Tray = new RollingTray(_trayx, _trayy, new Vector2(_traysize.X, _traysize.Y));
        PNumber = p_num;
    }

}