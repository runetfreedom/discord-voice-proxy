using System.Diagnostics;

namespace DiscordProxyInstaller
{
    internal class Program
    {
        static bool Ask(string question, bool defaultValue = true)
        {
            var variants = defaultValue ? "Y/n" : "y/N";
            do {
                Console.Write($"{question} [{variants}]: ");
                var answer = (Console.ReadLine() ?? "").ToLower();
                if (answer == "") return defaultValue;
                if (answer == "y") return true;
                if (answer == "n") return false;
            } while (true);
        }

        static void Main(string[] args)
        {
            if (!Discord.Exists())
            {
                Console.WriteLine("Discord not found!");
                Console.ReadLine();
                return;
            }

            if (Discord.IsInstalled() && Ask("Already installed, want to uninstall?", false))
            {
                Discord.Uninstall();
                return;
            }

            var process = Process.GetProcesses().Select(p => p.ProcessName.ToLower()).ToList();

            if (process.Contains("v2rayn"))
            {
                if (Ask("Found v2rayN. Do you want to use 127.0.0.1:10808?"))
                {
                    Discord.InstallAndRun("127.0.0.1", "10808");
                    return;
                }
            }

            if (process.Contains("nekoray") || process.Contains("nekobox"))
            {
                if (Ask("Found NekoRay / NekoBox. Do you want to use 127.0.0.1:2080?"))
                {
                    Discord.InstallAndRun("127.0.0.1", "2080");
                    return;
                }
            }

            if (process.Contains("invisible man xray"))
            {
                if (Ask("Found Invisible Man - XRay. Do you want to use 127.0.0.1:10801?"))
                {
                    Discord.InstallAndRun("127.0.0.1", "10801");
                    return;
                }
            }

            Console.Write("Enter socks5 host: ");
            var host = Console.ReadLine();

            Console.Write("Enter socks5 port: ");
            var port = Console.ReadLine();

            Console.Write("Enter socks5 login (optional): ");
            var login = Console.ReadLine();

            Console.Write("Enter socks5 password (optional): ");
            var password = Console.ReadLine();

            Discord.InstallAndRun(host ?? "", port ?? "", login ?? "", password ?? "");
        }
    }
}
