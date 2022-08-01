namespace FusionClient.Modules
{
    public abstract class FusionModule
    {
        public virtual void UI() { }
        public virtual void Start() { }
        public virtual void Stop() { }
        public virtual void Update() { }
        public virtual void WorldRevealed() { }
        public virtual void SceneLoaded(int index, string name) { }
        public virtual void WorldLoaded(VRC.Core.ApiWorld apiWorld, VRC.Core.ApiWorldInstance apiWorldInstance) { }
        public virtual void LocalPlayerLoaded(VRC.Core.APIUser apiUser) { }
        public virtual void PlayerJoined(VRC.Player player) { }
        public virtual void PlayerLeft(VRC.Player player) { }
    }
}
