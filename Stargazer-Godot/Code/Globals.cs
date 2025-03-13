using Godot;
using System;

namespace Stargazer
{
    /// <summary>
    /// Currently is no longer being used due to <see cref="Action"/>s and <see cref="Delegate"/>s
    /// </summary>
    public partial class Globals : Node
    {
        public static Globals Instance { get; private set; }

        public override void _Ready()
        {
            Instance = this;

        }

    }
}