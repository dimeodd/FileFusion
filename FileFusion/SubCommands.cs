using System;
using System.Collections.Generic;
using System.Text;

namespace FileFusion
{
  static class SubCommands
  {
    public delegate void Method();

    public class MethodUnit
    {
      public string name;
      public Method method;
      public MethodUnit(string name, Method method)
      {
        this.name = name;
        this.method = method;
      }
    }

    public readonly static MethodUnit[] AllCommands = new MethodUnit[] {
       new MethodUnit("/exit", ExitApp),
       new MethodUnit("/help", HelpCommand),
       new MethodUnit("/?", HelpCommand),
    };

    static void ExitApp()
    {
      Environment.Exit(0);
    }
    static void HelpCommand()
    {
      string output = Program.Title("Command List");
      foreach (var item in AllCommands)
      {
        output += $"\n{item.name}";
      }
      Console.WriteLine(output + "\n" + Program.theLine);
    }
  }
}
