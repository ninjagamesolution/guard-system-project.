#include "CAutoService.h"
#include <psapi.h>
#include <WtsApi32.h>
#include <direct.h>
#pragma comment(lib, "WtsApi32.lib")


char m_strServerIP[16];
char m_fileName[50] = { 0 };
HANDLE	hReceive;
char	m_strVersion[20];
HANDLE hThread;
bool bActive = false;
bool bFlag = true;

CAutoService::CAutoService()
{
	bActive = false;
}

CAutoService::~CAutoService()
{
	closesocket(m_Sock);
	CloseHandle(hReceive);
	CloseHandle(hThread);

}



void CAutoService::Start()
{
	struct stat info;
	if (stat("C:\\Program Files (x86)\\RM", &info) != 0) {
		_mkdir("C:\\Program Files (x86)\\RM");
	}
	if (stat("C:\\Program Files (x86)\\RM\\Server", &info) != 0) {
		_mkdir("C:\\Program Files (x86)\\RM\\Server");
	}

	GetServerIP();
	if (bActive == false) {
		StartSocket();
	}

	DWORD bClientProcess = FindProcessID();
	if (bClientProcess == 0) {
		RunProcess();
	}


}


bool CAutoService::RunProcess()
{
	WCHAR m_wcsProcessName[25] = { 0 };
	WCHAR m_wcsProcessPath[70] = { 0 };

	wsprintf(m_wcsProcessName, L"Monitor.Control.exe");
	wsprintf(m_wcsProcessPath, L"C:\\Program Files (x86)\\RM\\Server\\Monitor.Control.exe");
	PROCESS_INFORMATION pi;
	STARTUPINFO si;
	DWORD nsid = 1;

	ZeroMemory(&si, sizeof(si));
	si.cb = sizeof(si);
	HANDLE htoken;

	DWORD sessionId;

	sessionId = WTSGetActiveConsoleSessionId();
	WTSQueryUserToken(sessionId, &htoken);
	si.wShowWindow = TRUE;
	if (CreateProcessAsUser(htoken, m_wcsProcessPath, NULL, NULL,
		NULL, FALSE, 0, NULL, NULL, &si, &pi))
	{
		/* Process has been created; work with the process and wait for it to
		terminate. */
		WaitForSingleObject(pi.hProcess, INFINITE);
		CloseHandle(pi.hThread);
		CloseHandle(pi.hProcess);
		CloseHandle(htoken);
		return true;
	}
	else {
		CloseHandle(htoken);
		return false;
	}
	//CreateProcess Part...........
}



void CAutoService::GetServerIP()
{
	HKEY hKey = 0;
	char buf[MAX_PATH] = { 0 };
	char IP[16] = { 0 };
	DWORD dwType = 0;
	DWORD dwBufSize = sizeof(buf);
	DWORD dwIPSize = sizeof(IP);
	DWORD dw = sizeof(DWORD), dwData = 0;
	const char* subkey = "Ryonbong\\Monitor";

	if (RegOpenKeyA(HKEY_CURRENT_USER, subkey, &hKey) == ERROR_SUCCESS)
	{
		dwType = REG_SZ; //REG_BINARY
		RegQueryValueExA(hKey, "ServerIP", 0, &dwType, (BYTE*)IP, &dwIPSize);
		sprintf_s(m_strServerIP, buf);
		RegCloseKey(hKey);
	}
	else
	{
		sprintf_s(m_strServerIP, "127.0.0.1");
	}

}

void CAutoService::GetOwnerIP()
{

}


bool CAutoService::FindProcess()
{
	WCHAR m_wcsProcessName[40] = { 0 };
	wsprintf(m_wcsProcessName, L"Monitor.Control.exe");

	DWORD aProcesses[1024], cbNeeded, cProcesses;
	unsigned int i;
	if (!EnumProcesses(aProcesses, sizeof(aProcesses), &cbNeeded))
		return false;
	else
		wprintf(L"EnumProcesses() is OK!\n");
	bool bFlag = false;
	// Calculate how many process identifiers were returned.
	cProcesses = cbNeeded / sizeof(DWORD);
	// Print the name and process identifier for each process.
	for (i = 0; i < cProcesses; i++) {
		if (aProcesses[i] != 0) {
			WCHAR szProcessName[MAX_PATH] = L"";
			static int i;
			HMODULE hMod;
			DWORD cbNeeded;
			// Get a handle to the process.
			HANDLE hProcess = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, FALSE, aProcesses[i]);
			// Get the process name.
			if (hProcess != NULL)
			{
				if (EnumProcessModules(hProcess, &hMod, sizeof(hMod), &cbNeeded))
				{
					GetModuleBaseName(hProcess, hMod, szProcessName, sizeof(szProcessName) / sizeof(WCHAR));
				}
			}
			// Print the process name and identifier.

			if (wcscmp(szProcessName, L"") != 0) {

				if (wcscmp(m_wcsProcessName, szProcessName) == 0) {
					bFlag = true;
					break;
				}
			}
		}

	}
	return bFlag;
}

