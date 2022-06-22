using System;
namespace MagesClient.Enums
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
        Running = 1024,
        Jumping = 2048,

        Crouching = 4096,
        Aiming = 8192,
        Open2 = 16_384,
        Open3 = 32_768,
    }

    [Flags]
    public enum PlayerState
    {
        Jumping = 1,
        Crouching = 2,
        Running = 4,
    }
}
