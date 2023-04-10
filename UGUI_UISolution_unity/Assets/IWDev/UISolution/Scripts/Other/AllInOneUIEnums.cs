namespace IWDev.UISolution
{
    /// <summary>
    /// UI Elements basic state variants 
    /// </summary>
    public enum UIEBasicStates
    {
        None = 0,
        BeforeAppear = 1,
        Appear = 2,
        Idle = 3,
        Clicked = 4,
        Pressed = 5,
        Disappear = 6,
        WhileIdle = 7,
    }


    /// <summary>
    /// UI Elements transform states: BeforeAppear
    /// </summary>
    public enum UIEStates_BeforeAppear
    {
        None = 0,
        FromZeroScale = 1,
    }

    /// <summary>
    /// UI Elements transform states: Appear
    /// </summary>
    public enum UIEStates_Appear
    {
        None = 0,
        Fancy = 1,
        SimpleScale = 2,
    }

    /// <summary>
    /// UI Elements transform states: Idle
    /// </summary>
    public enum UIEStates_Idle
    {
        None = 0,
        Idle_Pressme = 1,
    }

    /// <summary>
    /// UI Elements transform states: Clicked
    /// </summary>
    public enum UIEStates_Clicked
    {
        None = 0,
        Fancy = 1,
        SimpleScale = 2,
    }


    /// <summary>
    /// UI Elements transform states: Pressed
    /// </summary>
    public enum UIEStates_Pressed
    {
        None = 0,
        Fancy = 1,
        SimpleScale = 2,
    }

    /// <summary>
    /// UI Elements transform states: Pressed
    /// </summary>
    public enum UIEStates_Disappear
    {
        None = 0,
        Fancy = 1,
        SimpleScale = 2,
        Instant = 3,

    }


    /// <summary>
    /// UI Elements transform states: Kick - means some animations which played on demand
    /// </summary>
    public enum UIEStates_Kick
    {
        None = 0,
        Bump = 1,
    }


    /// <summary>
    /// Window appear types 
    /// </summary>
    public enum UIWAppearTypes
    {
        None = 0,
        AlphaOnly = 1,
        Bubble = 2,
        FromBottom = 3,
        FromTop = 4,
        FromRight = 5,
        FromLeft = 6,

        FromBottomRight = 7,
        FromBottomLeft = 8,
        FromTopRight = 9,
        FromTopLeft = 10,
    }


    /// <summary>
    /// Window disappear types 
    /// </summary>
    public enum UIWDisappearTypes
    {
        None = 0,
        AlphaOnly = 1,
        Bubble = 2,
        ToBottom = 3,
        ToTop = 4,
        ToRight = 5,
        ToLeft = 6,

        ToBottomRight = 7,
        ToBottomLeft = 8,
        ToTopRight = 9,
        ToTopLeft = 10,
    }

    /// <summary>
    /// Window states 
    /// </summary>
    public enum UIWStateTypes
    {
        None = 0,
        Appear = 1,
        Idle = 2,
        Disappear = 3,
    }

}
