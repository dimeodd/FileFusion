using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FileFusion
{
  public class FolderTool
  {
    public static void a_Main(out string dirPath)
    {
      dirPath = SelectFolder();

    }

    static string SelectDisk()
    {

      Console.WriteLine(Program.Title("Select disk"));
      string helloMesage = "";
      DriveInfo[] drives = DriveInfo.GetDrives();
      int i = 0;
      foreach (var drive in drives)
      {
        i++;
        helloMesage += $"\n{i}) {drive.Name}";
      }
      Console.WriteLine(helloMesage);

      int select = DataTools.Read_IntSelect(1, drives.Length, helloMesage);
      return drives[select - 1].Name;
    }

    static string SelectFolder(string path = null)
    {
      if (path == null)
        return SelectFolder(SelectDisk());

      string helloMesage = Program.Title($"Selected folder:  \"{path}\"") + "\n";
      int select;

      if (path.Length > 3)
        helloMesage += "\n0) Back";
      else
        helloMesage += "\n0) Change Disk";

      helloMesage += $"\n1) =>> Select this folder\n";
      string[] dirs = Directory.GetDirectories(path);
      int i = 1;
      foreach (string s in dirs)
      {
        i++;
        helloMesage += $"\n{i}) {s}";
      }
      Console.WriteLine(helloMesage);

      select = DataTools.Read_IntSelect(0, dirs.Length + 1, helloMesage);

      if (select == 0)
      {
        if (path.Length > 3)
        {
          Console.WriteLine($"You select parent folder: {Directory.GetParent(path)}");
          return SelectFolder($"{Directory.GetParent(path)}");
        }
        else
        {
          return SelectFolder(SelectDisk());
        }
      }
      else if (select == 1)
        return path;
      else
      {
        try
        {
          return SelectFolder(dirs[select - 2]);
        }
        catch
        {
          Console.WriteLine($"Access denied, programm haven't permission to open \"{dirs[select - 2]}\" folder");
          return SelectFolder(path);
        }
      }


    }

    public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
    {
      // Get the subdirectories for the specified directory.
      DirectoryInfo dir = new DirectoryInfo(sourceDirName);

      if (!dir.Exists)
      {
        throw new DirectoryNotFoundException(
            "Source directory does not exist or could not be found: "
            + sourceDirName);
      }

      DirectoryInfo[] dirs = dir.GetDirectories();

      Directory.CreateDirectory(destDirName); // If the destination directory doesn't exist, create it.       

      // Get the files in the directory and copy them to the new location.
      FileInfo[] files = dir.GetFiles();
      foreach (FileInfo file in files)
      {
        string tempPath = Path.Combine(destDirName, file.Name);
        file.CopyTo(tempPath, false);
      }

      if (copySubDirs)
      {
        foreach (DirectoryInfo subdir in dirs)
        {
          string tempPath = Path.Combine(destDirName, subdir.Name);
          DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
        }
      }
    }

  }
}
