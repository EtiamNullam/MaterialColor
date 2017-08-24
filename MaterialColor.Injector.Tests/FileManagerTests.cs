using NUnit.Framework;
using System;
using System.IO;

namespace MaterialColor.Injector.Tests
{
    [TestFixture]
    public class FileManagerTests
    {
        [SetUp]
        public void CreateTestFile()
        {
            CreateDummyFile(Path.Combine(TestFilePaths.TestFolder, TestFilePaths.TestFile));
        }

        private void CreateDummyFile(string filePath)
        {
            using (var writer = File.CreateText(filePath))
            {
                writer.WriteLine(DateTime.Now);
                writer.Close();
            }
        }

        [TearDown]
        public void ClearTestFolder()
        {
            var folder = new DirectoryInfo(TestFilePaths.TestFolder);

            foreach (var file in folder.GetFiles())
            {
                file.Delete();
            }
        }

        //[Test]
        //public void Backup_Basic()
        //{
        //    var fileManager = new FileManager();
        //    var result = fileManager.MakeBackup(TestFilePaths.TestFileFullPath);

        //    Assert.True(result, "FileManager.MakeBackup() returned false");
        //    FileAssert.DoesNotExist(TestFilePaths.TestFileFullPath, "Original file still exists");
        //    FileAssert.Exists(TestFilePaths.BackupFileFullPath, "Backup file not created");
        //}
    }

    public class TestFilePaths
    {
        public const string TestFolder = "C:\\InjectorTests";
        public const string TestFile = "TestFile";

        public static string TestFileFullPath = Path.Combine(TestFolder, TestFile);
        public static string BackupFileFullPath => TestFileFullPath + ".backup";
    }
}
