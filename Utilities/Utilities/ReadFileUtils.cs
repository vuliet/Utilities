using System.Reflection;

namespace Utilities.Utilities
{
    public static class ReadFileUtils
    {
        public static Task<string> ReadFileFromAssemblyAsync(
            string projectName,
            string pathName,
            Type type)
        {
            var assembly = Assembly.GetAssembly(type);

            if (assembly is null)
                return Task.FromResult(string.Empty);

            var resourceName = $"{projectName}.{pathName}";

            using var stream = assembly.GetManifestResourceStream(resourceName);

            if (stream is null)
                return Task.FromResult(string.Empty);

            using var reader = new StreamReader(stream);
            return reader.ReadToEndAsync();
        }
    }
}
