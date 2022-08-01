namespace AssetChecker
{
    public struct AssetObjectData
    {
        public readonly string AssetPath;
        public readonly string Owner;
        public readonly string Component;

        public AssetObjectData(string assetPath, string owner, string component)
        {
            AssetPath = assetPath;
            Owner = owner;
            Component = component;
        }
    }
}