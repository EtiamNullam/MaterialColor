using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Injector.Console
{
    [Obsolete]
    public class Program
    {
        public static void Main(string[] args)
        {
            System.Console.Write("Select mode (d - default, r - recover backup, c - custom, p - publish field): ");
            var mode = System.Console.ReadLine();

            switch (mode)
            {
                case "d":
                case "D":
                    DefaultInjection();
                    break;
                case "r":
                case "R":
                    RecoverBackup();
                    break;
                case "c":
                case "C":
                    CustomInjection();
                    break;
                case "p":
                case "P":
                    PublishField();
                    break;
                default:
                    System.Console.WriteLine("Invalid mode.");
                    Main(args);
                    break;
            }
        }

        private static void DefaultInjection()
        {
            var fromModulePath = "MaterialColor.dll";
            var toModulePath = "Assembly-CSharp.dll";

            try
            {
                //injector = new MethodInjectorHelper(fromModulePath, toModulePath);
            }
            catch (Exception e)
            {
                ShowException(e);
                return;
            }

            var fileManager = new FileManager();

            if (!fileManager.MakeBackup(toModulePath))
            {
                System.Console.WriteLine("Backup already exists");
                PressAnyKeyToContinue();
                return;
            }

            //injector.InjectAsFirstInstruction("InjectionEntry", "Enter", "Game", "OnPrefabInit");

            try
            {
                //fileManager.SaveModule(injector.ToModule, toModulePath);
            }
            catch (Exception e)
            {
                ShowException(e);
            }
        }

        private static void RecoverBackup()
        {
            System.Console.Write("Backup path: ");
            var path = System.Console.ReadLine();

            if (string.IsNullOrEmpty(path))
            {
                path = "Assembly-CSharp.dll.backup";
            }

            if (!new FileManager().RestoreBackupForFile(path))
            {
                System.Console.WriteLine("Backup not found.");
                PressAnyKeyToContinue();
            }
        }

        private static void CustomInjection()
        {
            System.Console.Write("From module: ");
            var from = System.Console.ReadLine();

            System.Console.Write("To module: ");
            var to = System.Console.ReadLine();

            //MethodInjectorHelper injector;

            try
            {
                //injector = new MethodInjectorHelper(from, to);
            }
            catch (Exception e)
            {
                ShowException(e);
                return;
            }

            System.Console.Write("From type name: ");
            var fromTypeName = System.Console.ReadLine();

            System.Console.Write("From method name: ");
            var fromMethodName = System.Console.ReadLine();

            System.Console.Write("To type name: ");
            var toTypeName = System.Console.ReadLine();

            System.Console.Write("To method name: ");
            var toMethodName = System.Console.ReadLine();

            //injector.InjectAsFirstInstruction(fromTypeName, fromMethodName, toTypeName, toMethodName);
            //injector.InjectAsFirstInstructionAtEntryPoint(fromTypeName, fromMethodName);

            var fileManager = new FileManager();

            fileManager.MakeBackup(to);

            try
            {
                //fileManager.SaveModule(injector.ToModule, to);
            }
            catch (Exception e)
            {
                ShowException(e);
            }
        }

        private static void PublishField()
        {
            System.Console.Write("Module: ");
            var targetModuleName = System.Console.ReadLine();

            if (string.IsNullOrEmpty(targetModuleName))
            {
                targetModuleName = "Assembly-CSharp.dll";
            }

            //FieldPublishHelper publisher;

            try
            {
                //publisher = new FieldPublishHelper(targetModuleName);
            }
            catch (Exception e)
            {
                ShowException(e);
                return;
            }

            System.Console.Write("Type: ");
            var targetTypeName = System.Console.ReadLine();

            System.Console.Write("Field: ");
            var targetFieldName = System.Console.ReadLine();

            var fileManager = new FileManager();

            if (!fileManager.MakeBackup(targetModuleName))
            {
                System.Console.WriteLine("Can't create backup. Backup already exists?");
                PressAnyKeyToContinue();
                return;
            }

            //publisher.MakeFieldPublic(targetTypeName, targetFieldName);

            //fileManager.SaveModule(publisher.TargetModule, targetModuleName);
        }

        private static void PressAnyKeyToContinue()
        {
            System.Console.WriteLine("Press any key to continue...");
            System.Console.ReadKey(false);
        }

        private static void ShowException(Exception e)
        {
            System.Console.WriteLine(e);
            PressAnyKeyToContinue();
        }
    }
}
