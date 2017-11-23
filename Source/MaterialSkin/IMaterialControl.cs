namespace MaterialSkin
{
    public interface IMaterialControl
    {
        MaterialSkinManager SkinManager { get; }

        int Depth { get; set; }

        MouseState MouseState { get; set; }
    }

    public enum MouseState
    {
        HOVER,
        DOWN,
        OUT
    }
}
