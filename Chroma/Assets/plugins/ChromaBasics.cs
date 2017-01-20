﻿/**
 * Name: CromaBasics.cs
 * Author: Hunter Dubel
 * Description: Basic functions for Chroma in Unity with layer functionality.
 * Updated: January 19, 2017 00:39
 **/

using UnityEngine;

using Corale.Colore.Core;
using Corale.Colore.Razer.Keyboard;

using ColoreColor = Corale.Colore.Core.Color;

public class ChromaBasics : MonoBehaviour
{
    private ColoreColor[,] layer1 = new ColoreColor[Constants.MaxRows, Constants.MaxColumns];
    private ColoreColor[,] layer2 = new ColoreColor[Constants.MaxRows, Constants.MaxColumns];
    private float fade = 0;
    private bool isFade = false;

    //Uninitializes the Colore DLL on application quit. This MUST be called do to a current issue with DLL/Unity.
    public static void OnApplicationQuit()
    {
        Chroma.Instance.Uninitialize();
    }

    //Class Constructor used by Javascript.
    public ChromaBasics()
    {

    }

    /* Generates the Color
     * @param r     0-255 integer level of red.
     * @param g     0-255 integer level of green.
     * @param b     0-255 integer level of blue.
     * @param layer integer layer, where 1 is the highest layer, followed by 2.
     */
    public void AssignAll(int r, int g, int b, int layer)
    {
        ColoreColor col = new ColoreColor(r, g, b);
        AssignLayerAll(col,layer);
    }

    /* Generates the Color
     * @param hexcol Color coded in RGB hexadecimal format 0xRRGGBB.
     * @param layer integer layer, where 1 is the highest layer, followed by 2.
     */
    public void AssignAll(uint hexcol, int layer)
    {
        ColoreColor col = new ColoreColor(ColoreColor.FromRgb(hexcol));
        AssignLayerAll(col,layer);
    }

    /* Controls fading of a layer.
     *
     */
    public void AllFade(int r, int g, int b, int layer, bool isFade)
    {
        isFade = this.isFade;
        while (isFade)
        {
        ColoreColor col = new ColoreColor(r * fade, g * fade, b * fade);
        AssignLayerAll(col,layer);
        }
        fade = 0;
    }

    /* Assigns all of the keys to the supplied color.
     * @param col     Generated color
     * @param layer integer layer, where 1 is the highest layer, followed by 2.
     */
    public void AssignLayerAll(ColoreColor col, int layer)
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

    /* Assigns all of the keys to the supplied color.
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
        AssignLayerKey(col, this.layer, this.x, this.y);
    }

    /* Assigns all of the keys to the supplied color.
     * @hexcol      Color coded in RGB hexadecimal format of 0xRRGGBB.
     * @param layer integer layer, where 1 is the highest layer, followed by 2.
     * @param x     designated row on the keyboard.
     * @param y     designated column on the keyboard. 
     */
    public void AssignKey(uint hexcol, int layer, int x, int y)
    {
        ColoreColor col = new ColoreColor(ColoreColor.FromRgb(hexcol));
        AssignLayerKey(col, this.layer, this.x, this.y);
    }

    /* Assigns all of the keys to the supplied color.
     * @col      Color coded in RGB hexadecimal format of 0xRRGGBB.
     * @param layer integer layer, where 1 is the highest layer, followed by 2.
     * @param x     designated row on the keyboard.
     * @param y     designated column on the keyboard. 
     */
    public void AssignLayerKey(ColoreColor col, int layer, int x, int y)
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
            default:
                layer1[x, y] = col;
                break;
        }
    }

    //Sends the created layers to the SDK for visualization.
    public void Update()
    {
        fade += Time.deltatime;

        if (fade >= 1)
            fade = -1;
  
        //Loop through all Rows
        for (var r = 0; r < Constants.MaxRows; r++)
        {
            //Loop through all Columns
            for (var c = 0; c < Constants.MaxColumns; c++)
            {
                //Checks if the higher layer is blank. If so, display the underlying layer.
                ColoreColor empty = new ColoreColor(0, 0, 0);
                if (layer1[r, c] == empty)
                    Chroma.Instance.Keyboard[r, c] = layer2[r, c];
                else
                    Chroma.Instance.Keyboard[r, c] = layer1[r, c];
            }
        }
    }

}