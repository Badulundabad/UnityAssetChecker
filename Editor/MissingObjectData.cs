using UnityEditor;

namespace AssetChecker
{
    public struct MissingObjectData
    {
        public readonly string ScenePath;
        public readonly string SceneName;
        public readonly string ObjectPath;
        public readonly string ObjectName;
        public readonly string Component;
        public readonly GlobalObjectId ObjectId;

        public MissingObjectData(string objectPath, string objectName, string componentType)
        {
            ScenePath = string.Empty;
            SceneName = string.Empty;
            ObjectPath = objectPath;
            ObjectName = objectName;
            Component = componentType;
            ObjectId = new GlobalObjectId();
        }

        public MissingObjectData(string scenePath, string sceneName, string objectName, string componentType, GlobalObjectId objectId)
        {
            ScenePath = scenePath;
            SceneName = sceneName;
            ObjectPath = string.Empty;
            ObjectName = objectName;
            Component = componentType;
            ObjectId = objectId;
        }
    }
}