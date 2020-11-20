namespace DesignPlatform.Core {
    public abstract class Mode {
        public abstract void Tick();

        public virtual void OnModeResume() { }

        public virtual void OnModePause() { }
    }
}