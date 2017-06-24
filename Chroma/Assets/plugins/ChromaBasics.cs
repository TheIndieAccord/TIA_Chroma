using System;
using UnityEngine;

using Corale.Colore.Core;
using Corale.Colore.Razer.Keyboard;
//using Corale.Colore.Razer.Mouse;

using KeyboardCustom = Corale.Colore.Razer.Keyboard.Effects.Custom;
using ColoreColor = Corale.Colore.Core.Color;

public class ChromaBasics : MonoBehaviour
{
    //Default empty color.
    private ColoreColor empty = new ColoreColor(0, 0, 0);

    //Class variables. Common to all of the objects.
    private float fd = -1.0f;
    private int fdSpd = 10;
    private bool fading = false;

    //Keyboard specific variables
    //Initialize the required number of layers for the keyboard to use
    private ColoreColor[,] layer1 = new ColoreColor[Constants.MaxRows, Constants.MaxColumns];
    private ColoreColor[,] layer2 = new ColoreColor[Constants.MaxRows, Constants.MaxColumns];
    private ColoreColor[,] layer3 = new ColoreColor[Constants.MaxRows, Constants.MaxColumns];
    private KeyboardCustom keyboardGrid = KeyboardCustom.Create();

    //Uninitializes the Colore DLL on application quit. This MUST be called do to a current issue with DLL/Unity.
    public static void OnApplicationQuit()
    {
        Chroma.Instance.Uninitialize();
    }

    //Class Constructor used by Javascript.
    public ChromaBasics()
    {
        fd = -1.0f;
        //Assigns a base color for each layer
        //Keyboard
            AssignKeyboardLayer(0,0,0,1);
            AssignKeyboardLayer(0,0,0,2);
            AssignKeyboardLayer(0,0,0,3);

        //Cursor
        
    }

    /* Adjusts the fade speed.
     * @param spd   adjusts the speed of the fade
     */
    public void AllFdSp(int spd)
    {
        fdSpd = spd;
    }

    /* Controls fading of a layer
     * @param r     red
     * @param g     green
     * @param b     blue
     * @param layer designated layer
     * @param isfd  switch to activate/deactivate fade
     */
    public void AllFd(int r, int g, int b, int layer, bool isfd)
    {
        fading = isfd;
        if (fading == true)
        {
            ColoreColor col = new ColoreColor((int)(r * Math.Abs(fd)), (int)(g * Math.Abs(fd)), (int)(b * Math.Abs(fd)));
            AssignKeyboardLayer(col, layer);
        }
    }

    /* Assigns the key to the supplied color.
     * @param r     0-255 integer level of red.
     * @param g     0-255 integer level of green.
     * @param b     0-255 integer level of blue.
     * @param layer integer layer, where 1 is the highest layer, followed by 2.
     * @param x     designated row on the keyboard.
     * @param y     designated column on the keyboard. 
     */
    public void AssignKey(int r, int g, int b, int layer, int x, int y)
    {
        ColoreColor col = new ColoreColor(r, g, b);
        AssignKeyboardKey(col, layer, x, y);
    }

    /* Assigns the key to the supplied color.
     * @hexcol      Color coded in RGB hexadecimal format of 0xRRGGBB.
     * @param layer integer layer, where 1 is the highest layer, followed by 2.
     * @param x     designated row on the keyboard.
     * @param y     designated column on the keyboard. 
     */
    public void AssignKey(uint hexcol, int layer, int x, int y)
    {
        ColoreColor col = new ColoreColor(ColoreColor.FromRgb(hexcol));
        AssignKeyboardKey(col, layer, x, y);
    }

    /* Assigns an entire horizontal row to a given color.
     * @param hexcol Color coded in RGB hexadecimal format of 0xRRGGBB.
     * @param layer Integer layer, where 1 is the highest layer, incremented up.
     * @param row Integer that holds the row value.
     */
    public void AssignKeyboardHorizontal(uint hexcol, int layer, int row)
    {
        ColoreColor col = new ColoreColor(ColoreColor.FromRgb(hexcol));
        for (var c = 0; c < Constants.MaxColumns; c++)
        {
            AssignKeyboardKey(col, layer, row, c);
        }
    }

    /* Generates the Color
     * @param r     0-255 integer level of red.
     * @param g     0-255 integer level of green.
     * @param b     0-255 integer level of blue.
     * @param layer integer layer, where 1 is the highest layer, followed by 2.
     */
    public void AssignKeyboardLayer(int r, int g, int b, int layer)
    {
        ColoreColor col = new ColoreColor(r, g, b);
        AssignKeyboardLayer(col, layer);
    }

