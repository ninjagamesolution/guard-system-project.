using Microsoft.Win32;
using Monitor.TaskControl.myEvents;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Monitor.TaskControl.Utils
{
    public static class EnvironmentHelper
    {
        //Loader shutdown
        public static void ShutDown(bool triggerEvent = false)
        {
            if (triggerEvent)
            {
                Events.RaiseOnExit(null);
            }
            Environment.Exit(0);
        }
        public static void Restart(bool triggerEvent = false)
        {
            if (triggerEvent)
            {
                Events.RaiseOnExit(null);
            }

            Process.Start(FileName);
            Environment.Exit(0);
        }
        public static void SetValidEnvironment()
        {
            Environment.CurrentDirectory = Path.GetDirectoryName(FileName);
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
        }
        public static Version GetAssemblyVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }

        public static string FileName
        {
            get { return Process.GetCurrentProcess().MainModule.FileName; }
        }

        public static string GetMachineGuid()
        {
            const string location = @"SOFTWARE\Microsoft\Cryptography";
            const string name = "MachineGuid";

            using (var localMachineX64View = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                using (var rk = localMachineX64View.OpenSubKey(location))
                {
                    if (rk == null)
                    {
                        return "failed";
                    }

                    var machineGuid = rk.GetValue(name);

                    if (machineGuid == null)
                    {
                        return "failed2";
                    }

                    return machineGuid.ToString();
                }
            }
        }
    }

    public static class RandomHelper
    {
        public static Random Random { get; private set; }

        static RandomHelper()
        {
            Random = new Random(Environment.TickCount);
        }

        public static string RandomString(int length = 15, char[] chars = null)
        {
            var str = new StringBuilder();
            chars = chars ?? "abcdefghijklmnopqrstvuwxyzABCDEFGHIJKLMNOPQRSTVUWXYZ0123456789".ToCharArray();

            for (var i = 0; i < length; i++)
            {
                str.Append(chars[Random.Next(chars.Length)]);
            }

            return str.ToString();
        }
    }

    internal static class FileHelper
    {
        internal static void SafeWriteAllBytes(string path, byte[] bytes)
        {
            if (File.Exists(path))
            {
                //var temp = Path.Combine(Settings.Instance.Directories.TempDirectory, RandomHelper.RandomString() + Path.GetExtension(path));
                //File.Move(path, temp);

                try
                {
                    //File.Delete(temp);
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            File.WriteAllBytes(path, bytes);
        }
        internal static void CopyFile(string file, string directory, string name = "", bool overrideFile = true)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = Path.GetFileName(file);
            }

            var newFilePath = Path.Combine(directory, name);

            if (!overrideFile && File.Exists(newFilePath))
            {
                throw new Exception(String.Format("The file {0} already exists.", newFilePath));
            }

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllBytes(newFilePath, File.ReadAllBytes(file));
        }

        internal static void SafeCopyFile(string file, string directory, string name = "", bool overrideFile = true)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = Path.GetFileName(file);
            }

            var newFilePath = Path.Combine(directory, name);

            if (!overrideFile && File.Exists(newFilePath))
            {
                throw new Exception(string.Format("The file {0} already exists.", newFilePath));
            }

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            SafeWriteAllBytes(newFilePath, File.ReadAllBytes(file));
        }

        internal static void SafeMoveFolder(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    SafeMoveFolder(subdir.FullName, tempPath, copySubDirs);
                }
            }

            //Delete Source directory
            Directory.Delete(sourceDirName, true);
        }
    }
}