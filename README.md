**English** | [**Русский**](https://github.com/runetfreedom/discord-voice-proxy/blob/master/README.ru.md)

# What is This

This is a DLL for automatically loading [force-proxy](https://github.com/runetfreedom/force-proxy/) into the Discord process, along with an automatic installer.

This DLL handles Discord updates properly, so it works on a "set it and forget it" basis.

## Why

Discord voice chats do not support proxy connections, which forces users to route all computer traffic through a VPN or tunnel, which is highly inconvenient.

## How It Works

It using DLL Hijacking of the system library `DWrite.dll`.

This approach allows us to execute code inside the `Discord.exe` process, parse the configuration, and load `force-proxy.dll`, which intercepts Discord's network traffic and directs it through a SOCKS5 proxy.

# Installation

There are two installation methods:

### Automatic Installation

For convenience, there is a [ready-to-use installer](https://github.com/runetfreedom/discord-voice-proxy/releases/latest/download/DiscordProxyInstaller.exe). When launching it, you only need to enter the IP and port for your proxy, but the installer can automatically detect these settings if you are running one of the following clients:

1. v2rayN
2. NekoRay / NekoBox
3. Invisible Man - XRay (proxy mode must be set to SOCKS)

Note: You should have .NET 8 installed. If you don't have it installed, try downloading and installing it from [here](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).

Want to uninstall? Just run the installer again, and it will offer to remove the installed files.

### Manual Installation

Prefer manual installation? No problem.

1. First, download `DWrite.dll` and `force-proxy.dll` from the release page.
2. Open `%LocalAppData%\Discord` in File Explorer.
3. Find the `app` folder with the latest version and place both DLL files there.
4. Create a file named `proxy.txt` and enter the following:

 ```
 SOCKS5_PROXY_ADDRESS=YOUR_PROXY_IP
 SOCKS5_PROXY_PORT=YOUR_PROXY_PORT
 ```

Optionally, you can pass SOCKS5 credentials using:

```
SOCKS5_PROXY_LOGIN=login
SOCKS5_PROXY_PASSWORD=password
```

Don’t forget to restart Discord. Done!

# Troubleshooting

## Troubleshooting DNS Issues

If Discord works when using a TUN/VPN mode but gets stuck on "Checking for updates..." or repeatedly shows "Connection failed, retrying..." when using `discord-voice-proxy` with *known-working* SOCKS5 proxy without TUN, checking for local DNS poisoning is especially important.

`discord-voice-proxy` redirects Discord's connections through SOCKS5, but hostname resolution may still happen through the Windows DNS resolver before the connection is passed to the proxy.

In networks where DNS responses are filtered or poisoned, domains such as:

```
discord.com
updates.discord.com
cdn.discordapp.com
```

may resolve to incorrect or unrelated IP addresses. In this situation, `discord-voice-proxy` may still successfully send the connection through SOCKS5, but it will be connecting to the wrong destination.

You can diagnose this behavior further by looking up information about DNS pollution/poisoning and how to detect it.

### In this case:

Configure Windows to use a reliable DNS resolver, or only use the local DNS server for IPv4 & IPv6 provided by your proxy client.

If your DNS is being poisoned, then check both IPv4 & IPv6 resolvers, because Windows may still use router-provided IPv6 DNS.

After changing DNS settings, clear the Windows DNS cache:

```
ipconfig /flushdns
```

Then completely exit Discord and start it again.
