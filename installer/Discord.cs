using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DiscordProxyInstaller
{
    internal class Discord
    {
        public static bool Exists()
        {
            return Directory.Exists(GetRootDirectory()) && Directory.Exists(GetAppDirectory());
        }

        public static string GetRootDirectory()
        {
            return Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Discord");
        }

        public static string GetAppDirectory()
        {
            var root = GetRootDirectory();
            var result = "";

            foreach (var item in Directory.GetDirectories(root))
            {
                var dir = Path.GetFileName(item);
                if (dir.StartsWith("app-"))
                {
                    var version = dir.Split("app-")[1];
                    if (result == "" || new Version(result).CompareTo(new Version(version)) < 0)
                    {
                        result = version;
                    }
                }
            }

            return Path.Join(root, "app-" + result);
        }

        public static void Install(string host, string port, string login = "", string password = "")
        {
            var appDir = GetAppDirectory();

            WriteResourceToFile("DWrite.dll", Path.Join(appDir, "DWrite.dll"));
            WriteResourceToFile("force-proxy.dll", Path.Join(appDir, "force-proxy.dll"));

            var lines = new string[] {
                "SOCKS5_PROXY_ADDRESS=" + host,
                "SOCKS5_PROXY_PORT=" + port,
                "SOCKS5_PROXY_LOGIN=" + login,
                "SOCKS5_PROXY_PASSWORD=" + password,
            };
            File.WriteAllLines(Path.Join(appDir, "proxy.txt"), lines);
        }

        public static void Run()
        {
            var appDir = GetAppDirectory();
            var exePath = Path.Combine(appDir, "Discord.exe");

            Process process = new Process();
            process.StartInfo.FileName = exePath;
            process.StartInfo.WorkingDirectory = appDir;
            process.Start();
        }

        public static void InstallAndRun(string host, string port, string login = "", string password = "")
        {
            Kill();
            Install(
              host,
              port,
              !string.IsNullOrEmpty(login) ? login : "empty",
              !string.IsNullOrEmpty(password) ? password : "empty"
            );
            Run();
        }

        public static bool IsInstalled()
        {
            return File.Exists(Path.Join(GetAppDirectory(), "DWrite.dll"));
        }

        public static void Uninstall()
        {
            Kill();
            File.Delete(Path.Join(GetAppDirectory(), "DWrite.dll"));
        }

        public static void Kill()
        {
            foreach (var process in Process.GetProcessesByName("Discord"))
            {
                process.Kill();
            }

            Thread.Sleep(1000);
        }

        private static void WriteResourceToFile(string resourceName, string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var resource = assembly.GetManifestResourceStream(assembly.GetName().Name + "." + resourceName))
            {
                using (var file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    resource.CopyTo(file);
                }
            }
        }
    }
}
