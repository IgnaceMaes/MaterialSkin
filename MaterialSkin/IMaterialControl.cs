using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialSkin
{
    interface IMaterialControl
    {
        int Depth { get; set; }
        MaterialSkinManager SkinManager { get; }
        MouseState MouseState { get; set; }

        AnimationUsage AnimationUsage { get; set; }
    }

    public enum MouseState
    {
        HOVER,
        DOWN,
        OUT
    }

    public enum AnimationUsage
    {
        FULL,
        LITTLE,
        NONE
    }
}
