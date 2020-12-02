namespace DesignPlatform.Modes {

    /// <summary>
    /// The base class that declares what all valid modes must implement.
    /// </summary>
    public abstract class Mode {

        /// <summary>Defines the actions to take at every frame where this mode is active.</summary>
        public abstract void Tick();

        /// <summary>Defines the actions to take when changing into this mode.</summary>
        public virtual void OnModeResume() { }

        /// <summary>Defines the actions to take when changing out of this mode.</summary>
        public virtual void OnModePause() { }
    }
}