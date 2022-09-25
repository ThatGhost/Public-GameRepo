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
        JumpDown = 2048,

        JumpUp = 4096,
        Crouching = 8192,
        Aiming = 16_384,
        Interact = 32_768,
    }
}
