using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class CommentColor
{
    public int CommentNumber;
	public Color32 CommentCol;
}

public class ColorManager : MonoBehaviour
{

    public CommentColor[] Commentcolor;
    public static ColorManager instance;
    private void Awake()
    {
        if(instance == null){
            instance = this;
        }
    }
}
