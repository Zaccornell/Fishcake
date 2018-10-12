using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOptions
{
    private static readonly PlayerOptions m_instance = new PlayerOptions();

    static PlayerOptions()
    {

    }
    private PlayerOptions()
    {

    }

    public static PlayerOptions Instance
    {
        get { return m_instance; }
    }
}