bool CAutoService::GetFileSource()
{
	return true;
}

//PatchService Part...................................

DWORD WINAPIV CAutoService::Accept(LPVOID lpParam)
{
	struct sockaddr_in client_addr;
	SOCKET socket = (SOCKET)lpParam;
	SOCKET client_socket;
	int addr_size = sizeof(client_addr);
	while (1) {
		client_socket = accept(socket, (struct sockaddr*)&client_addr, &addr_size);
		if (bFlag == false) {
			continue;
		}
		DWORD dwProcessID = FindProcessID();
		if (dwProcessID > 0) {
			CloseProcess(dwProcessID);
		}
		if (client_socket > 0) {
			DWORD dwClientThreadID;
			CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)Receive, (LPVOID)client_socket, 0, &dwClientThreadID);
		}
	}


	return 0;
}
bool CAutoService::CloseProcess(DWORD dwProcessID) {
	DWORD dwDesiredAccess = PROCESS_TERMINATE;
	BOOL  bInheritHandle = FALSE;
	HANDLE hProcess = OpenProcess(dwDesiredAccess, bInheritHandle, dwProcessID);
	if (hProcess == NULL)
		return FALSE;

	BOOL result = TerminateProcess(hProcess, 1);

	CloseHandle(hProcess);

	return result;
}
DWORD WINAPIV CAutoService::Receive(LPARAM lpParam)
{
	SOCKET client_socket = (SOCKET)lpParam;
	int length = 0;
	char buf[1024];
	memset(buf, 0, sizeof(buf));
	int size_recv;
	FILE *fp;
	fopen_s(&fp, "C:\\Program Files (x86)\\RM\\Server\\Monitor.Control.exe", "wb");
	bFlag = false;
	while (1)
	{
		size_recv = recv(client_socket, buf, sizeof(buf), 0);
		if (size_recv < 0)
		{
			break;
		}
		else if (strcmp(buf, "end") == 0) {

			fclose(fp);
			break;
		}
		else if (strcmp(buf, "*stop*") == 0) {
			DWORD dwProcessID = FindProcessID();
			if (dwProcessID > 0) {
				CloseProcess(dwProcessID);
			}
			break;
		}
		else {
			int write_sz = fwrite(buf, sizeof(char), size_recv, fp);
			memset(buf, 0, sizeof(buf));
		}
	}
	bFlag = true;
	closesocket(client_socket);
	CloseHandle(hReceive);

	return 0;
}

bool CAutoService::StartSocket()
{
	WSADATA wsaData;
	m_Sock = INVALID_SOCKET;
	struct sockaddr_in m_ServerAddr;
	int opt = 1;
	int iResult;
	// Initialize Winsock
	iResult = WSAStartup(MAKEWORD(2, 2), &wsaData);
	if (iResult != 0) {
		return false;
	}

	memset(&m_ServerAddr, '\0', sizeof(m_ServerAddr));

	if ((m_Sock = socket(AF_INET, SOCK_STREAM, 0)) == INVALID_SOCKET)
		return false;

	char host[256];
	char *IP;
	struct hostent  *host_entry;
	int hostname;
	hostname = gethostname(host, sizeof(host));
	if (hostname == -1) {
		printf("GetHost Name");
		return false;
	}
	host_entry = gethostbyname(host);
	if (host_entry == NULL) {
		return false;
	}


	m_ServerAddr.sin_family = AF_INET;
	m_ServerAddr.sin_addr.s_addr = INADDR_ANY;
	m_ServerAddr.sin_port = htons(PORT);

	int result = bind(m_Sock, (struct sockaddr *)&m_ServerAddr, sizeof(m_ServerAddr));
	if (result < 0) {
		printf_s("Failed binding socket\n");
		return false;
	}

	listen(m_Sock, 512);

	int size_recv, total_size = 0;

	DWORD dwThread;
	CreateThread(NULL, NULL, (LPTHREAD_START_ROUTINE)Accept, (LPVOID)m_Sock, 0, &dwThread);
	bActive = true;
	return true;
}

