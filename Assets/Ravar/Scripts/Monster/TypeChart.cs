namespace Itsdits.Ravar.Monster 
{
    /// <summary>
    /// Type chart for strength and weakness definitions by <see cref="MonsterType"/>.
    /// </summary>
    public static class TypeChart
    {
        private static readonly float[][] Chart =
        {
            // Normal, Aqua, Ember, Earth, Shock, Air, Spirit, Force, Shadow, Light
            new[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f },       // Normal
            new[] { 1f, 0.5f, 2f, 0.5f, 1f, 1f, 1f, 1f, 1f, 1f },   // Aqua
            new[] { 1f, 0.5f, 0.5f, 2f, 1f, 1f, 1f, 1f, 1f, 1f },   // Ember
            new[] { 1f, 2f, 0.5f, 0.5f, 1f, 1f, 1f, 1f, 1f, 1f },   // Earth
            new[] { 1f, 2f, 1f, 0.5f, 0.5f, 2f, 1f, 1f, 1f, 1f },   // Shock
            new[] { 1f, 1f, 1f, 2f, 0.5f, 0.5f, 1f, 1f, 1f, 1f },   // Air
            new[] { 1f, 1f, 1f, 1f, 1f, 1f, 0.5f, 0.5f, 1f, 2f },   // Spirit
            new[] { 1f, 1f, 1f, 1f, 1f, 1f, 2f, 0.5f, 0.5f, 1f },   // Force
            new[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 0.5f, 0.5f },   // Shadow
            new[] { 1f, 1f, 1f, 1f, 1f, 1f, 0.5f, 1f, 2f, 0.5f }    // Light
        };

        /// <summary>
        /// Get the type effectiveness based on the chart.
        /// </summary>
        /// <param name="attackType">Type of attack.</param>
        /// <param name="defenseType">Type of monster being attacked.</param>
        /// <returns>Float to change attack bonus.</returns>
        public static float GetEffectiveness(MonsterType attackType, MonsterType defenseType)
        {
            // For single type monsters we return 1 for the empty secondary type.
            if (attackType == MonsterType.None || defenseType == MonsterType.None)
            {
                return 1;
            }

            var row = (int)attackType;
            var col = (int)defenseType;
            return Chart[row][col];
        }
    }
}