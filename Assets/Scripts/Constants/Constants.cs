﻿namespace Assets.Scripts.Constants
{
    public static class Tags
    {
        public static string Player = "Player";
    }

    public static class InputManager
    {
        public static string Horizontal = "Horizontal";
        public static string Vertical = "Vertical";
        public static string Jump = "Jump";
        public static string Invisibility = "Invisibility";
        public static string Hack = "Hack";
        public static string Translocate = "Translocate";
        public static string CancelTranslocator = "CancelTranslocator";
    }

    public static class Layer
    {
        public static string Hackable = "Hackable";
    }

    public static class AnimationParams
    {
        public static string HasXVelocity = "HasXVelocity";
        public static string Open = "Open";
        public static string Exit = "Exit";
        public static string Close = "Close";
        public static string Caught = "Caught";
        public static string Reset = "Reset";
        public static string HackingStance = "HackingStance";
        public static string Hacking = "Hacking";
    }

    public static class Delays
    {
        public static float CaughtDelay = 0.5f;
    }
}
