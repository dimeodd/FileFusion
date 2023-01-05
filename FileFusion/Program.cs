using System;
using System.IO;

namespace FileFusion
{

  class Program
  {
    public const string theLine = "======================================";
    public const string oldFolderName = "OLD_DATA_FOLDER";
    public const string oldName = "— The Old";


    static void Main(string[] args)
    {
      DirectoryInfo SettingsPatn = Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/FusionAppC");

      Console.WriteLine(Title("FusionApp V0.0.1!"));
      Console.WriteLine("What do you need?" +
        "\n1) Fusion data" +
        "\n2) Update data" +
        "\n3) Drives List"
      );
      int select = DataTools.Read_IntSelect(1, 3);
      if (select == 3)
        GetDrivesList();
      else if (select < 3)
      {




        if (select == 1)
        {
          string folder_a = null;
          string folder_b = null;

          FileInfo[] DirPaths = Array.FindAll(SettingsPatn.GetFiles(), x => x.Name.EndsWith(".txt"));
          if (DirPaths.Length > 0)
            using (StreamReader sr = new StreamReader(DirPaths[0].FullName))
            {
              folder_a = sr.ReadLine();
              folder_b = sr.ReadLine();
              sr.Close();

              if (Directory.Exists(folder_a) & Directory.Exists(folder_b))
              {
                Console.WriteLine("Use previosle Ways?" +
                $"\n{folder_a}" +
                $"\n{folder_b}");

                if (!DataTools.Read_yn())
                {
                  folder_a = null;
                  folder_b = null;
                }
              }
              else
              {
                folder_a = null;
                folder_b = null;
              }
            }

          if (folder_a == null || folder_b == null)
          {
            #region Select folders Mannualy
            folder_a = DataTools.GetFolderPath("\nChose Folder A");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(Program.theLine + "\nSelected folder A: " + folder_a);
            Console.ForegroundColor = ConsoleColor.White;
            folder_b = DataTools.GetFolderPath("\nChose Folder B");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(Program.theLine + "\nSelected folder A: " + folder_a);
            Console.WriteLine("Selected folder B: " + folder_b);
            Console.ForegroundColor = ConsoleColor.White;

            using (StreamWriter sw = new StreamWriter(SettingsPatn + "/temp.txt", false, System.Text.Encoding.Default))
            {
              sw.WriteLine(folder_a);
              sw.WriteLine(folder_b);
              sw.Close();
            }
            #endregion
          }

          string message = "move old files in specal folder?\n1)yes\n2)no";
          Console.WriteLine(message);
          int a_select = DataTools.Read_IntSelect(1, 2, message);
          if (a_select == 1)
            DataTools.FusionData(folder_a, folder_b, null, null, oldFolderName);
          else if (a_select == 2)
            DataTools.FusionData(folder_a, folder_b, null, null);

          Console.ForegroundColor = ConsoleColor.Green;
          Console.WriteLine(Title("Fusion Complete"));
          Console.WriteLine($"{folder_a}\n{folder_b}");
        }
        //else if (select == 2)
        //  DataTools.UpdateData(folder_a, folder_b);
      }
      Console.WriteLine("Press any key to exit");
      Console.ReadKey();
    }

    public static string Title(string title)
    {
      title = $" {title} ";
      if (theLine.Length >= title.Length)
      {
        string outtext = theLine.Remove(0, title.Length);
        outtext = outtext.Insert(outtext.Length / 2, title);
        return outtext;
      }
      else
      {
        return string.Format("{0}\n{1}\n{0}", theLine, title);
      }
    }

    static void GetDrivesList()
    {
      DriveInfo[] drives = DriveInfo.GetDrives();
      Console.WriteLine(Program.Title("Drives Info"));
      foreach (var drive in drives)
      {
        Console.WriteLine($"\nDisk: {drive.Name}");
        Console.WriteLine($"Type: {drive.DriveType}");
        if (drive.IsReady)
        {
          Console.WriteLine($"File format: {drive.DriveFormat}");
          if (drive.VolumeLabel != "")
            Console.WriteLine($"Label: {drive.VolumeLabel}");
          else
            Console.WriteLine($"Label: Not Set");
        }
      }
      Console.WriteLine("\n");
    }


  }
}