int CAutoService::SendData(char * buf, int len)
{
	int total = 0;
	int bytesleft = len;
	int n;

	while (total < len) {
		n = send(m_Sock, buf + total, bytesleft, 0);
		if (n == -1) { break; }
		total += n;
		bytesleft -= n;
	}

	len = total;
	return total;
}





DWORD CAutoService::FindProcessID()
{
	PROCESSENTRY32 processInfo;
	processInfo.dwSize = sizeof(processInfo);

	HANDLE processesSnapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, NULL);
	if (processesSnapshot == INVALID_HANDLE_VALUE) {
		return 0;
	}
	WCHAR processName[22] = L"Monitor.Control.exe";
	Process32First(processesSnapshot, &processInfo);
	if (wcscmp(processName, processInfo.szExeFile) == 0) {
		CloseHandle(processesSnapshot);
		return processInfo.th32ProcessID;
	}


	while (Process32Next(processesSnapshot, &processInfo))
	{
		if (wcscmp(processName, processInfo.szExeFile) == 0)
		{
			CloseHandle(processesSnapshot);
			return processInfo.th32ProcessID;
		}
	}

	CloseHandle(processesSnapshot);
	return 0;
}

int CAutoService::GetRegisterVersion()
{
	HKEY hKey = 0;
	char buf[MAX_PATH] = { 0 };
	char IP[16] = { 0 };
	DWORD dwType = 0;
	DWORD dwBufSize = sizeof(buf);
	DWORD dwIPSize = sizeof(IP);
	DWORD dw = sizeof(DWORD), dwData = 0;
	const char* subkey = "Ryonbong\\CAutoService";

	if (RegOpenKeyA(HKEY_CURRENT_USER, subkey, &hKey) == ERROR_SUCCESS)
	{
		dwType = REG_SZ; //REG_BINARY
		RegQueryValueExA(hKey, "Version", 0, &dwType, (BYTE*)buf, &dwBufSize);
		sprintf_s(m_strVersion, buf);
		RegCloseKey(hKey);
	}
	else
	{
		sprintf_s(m_strVersion, "1.0");
	}

	return 0;
}

int CAutoService::SetRegisterVerion(char * version)
{
	HKEY hk;
	DWORD dwDisp;

	RegCreateKeyExA(HKEY_CURRENT_USER, "Ryonbong\\CAutoService", 0, NULL, REG_OPTION_NON_VOLATILE, KEY_WRITE, NULL, &hk, &dwDisp);

	RegSetValueExA(hk, "Version", 0, REG_SZ, (PBYTE)&version, sizeof(version));
	return 0;
}



void CAutoService::WriteFile(char * strPath, char * buf, int length)
{
}

void CAutoService::AutoRunProcess(char * strPath)
{
	WCHAR m_wcsProcessName[25] = { 0 };
	WCHAR m_wcsProcessPath[50] = { 0 };
	wsprintf(m_wcsProcessName, L"Monitor.TaskView.exe");
	wsprintf(m_wcsProcessPath, L"C:\\Program Files (x86)\\RM\\Server\\Monitor.Control.exe");
	STARTUPINFOW process_startup_info{ 0 };
	process_startup_info.cb = sizeof(process_startup_info); // setup size of strcture in bytes

	PROCESS_INFORMATION process_info{ 0 };
	WCHAR m_wcsCommand[40] = { 0 };
	size_t convertedChars = 0;

	if (CreateProcessW((LPWSTR)m_wcsProcessPath, NULL, NULL, NULL, TRUE, 0, NULL, NULL, &process_startup_info, &process_info))
	{
		WaitForSingleObject(process_info.hProcess, INFINITE); // uncomment to wait till process finish
		CloseHandle(process_info.hProcess);
		CloseHandle(process_info.hThread);

	}
}
