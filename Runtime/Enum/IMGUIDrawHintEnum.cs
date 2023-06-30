namespace InEditor
{
    public enum IMGUIDrawHintEnum
    {
        /// <summary>
        /// Do nothing with the field
        /// </summary>
        None = -1,
        /// <summary>
        /// Auto detecting what to draw with this value
        /// </summary>
        Auto,
        /// <summary>
        /// Force the field to be drawn like label, convert by <see cref="object.ToString"/> 
        /// </summary>
        ToStringLabel,
    }
}