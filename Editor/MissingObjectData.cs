namespace AssetChecker
{
    public struct MissingObjectData
    {
        public readonly string ScenePath;
        public readonly string SceneName;
        public readonly string ObjectPath;
        public readonly string ObjectName;
        public readonly string Component;
        public readonly int ObjectSceneIndex;

        public MissingObjectData(string objectPath, string objectName, string componentType)
        {
            ScenePath = string.Empty;
            SceneName = string.Empty;
            ObjectPath = objectPath;
            ObjectName = objectName;
            Component = componentType;
            ObjectSceneIndex = -1;
        }

        public MissingObjectData(string scenePath, string sceneName, string objectName, string componentType, int objectIndex)
        {
            ScenePath = scenePath;
            SceneName = sceneName;
            ObjectPath = string.Empty;
            ObjectName = objectName;
            Component = componentType;
            ObjectSceneIndex = objectIndex;
        }
    }
}