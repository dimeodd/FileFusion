using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FileFusion
{
  class DataTools
  {
    public static void FusionData(string Folder_a, string Folder_b, DirectoryInfo oldPath_A, DirectoryInfo oldPath_B, string OldFolderName = null)
    {
      DirectoryInfo dirA = new DirectoryInfo(Folder_a);
      DirectoryInfo dirB = new DirectoryInfo(Folder_b);

      if (OldFolderName != null && oldPath_A == null && oldPath_B == null)
      {
        oldPath_A = Directory.CreateDirectory(Folder_a + "\\" + OldFolderName);
        oldPath_B = Directory.CreateDirectory(Folder_b + "\\" + OldFolderName);
      }

      if (!dirA.Exists)
      {
        throw new DirectoryNotFoundException(
            "Source directory does not exist or could not be found: "
            + Folder_a);
      }
      if (!dirB.Exists)
      {
        throw new DirectoryNotFoundException(
            "Source directory does not exist or could not be found: "
            + Folder_b);
      }

      FileInfo[] files_A = dirA.GetFiles();
      FileInfo[] files_B = dirB.GetFiles();

      //Copy & replace files from Folder_A
      foreach (FileInfo a_file in files_A)
      {
        FileInfo b_file = Array.Find(files_B, x => x.Name == a_file.Name);

        if (b_file != null)
        {
          if (b_file.LastWriteTime > a_file.LastWriteTime)
          {
            if (oldPath_A != null) ReplaceFile(a_file, oldPath_A.FullName, oldPath_B.FullName);
            else ReplaceFile(a_file, a_file.DirectoryName, b_file.DirectoryName);

            string tempPath = Path.Combine(Folder_a, b_file.Name);
            b_file.CopyTo(tempPath, false);
          }
          else if (b_file.LastWriteTime < a_file.LastWriteTime)
          {
            if (oldPath_B != null) ReplaceFile(b_file, oldPath_B.FullName, oldPath_A.FullName);
            else ReplaceFile(b_file, b_file.DirectoryName, a_file.DirectoryName);
            string tempPath = Path.Combine(Folder_b, a_file.Name);
            a_file.CopyTo(tempPath, false);
          }
          else //else it same files
            continue;
        }
        else
        {
          a_file.CopyTo(Folder_b + "\\" + a_file.Name, false);
        }

      }

      //Copy files from Folder_B
      foreach (FileInfo b_file in files_B)
      {
        FileInfo a_file = Array.Find(files_A, x => x.Name == b_file.Name);
        if (a_file == null)
          b_file.CopyTo(Folder_a + "\\" + b_file.Name, false);
      }

      DirectoryInfo[] folders_A = dirA.GetDirectories();
      DirectoryInfo[] folders_B = dirB.GetDirectories();

      //Copy & replace Folders in dirA
      foreach (DirectoryInfo a_folder in folders_A)
      {
        if (a_folder.Name == Program.oldFolderName)
          continue;

        int i_Bfolder = Array.FindIndex(folders_B, x => x.Name == a_folder.Name);

        if (i_Bfolder != -1)
          FusionData(a_folder.FullName, folders_B[i_Bfolder].FullName, oldPath_A, oldPath_B, OldFolderName);
        else
          FolderTool.DirectoryCopy(a_folder.FullName, Folder_b + "\\" + a_folder.Name, true);
      }
      foreach (DirectoryInfo b_folder in folders_B)
      {
        if (b_folder.Name == Program.oldFolderName)
          continue;

        int i_Afolder = Array.FindIndex(folders_A, x => x.Name == b_folder.Name);
        if (i_Afolder == -1)
          FolderTool.DirectoryCopy(b_folder.FullName, Folder_a + "\\" + b_folder.Name, true);
      }

      static void ReplaceFile(FileInfo a_file, string oldPath, string CopyPath)
      {
        string moveFullName, moveFullNameTemp;
        DirectoryInfo oldDir_A = Directory.CreateDirectory(oldPath);
        int dot = a_file.Name.LastIndexOf(".");

        if (dot != -1)
          moveFullName = "\\" + a_file.Name.Insert(dot, Program.oldName);
        else
          moveFullName = "\\" + a_file.Name + Program.oldName;

        while (File.Exists(oldDir_A.FullName + moveFullName))
        {
          int i_result = moveFullName.Length - 1;
          if (dot != -1)
            i_result = GetNumFromEndString(moveFullName.Substring(dot, i_result-dot));
          else
            i_result = GetNumFromEndString(moveFullName.Substring(i_result - 4, i_result));

          if (i_result > 0)
          {
            int next_num = i_result + 1;
            moveFullName = moveFullName.Replace(Program.oldName + i_result, Program.oldName + next_num);
          }
          else
            moveFullName = moveFullName.Replace(Program.oldName, Program.oldName + 1);
        }
        moveFullNameTemp = moveFullName;

        File.Move(a_file.FullName, oldDir_A.FullName + moveFullName);

        if (CopyPath != null)
        {
          DirectoryInfo oldDir_B = Directory.CreateDirectory(CopyPath);
          while (File.Exists(oldDir_B.FullName + moveFullName))
          {
            int i_result = moveFullName.Length - 1;
            if (dot != -1)
              i_result = GetNumFromEndString(moveFullName.Substring(dot, i_result - dot));
            else
              i_result = GetNumFromEndString(moveFullName.Substring(i_result - 4, i_result));

            if (i_result > 0)
            {
              int next_num = i_result + 1;
              moveFullName = moveFullName.Replace(Program.oldName + i_result, Program.oldName + next_num);
            }
            else
              moveFullName = moveFullName.Replace(Program.oldName, Program.oldName + 1);
          }
          File.Copy(oldDir_A.FullName + moveFullNameTemp, oldDir_B.FullName + moveFullName);
        }
      }
    }







    public static string GetFolderPath(string extraInfo = null)
    {
      int select;
      string path = null;

      #region HelloMessage
      if (extraInfo != null)
        Console.WriteLine(extraInfo);
      Console.WriteLine(Program.Title("FolderTool"));
      Console.WriteLine("Select:" +
        "\n1) Open folder whit tool" +
        "\n2) Write folder path manually");
      #endregion

      select = Read_IntSelect(1, 2);

      if (select == 1)
      {
        FolderTool.a_Main(out path);
      }
      if (select == 2)
      {
        Console.Write("\ninsert folder path: ");
        path = Console.ReadLine().Trim();
      }

      if (Directory.Exists(path))
        return path;
      else
      {
        DataTools.SpecalCommands(path);
        Console.WriteLine("Way not found");
        return GetFolderPath();
      }
    }

    public static void SpecalCommands(string input)
    {
      SubCommands.MethodUnit command = Array.Find(SubCommands.AllCommands, x => x.name == input.ToLower());

      if (command != null)
        command.method();
      else
        Console.WriteLine("Unknown command, use \"/help\"");
    }

    public static int Read_IntSelect(int min, int max, string subMessage = null)
    {
      string consoleInput = Console.ReadLine();

      if (Int32.TryParse(consoleInput, out int select) && select >= min & select <= max)
      {
        return select;
      }
      else
      {
        DataTools.SpecalCommands(consoleInput);
        if (subMessage == null)
          Console.WriteLine($"number not included in [{min},{max}], input again");
        else
          Console.WriteLine(subMessage);
        return Read_IntSelect(min, max, subMessage);
      }

    }
    public static bool Read_yn()
    {
      ConsoleKey response;
      do
      {
        Console.Write("[y/n] ");
        response = Console.ReadKey(false).Key;   // true is intercept key (dont show), false is show
        if (response != ConsoleKey.Enter)
          Console.WriteLine();
      } while (response != ConsoleKey.Y && response != ConsoleKey.N);

      if (response == ConsoleKey.Y)
        return true;
      else
        return false;
    }

    public static int GetNumFromEndString(string s_num)
    {
      Stack<char> numer = new Stack<char>();
      string result = "";
      bool isNum = false;
      for (int i = s_num.Length - 1; i >= 0; i--)
      {
        if (char.IsDigit(s_num[i]))
        {
          isNum = true;
          numer.Push(s_num[i]);
        }
        else if (isNum)
          break;
      }

      while (numer.Count > 0)
        result += numer.Pop();

      Int32.TryParse(result,out int res);
      return res;
    }
  }
}

