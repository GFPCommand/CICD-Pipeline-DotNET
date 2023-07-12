﻿using System.Diagnostics;
using System.Text.Json;

string pathFrom = string.Empty;
string pathTo = string.Empty;

string jsonLoad = File.ReadAllText("appsettings.json");

using JsonDocument doc = JsonDocument.Parse(jsonLoad);
JsonElement element = doc.RootElement;

pathFrom = element.GetProperty("CopyDirectorySettings").GetProperty("PathFrom").ToString();
pathTo = element.GetProperty("CopyDirectorySettings").GetProperty("PathTo").ToString();

FileSystemWatcher watcher;

try
{
    watcher = new FileSystemWatcher(pathFrom)
    {
        NotifyFilter = NotifyFilters.Attributes
                                | NotifyFilters.CreationTime
                                | NotifyFilters.DirectoryName
                                | NotifyFilters.FileName
                                | NotifyFilters.LastWrite
                                | NotifyFilters.Security
                                | NotifyFilters.Size
    };
}
catch (Exception)
{
    throw new Exception("Directory not found!\nPlease check pathFrom!");
}

watcher.Changed += OnChanged;
watcher.Renamed += OnRenamed;
watcher.Created += OnCreated;
watcher.Deleted += OnDeleted;

watcher.Filter = "*";
watcher.IncludeSubdirectories = true;
watcher.EnableRaisingEvents = true;

void OnChanged(object sender, FileSystemEventArgs e)
{
    if (e.ChangeType != WatcherChangeTypes.Changed) return;

    try
    {
        Pipeline();
    }
    catch (Exception ex)
    {
        watcher.Dispose();
        throw new Exception("Error in OnChanged method!\n Error code: \n" + ex.Message);
    }

    if (OperatingSystem.IsWindows()) {
        //Console.WriteLine("Changed");
    } else if (OperatingSystem.IsLinux())
    {
        File.AppendAllText(@"/home/user/smth.txt", "Changed\n");
    }
}

void OnRenamed(object sender, FileSystemEventArgs e)
{
    if (e.ChangeType != WatcherChangeTypes.Renamed) return;

    try
    {
        Pipeline();
    }
    catch (Exception ex)
    {
        watcher.Dispose();
        throw new Exception("Error in OnRenamed method!\n Error code: \n" + ex.Message);
    }

    if (OperatingSystem.IsWindows())
    {
        Console.WriteLine("Renamed");
    } else if (OperatingSystem.IsLinux())
    {
        File.AppendAllText(@"/home/user/smth.txt", "Renamed\n");
    }
}

void OnCreated(object sender, FileSystemEventArgs e)
{
    if (e.ChangeType != WatcherChangeTypes.Created) return;

    try
    {
        Pipeline();
    }
    catch (Exception ex) 
    {
        watcher.Dispose();
        throw new Exception("Error in OnCreated method!\n Error code: \n" + ex.Message);
    }

    if (OperatingSystem.IsWindows())
    {
        Console.WriteLine("Created");
    } else if (OperatingSystem.IsLinux())
    {
        File.AppendAllText(@"/home/user/smth.txt", "Created\n");
    }
}

void OnDeleted(object sender, FileSystemEventArgs e)
{
    if (e.ChangeType != WatcherChangeTypes.Deleted) return;

    try
    {
        Pipeline();
    }
    catch (Exception ex) 
    { 
        watcher.Dispose();
        throw new Exception("Error in OnDeleted method!\n Error code: \n" + ex.Message); 
    }

    if (OperatingSystem.IsWindows())
    {
        Console.WriteLine("Deleted");
    }
    else if (OperatingSystem.IsLinux())
    {
        File.AppendAllText(@"/home/user/smth.txt", "Deleted\n");
    }
}

void Pipeline()
{
    //CopyDirectory(pathFrom, pathTo);
    BuildProject();
    MoveDirectory();
}

void CopyDirectory(string sourcePath, string targetPath)
{
    foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
    {
        Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
    }

    //Copy all the files & Replaces any files with the same name
    foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
    {
        File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
    }
}

void BuildProject()
{
    using(Process winProcess = new()){
        ProcessStartInfo startInfo = new();
        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
        if (OperatingSystem.IsWindows())
        {
            startInfo.FileName = "cmd.exe";
        }
        else if (OperatingSystem.IsLinux())
        {
            startInfo.FileName = "/bin/bash";
        }
        startInfo.Arguments = $"/c cd \"{pathFrom}\"&dotnet publish -o \"{pathTo}\""; //looping while executing
        winProcess.StartInfo = startInfo;
        winProcess.Start();
    }
}

void MoveDirectory()
{

}

while (true)
{
    await Task.Delay(100);
}