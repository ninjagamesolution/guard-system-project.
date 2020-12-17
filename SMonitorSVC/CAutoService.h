
#pragma once
#pragma comment(lib,"WS2_32.lib") // link with Ws2_32.lib
#include <Winsock.h>
#include <Windows.h>
#include <process.h>
#include <string>
#include <vector>
#include <map>
#include <tlhelp32.h>
#include <psapi.h>
using namespace std;
#define PORT 9997

class CAutoService
{
public:
	CAutoService();
	~CAutoService();
public:
	//AutoService Part............................
	bool	static FindProcess();
	bool	static GetFileSource();
	void	Start();

	bool	static RunProcess();
	//PathService Part.............................
	DWORD	static WINAPIV Receive(LPARAM lpParam);
	DWORD	static WINAPIV Accept(LPVOID lpParam);
	int		static GetRegisterVersion();
	int		static SetRegisterVerion(char *version);
	void	static WriteFile(char * strPath, char* buf, int length);
	void	static AutoRunProcess(char * strPath);
	bool	StartSocket();
	int		SendData(char *buf, int len);
	DWORD	static FindProcessID();
	bool	static CloseProcess(DWORD dwProcessID);
private:
	void	GetServerIP();
	void	GetOwnerIP();

public:
	char	m_strClientProcess[20];
	WCHAR	m_wcsClientProcess[20];
	char	m_strServiceProcess[20];
	WCHAR	m_wcsServiceProcess[20];
	char	m_strOwnerIP[16];
	SOCKET	 m_Sock;
};

