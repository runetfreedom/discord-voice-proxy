#include <windows.h>
#include <string>
#include <fstream>
#include <sstream>
#include <iostream>
#include <filesystem>
#include <detours/detours.h>
#include "proxy.h"

namespace fs = std::filesystem;

extern "C" BOOL(__stdcall * Real_CreateProcessW)(LPCWSTR a0,
	LPWSTR a1,
	LPSECURITY_ATTRIBUTES a2,
	LPSECURITY_ATTRIBUTES a3,
	BOOL a4,
	DWORD a5,
	LPVOID a6,
	LPCWSTR a7,
	LPSTARTUPINFOW a8,
	LPPROCESS_INFORMATION a9) = CreateProcessW;

HMODULE OriginalDLL;
uintptr_t OrignalDWriteCreateFactory;
uintptr_t OrignalGetFileVersionInfoSizeW;
uintptr_t OrignalGetFileVersionInfoW;
uintptr_t OrignalVerQueryValueW;

bool EndsWith(const std::wstring& fullString, const std::wstring& ending)
{
	if (ending.size() > fullString.size())
		return false;

	return fullString.compare(fullString.size() - ending.size(), ending.size(), ending) == 0;
}

BOOL __stdcall Mine_CreateProcessW(LPCWSTR lpApplicationName,
	LPWSTR lpCommandLine,
	LPSECURITY_ATTRIBUTES lpProcessAttributes,
	LPSECURITY_ATTRIBUTES lpThreadAttributes,
	BOOL bInheritHandles,
	DWORD dwCreationFlags,
	LPVOID lpEnvironment,
	LPCWSTR lpCurrentDirectory,
	LPSTARTUPINFOW lpStartupInfo,
	LPPROCESS_INFORMATION lpProcessInformation)
{
	;
	WCHAR path[MAX_PATH];
	GetModuleFileNameW(NULL, path, MAX_PATH);

	std::vector<std::wstring> files = {
		L"proxy.txt",
		L"force-proxy.dll",
		L"DWrite.dll"
	};

	if (lpApplicationName != NULL) {
		auto targetAppName = std::wstring(lpApplicationName);
		auto currentPath = std::wstring(path);

		if (EndsWith(targetAppName, L"Discord.exe") && targetAppName != currentPath) {
			auto currentDir = fs::path(currentPath).parent_path();
			auto targetDir = fs::path(targetAppName).parent_path();

			for (const auto& file : files) {
				if (fs::exists(currentDir.wstring() + L"\\" + file)) {
					fs::copy(currentDir.wstring() + L"\\" + file, targetDir.wstring() + L"\\" + file, fs::copy_options::overwrite_existing);
				}
			}
		}
	}

	return Real_CreateProcessW(lpApplicationName,
		lpCommandLine,
		lpProcessAttributes,
		lpThreadAttributes,
		bInheritHandles,
		dwCreationFlags,
		lpEnvironment,
		lpCurrentDirectory,
		lpStartupInfo,
		lpProcessInformation);
}

void LoadForceProxy()
{
	std::ifstream file("proxy.txt");
	std::string line;

	while (file.is_open() && getline(file, line)) {
		std::stringstream ss(line);

		std::string key, value;
		if (getline(ss, key, '=') && getline(ss, value)) {
			SetEnvironmentVariableA(key.c_str(), value.c_str());
		}
	}

	if (LoadLibraryA("force-proxy.dll") == NULL) {
		char text[MAX_PATH];
		_snprintf_s(text, sizeof(text), "Cannot load force-proxy.dll. Error code: %d", GetLastError());
		MessageBoxA(0, text, "Proxy", MB_ICONERROR);
	}
}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved) {
	char path[MAX_PATH];
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	{
		DisableThreadLibraryCalls(hModule);
		CopyMemory(path + GetSystemDirectoryA(path, MAX_PATH - 12), "\\DWrite.dll", 13);
		OriginalDLL = LoadLibraryA(path);
		if (OriginalDLL == NULL)
		{
			MessageBoxA(0, "Cannot load original version.dll", "Proxy", MB_ICONERROR);
			MessageBoxA(0, path, "Proxy", MB_ICONERROR);
			ExitProcess(0);
		}
		OrignalDWriteCreateFactory = (uintptr_t)GetProcAddress(OriginalDLL, "DWriteCreateFactory");

		LoadForceProxy();
		DetourRestoreAfterWith();
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
		DetourAttach((PVOID *)(&Real_CreateProcessW), Mine_CreateProcessW);
		DetourTransactionCommit();

		break;
	}
	case DLL_PROCESS_DETACH:
	{
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
		DetourDetach((PVOID*)(&Real_CreateProcessW), Mine_CreateProcessW);
		DetourTransactionCommit();

		FreeLibrary(OriginalDLL);

		break;
	}
	}
	return TRUE;
}