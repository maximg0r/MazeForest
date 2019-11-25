using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameData {
    public static bool show_tutorial = true;
	public static int level_difficulty = 1;
	/* 1 = easy
	 * 2 = medium
	 * 3 = hard */

    public static float getParticleCooldown()
    {
        return Random.Range(-level_difficulty * 5 + 30, -level_difficulty * 5 + 50);
    }

    public static float getHintCooldown()
    {
        return Random.Range(level_difficulty * 5 + 10, level_difficulty * 5 + 30);
    }
}
