using MediawikiTranslator.Models.DamageTable;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization.Json;

namespace MediawikiTranslator.Parsers
{
    internal class DamageTable
    {
        internal static Task<SourceData[]> FromGameSource(byte[] zipFile)
        {
            return Task.Run(async () =>
            {
                DirectoryInfo workspace = Utilities.GetWorkspace();
                string zipPath = Path.Combine(workspace.FullName, Guid.NewGuid().ToString() + ".zip");
                await File.WriteAllBytesAsync(zipPath, zipFile);
                string extractDir = Path.Combine(workspace.FullName, Guid.NewGuid().ToString());
                System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, extractDir);
                IEnumerable<string> files = Directory.EnumerateFiles(extractDir);
                List<SourceData> sources = [];
                foreach (string file in files)
                {
                    SourceData? data = null;
                    try
                    {
                        string fileData = await File.ReadAllTextAsync(file);
                        data = SourceData.FromJson(fileData);
                        data!.FileName = Path.GetFileNameWithoutExtension(file);
                    }
                    finally
                    {
                        if (data != null)
                        {
                            sources.Add(data);
                        }
                    }
                }
                return sources.ToArray();
            });
        }
    }
}
