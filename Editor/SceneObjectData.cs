namespace AssetChecker
{
    public struct SceneObjectData
    {
        public readonly string SceneName;
        public readonly string ScenePath;
        public readonly string Owner;
        public readonly string Component;

        public SceneObjectData(string sceneName, string scenePath, string owner, string component)
        {
            SceneName = sceneName;
            ScenePath = scenePath;
            Owner = owner;
            Component = component;
        }
    }
}
