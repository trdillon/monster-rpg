using UnityEngine;

namespace Itsdits.Ravar.Util
{
    /// <summary>
    /// Static helper class that pools yields to avoid garbage collection being triggered.
    /// </summary>
    public static class YieldHelper
    {
        public static readonly WaitForSeconds TYPING_TIME = new WaitForSeconds(0.02f);
        public static readonly WaitForSeconds FIFTH_SECOND = new WaitForSeconds(0.2f);
        public static readonly WaitForSeconds HALF_SECOND = new WaitForSeconds(0.5f);
        public static readonly WaitForSeconds ONE_SECOND = new WaitForSeconds(1f);
        public static readonly WaitForSeconds TWO_SECONDS = new WaitForSeconds(2f);
        public static readonly WaitForSeconds TWO_AND_CHANGE_SECONDS = new WaitForSeconds(2.1f);
        public static readonly WaitForSeconds THREE_SECONDS = new WaitForSeconds(3f);
        public static readonly WaitForSeconds FOUR_SECONDS = new WaitForSeconds(4f);
        public static readonly WaitForEndOfFrame END_OF_FRAME = new WaitForEndOfFrame();
    }
}