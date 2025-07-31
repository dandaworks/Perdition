using UnityEngine;
using System.Collections.Generic;

public class PersistentData : MonoBehaviour
{
    public static List<string> collectibleNames = new List<string>();

    public static bool beatTutorial = false;
    public static bool defeatedFamine = false;
    public static bool defeatedWar = false;
    public static bool defeatedConquest = false;
    public static bool endGame = false;
}
