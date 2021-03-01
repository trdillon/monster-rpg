public class TypeChart
{
    static float[][] chart =
    {
        // Normal, Aqua, Ember, Earth, Shock, Air, Spirit, Force, Shadow, Light
        //
        new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f },         // Normal
        new float[] { 1f, 0.5f, 2f, 0.5f, 1f, 1f, 1f, 1f, 1f, 1f },     // Aqua
        new float[] { 1f, 0.5f, 0.5f, 2f, 1f, 1f, 1f, 1f, 1f, 1f },     // Ember
        new float[] { 1f, 2f, 0.5f, 0.5f, 1f, 1f, 1f, 1f, 1f, 1f },     // Earth
        new float[] { 1f, 2f, 1f, 0.5f, 0.5f, 2f, 1f, 1f, 1f, 1f },     // Shock
        new float[] { 1f, 1f, 1f, 2f, 0.5f, 0.5f, 1f, 1f, 1f, 1f },     // Air
        //TODO - Implement these
        new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 0.5f, 1f, 1f, 1f },       // Spirit
        new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 0.5f, 1f, 1f },       // Force
        new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 0.5f, 1f },       // Shadow
        new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 0.5f }        // Light
    };

    public static float GetEffectiveness(MonsterType attackType, MonsterType defenseType)
    {
        // for single type monsters we return 1 for the empty secondary type
        if (attackType == MonsterType.None || defenseType == MonsterType.None)
            return 1;

        int row = (int)attackType;
        int col = (int)defenseType;

        return chart[row][col];
    }
}
