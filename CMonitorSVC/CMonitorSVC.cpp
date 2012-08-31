#include <iostream>
#include <Windows.h>
#include <tchar.h>
#include "CAutoService.h"

SERVICE_STATUS        g_ServiceStatus = { 0 };
SERVICE_STATUS_HANDLE g_StatusHandle = NULL;
HANDLE                g_ServiceStopEvent = INVALID_HANDLE_VALUE;

CAutoService		*g_AutoClientService;

VOID WINAPI ServiceMain(DWORD argc, LPSTR *argv);
VOID WINAPI ServiceCtrlHandler(DWORD);
DWORD WINAPI ServiceWorkerThread(LPVOID lpParam);

void MainProc();

#define SERVICE_NAME  "CMonService"

int main()
{
	OutputDebugStringA("CMonService : Main: Entry");

	SERVICE_TABLE_ENTRYA ServiceTable[] =
	{
	  {(LPSTR)SERVICE_NAME, (LPSERVICE_MAIN_FUNCTIONA)ServiceMain},
	  {NULL, NULL}
	};

	if (StartServiceCtrlDispatcherA(ServiceTable) == FALSE)
	{
		OutputDebugStringA("CMonService: Main: StartServiceCtrlDispatcher returned error");
		return GetLastError();
	}

	OutputDebugStringA("CMonService: Main: Exit");
	return 0;
}

VOID WINAPI ServiceMain(DWORD argc, LPSTR *argv)
{
	DWORD Status = E_FAIL;
	HANDLE hThread;
	OutputDebugStringA("CMonServicee: ServiceMain: Entry");

	g_StatusHandle = RegisterServiceCtrlHandlerA(SERVICE_NAME, ServiceCtrlHandler);

	if (g_StatusHandle == NULL)
	{
		OutputDebugStringA("CMonService: ServiceMain: RegisterServiceCtrlHandler returned error");
		goto EXIT;
	}

	// Tell the service controller we are starting
	ZeroMemory(&g_ServiceStatus, sizeof(g_ServiceStatus));
	g_ServiceStatus.dwServiceType = SERVICE_WIN32_OWN_PROCESS;
	g_ServiceStatus.dwControlsAccepted = 0;
	g_ServiceStatus.dwCurrentState = SERVICE_START_PENDING;
	g_ServiceStatus.dwWin32ExitCode = 0;
	g_ServiceStatus.dwServiceSpecificExitCode = 0;
	g_ServiceStatus.dwCheckPoint = 0;

	if (SetServiceStatus(g_StatusHandle, &g_ServiceStatus) == FALSE)
	{
		OutputDebugStringA("CMonService: ServiceMain: SetServiceStatus returned error");
	}

	/*
	 * Perform tasks neccesary to start the service here
	 */
	OutputDebugStringA("CMonService: ServiceMain: Performing Service Start Operations");

	// Create stop event to wait on later.
	g_ServiceStopEvent = CreateEvent(NULL, TRUE, FALSE, NULL);
	if (g_ServiceStopEvent == NULL)
	{
		OutputDebugStringA("CMonService: ServiceMain: CreateEvent(g_ServiceStopEvent) returned error");

		g_ServiceStatus.dwControlsAccepted = 0;
		g_ServiceStatus.dwCurrentState = SERVICE_STOPPED;
		g_ServiceStatus.dwWin32ExitCode = GetLastError();
		g_ServiceStatus.dwCheckPoint = 1;

		if (SetServiceStatus(g_StatusHandle, &g_ServiceStatus) == FALSE)
		{
			OutputDebugStringA("CMonService: ServiceMain: SetServiceStatus returned error");
		}
		goto EXIT;
	}

	// Tell the service controller we are started
	g_ServiceStatus.dwControlsAccepted = SERVICE_ACCEPT_STOP;
	g_ServiceStatus.dwCurrentState = SERVICE_RUNNING;
	g_ServiceStatus.dwWin32ExitCode = 0;
	g_ServiceStatus.dwCheckPoint = 0;

	if (SetServiceStatus(g_StatusHandle, &g_ServiceStatus) == FALSE)
	{
		OutputDebugStringA("CMonService: ServiceMain: SetServiceStatus returned error");
	}

	// Start the thread that will perform the main task of the service
	hThread = CreateThread(NULL, 0, ServiceWorkerThread, NULL, 0, NULL);

	OutputDebugStringA("CMonService: ServiceMain: Waiting for Worker Thread to complete");

	// Wait until our worker thread exits effectively signaling that the service needs to stop
	WaitForSingleObject(hThread, INFINITE);

	OutputDebugStringA("CMonService: ServiceMain: Worker Thread Stop Event signaled");


	/*
	 * Perform any cleanup tasks
	 */
	OutputDebugStringA("CMonService: ServiceMain: Performing Cleanup Operations");

	CloseHandle(g_ServiceStopEvent);

	g_ServiceStatus.dwControlsAccepted = 0;
	g_ServiceStatus.dwCurrentState = SERVICE_STOPPED;
	g_ServiceStatus.dwWin32ExitCode = 0;
	g_ServiceStatus.dwCheckPoint = 3;

	if (SetServiceStatus(g_StatusHandle, &g_ServiceStatus) == FALSE)
	{
		OutputDebugStringA("CMonService: ServiceMain: SetServiceStatus returned error");
	}

EXIT:
	OutputDebugStringA("CMonService: ServiceMain: Exit");

	return;
}

VOID WINAPI ServiceCtrlHandler(DWORD CtrlCode)
{
	OutputDebugStringA("CMonService: ServiceCtrlHandler: Entry");

	switch (CtrlCode)
	{
	case SERVICE_CONTROL_STOP:

		OutputDebugStringA("CMonService: ServiceCtrlHandler: SERVICE_CONTROL_STOP Request");

		if (g_ServiceStatus.dwCurrentState != SERVICE_RUNNING)
			break;
		/*
		 * Perform tasks neccesary to stop the service here
		 */

		g_ServiceStatus.dwControlsAccepted = 0;
		g_ServiceStatus.dwCurrentState = SERVICE_STOP_PENDING;
		g_ServiceStatus.dwWin32ExitCode = 0;
		g_ServiceStatus.dwCheckPoint = 4;

		if (SetServiceStatus(g_StatusHandle, &g_ServiceStatus) == FALSE)
		{
			OutputDebugStringA("CMonService: ServiceCtrlHandler: SetServiceStatus returned error");
		}

		// This will signal the worker thread to start shutting down
		SetEvent(g_ServiceStopEvent);

		break;

	default:
		break;
	}

	OutputDebugStringA("CMonService: ServiceCtrlHandler: Exit");
}



DWORD WINAPI ServiceWorkerThread(LPVOID lpParam)
{
	g_AutoClientService = new CAutoService;
	OutputDebugStringA("CMonService: ServiceWorkerThread: Entry");

	while (WaitForSingleObject(g_ServiceStopEvent, 0) != WAIT_OBJECT_0)
	{
		if (g_ServiceStatus.dwCurrentState == SERVICE_RUNNING) {
			MainProc();
		}
		
	}

	OutputDebugStringA("CMonService: ServiceWorkerThread: Exit");

	return ERROR_SUCCESS;
}



void MainProc() {

	g_AutoClientService->Start();
	Sleep(60 * 1000);
	return;
}

