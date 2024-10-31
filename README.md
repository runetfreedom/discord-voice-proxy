**English** | [**Русский**](https://github.com/runetfreedom/force-proxy/blob/master/README.md)

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

Don’t forget to restart Discord. Done!