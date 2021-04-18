using System.IO;
using System.Reflection;

namespace SafeWarehouseApp.Extensions
{
    public static class AssetLoader
    {
        public static Stream ReadAssetStream(string name)
        {
            var fileName = $"SafeWarehouseApp.Assets.{name}";
            var assembly = Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream(fileName)!;
        }
    }
}