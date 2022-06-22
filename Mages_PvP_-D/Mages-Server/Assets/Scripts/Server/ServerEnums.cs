using System;

namespace MagesServer.Enums
{
    public enum GameState
    {
        Syncronisation,
        Running,
        Ended,
        All
    }

    [Flags]
    public enum InputType
    {
        Ultimate = 1,
        Ability1 = 2,
        Ability2 = 4,
        Ability3 = 8,

        Ability4 = 16,
        Emote = 32,
        Left = 64,
        Right = 128,

        Up = 256,
        Down = 512,
        Running = 1_024,
        Jumping = 2_048,

        Crouching = 4_096,
        Aiming = 8_192,
        Open2 = 16_384,
        Open3 = 32_768,
    }
}
