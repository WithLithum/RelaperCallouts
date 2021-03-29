namespace RelaperCallouts
{
    public enum BlipColor
    {
        White,
        Red,
        /// <summary>
        /// If not set, it's Green. If set to enemy, it's Red. If set to friendly, it's blue.
        /// </summary>
        Dynamic,
        Blue,
        Dynamic2,
        /// <summary>
        /// If not set, it's Yellow.
        /// </summary>
        YellowDynamic,
        LightRed,
        Violet,
        Pink,
        LightOrange,
    }
}