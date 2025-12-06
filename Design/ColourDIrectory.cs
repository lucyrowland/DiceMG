using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Collections.Generic;


public class ColourDirectory
{

    //File names, colour hex code from Lospec and then corresponding colour name that i just wrote on notepad
    static string file_path = @"C:\lu_git\csharp\DiceMG\Design\";
    string hex_file_name = file_path + "pink_palette.hex";
    private string colour_file_name = file_path + "colour_names.txt";
    
    //Dictionary of colour names and hex values
    public Dictionary<string, Color> Palette  = new Dictionary<string, Color>();
    
    public ColourDirectory()
    {
        string[] hexColours = File.ReadAllLines(hex_file_name);
        string[] colourNames = File.ReadAllLines(colour_file_name);
        for(int i = 0; i < hexColours.Length; i++)
        {
            Color colour = ColourFromHex(hexColours[i].Trim());
            string colourName = colourNames[i].Trim();
            
            Palette.Add(colourName, colour);
            
        }
    }

    Color ColourFromHex(string hex)
    {
        byte r = Convert.ToByte(hex.Substring(0, 2), 16);
        byte g = Convert.ToByte(hex.Substring(2, 2), 16);
        byte b = Convert.ToByte(hex.Substring(4, 2), 16);
        return new Color(r, g, b);
    }
    
    public Color Paint(string colourName) => Palette[colourName];
}