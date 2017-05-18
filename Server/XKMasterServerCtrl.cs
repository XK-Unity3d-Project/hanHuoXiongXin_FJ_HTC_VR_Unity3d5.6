using System.Diagnostics;
using UnityEngine;

public class XKMasterServerCtrl
{
	static bool IsOpenMasterServer;
	public static void CheckMasterServerIP()
	{
		if (IsOpenMasterServer) {
			return;
		}
		IsOpenMasterServer = true;
		KillSystemProcess("MasterServer");
		KillSystemProcess("Facilitator");
		OpenGameProcess("MasterServer/MasterServer.exe");
		OpenGameProcess("Facilitator/Facilitator.exe");
	}

	public static void CloseMasterServer()
	{
		KillSystemProcess("MasterServer");
		KillSystemProcess("Facilitator");
	}
	
	public static void KillSystemProcess(string processName)
	{
		if (processName == "") {
			return;
		}

		Process[] myProcesses = Process.GetProcesses();
		//UnityEngine.Debug.Log("Length ** "+myProcesses.Length);
		foreach (Process process in myProcesses) {
			try
			{
				if (!process.HasExited)
				{
					//UnityEngine.Debug.Log("name --- "+process.ProcessName);
					if (process.ProcessName == processName) {
						process.Kill();
					}
				}
			}
			catch (System.InvalidOperationException)
			{
				//UnityEngine.Debug.Log("Holy batman we've got an exception!");
			}
		}
	}
	
	
	public static void OpenGameProcess(string processName, bool isCreateWindow = false)
	{
		if (processName == "") {
			return;
		}
		UnityEngine.Debug.Log("OpenGameProcess -> processName "+processName);

		Process p = new Process();
		//p.StartInfo.FileName = "MasterServer/MasterServer.exe";
		p.StartInfo.FileName = processName;
		p.StartInfo.UseShellExecute = false;
		p.StartInfo.RedirectStandardInput = true;
		p.StartInfo.RedirectStandardOutput = true;
		p.StartInfo.RedirectStandardError = true;
		p.StartInfo.CreateNoWindow = !isCreateWindow;//true表示不显示黑框，false表示显示dos界面
		
		p.Start();
		//要执行的dos命令  p.StandardInput.WriteLine("");
		//p.StandardInput.WriteLine("exit");
		//p.Close();
	}
}