    /* Generates the Color
     * @param hexcol Color coded in RGB hexadecimal format 0xRRGGBB.
     * @param layer integer layer, where 1 is the highest layer, followed by 2.
     */
    public void AssignKeyboardLayer(uint hexcol, int layer)
    {
        ColoreColor col = new ColoreColor(ColoreColor.FromRgb(hexcol));
        AssignKeyboardLayer(col, layer);
    }

    /* Assigns an entire vertical column to a given color.
     * @param hexcol Color coded in RGB hexadecimal format of 0xRRGGBB.
     * @param layer Integer layer, where 1 is the highest layer, incremented up.
     * @param column Integer that holds the column value.
     */
    public void AssignKeyboardVertical(uint hexcol, int layer, int column)
    {
        ColoreColor col = new ColoreColor(ColoreColor.FromRgb(hexcol));
        for (var r = 0; r < Constants.MaxRows; r++)
        {
            AssignKeyboardKey(col, layer, r, column);
        }
    }

    //Sends the created layers to the SDK for visualization.
    public void Update()
    {
        fd = fd + Time.deltaTime / fdSpd;
        if (Math.Abs(fd) < 0.01f)
            fd = 0.01f;

        if (fd >= 1.0)
            fd = -1.0f;

        //Loop through all Rows
        for (var r = 0; r < Constants.MaxRows; r++)
        {
            //Loop through all Columns
            for (var c = 0; c < Constants.MaxColumns; c++)
            {
                //Checks if the higher layer is blank. If so, display the underlying layer.
                // if (Chroma.Instance.Keyboard[r, c]!=null) {
                    if (layer1[r, c] == empty)
                        keyboardGrid[r, c] = layer2[r, c];
                    else
                        keyboardGrid[r, c] = layer1[r, c];
                //}
                
            }
        }
        Chroma.Instance.Keyboard.SetCustom(keyboardGrid);
    }

    /*
     *
     * This is where most of the magic happens. Functions above call these to assign the colors.
     * None of these functions should ever be directly called.
     *
     */

    /* Assigns all of the keys to the supplied color.
     * @param col     Generated color
     * @param layer integer layer, where 1 is the highest layer, followed by 2.
     */
    private void AssignKeyboardLayer(ColoreColor col, int layer)
    {
        //Supplies the given color combination to ALL of the keys on the required layer.
        switch (layer)
        {
            case 1:
                //Loop through all Rows
                for (var rows = 0; rows < Constants.MaxRows; rows++)
                {
                    //Loop through all Columns
                    for (var c = 0; c < Constants.MaxColumns; c++)
                    {
                        layer1[rows, c] = col;
                    }
                }
                break;
            case 2:
                //Loop through all Rows
                for (var rows = 0; rows < Constants.MaxRows; rows++)
                {
                    //Loop through all Columns
                    for (var c = 0; c < Constants.MaxColumns; c++)
                    {
                        layer2[rows, c] = col;
                    }
                }
                break;
            case 3:
                //Loop through all Rows
                for (var rows = 0; rows < Constants.MaxRows; rows++)
                {
                    //Loop through all Columns
                    for (var c = 0; c < Constants.MaxColumns; c++)
                    {
                        layer3[rows, c] = col;
                    }
                }
                break;
            default:
                //Loop through all Rows
                for (var rows = 0; rows < Constants.MaxRows; rows++)
                {
                    //Loop through all Columns
                    for (var c = 0; c < Constants.MaxColumns; c++)
                    {
                        layer1[rows, c] = col;
                    }
                }
                break;
        }
    }

    /* Assigns the keys to the supplied color.
     * @col      Color coded in RGB hexadecimal format of 0xRRGGBB.
     * @param layer integer layer, where 1 is the highest layer, followed by 2.
     * @param x     designated row on the keyboard.
     * @param y     designated column on the keyboard. 
     */
    private void AssignKeyboardKey(ColoreColor col, int layer, int x, int y)
    {
        layer1[x, y] = col;
        //Assigns the specified key in the required layer to the given color.
        switch (layer)
        {
            case 1:
                layer1[x, y] = col;
                break;
            case 2:
                layer2[x, y] = col;
                break;
            case 3:
                layer3[x, y] = col;
                break;
            default:
                layer1[x, y] = col;
                break;
        }
    }


}