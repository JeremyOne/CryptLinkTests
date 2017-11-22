using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptLinkTests {
    
    /// <summary>
    /// Some very basic tests to confirm we can add/modify/delete files
    /// </summary>
    [TestFixture]
    public class FileIOBasics {

        [Test]
        public void FileIOAddModifyDelete() {
            var testPath = CryptLink.Utility.GetTempFilePath("txt");

            //new file
            System.IO.File.WriteAllText(testPath, "test");
            Assert.True(System.IO.File.Exists(testPath), "New temp file exists");

            //modify file
            System.IO.File.AppendAllText(testPath, "text");
            string fileContents = System.IO.File.ReadAllText(testPath);
            Assert.AreEqual(fileContents, "testtext", "Modified file contents were as expected");

            //wait for a bit so the file system can catch up
            System.Threading.Thread.Sleep(1000);

            //delete file
            CryptLink.Utility.WaitDeleteFile(testPath, new TimeSpan(0, 0, 30));
            Assert.False(System.IO.File.Exists(testPath), "Deleted temp file no longer exists");
        }

        [Test, Category("Expensive")]
        public void FileIOWaitDelete() {
            var testPath = CryptLink.Utility.GetTempFilePath("txt");
            var lockLength = new TimeSpan(0, 0, 5);
            var timeoutLength = new TimeSpan(0, 0, 15);
            var minTestEnd = DateTime.Now.Add(lockLength).Add(-lockLength);

            //new file
            System.IO.File.WriteAllText(testPath, "test");

            //lock file open
            var task = Task.Run(() => LockFile(testPath, lockLength));
            System.Threading.Thread.Sleep(500);

            //delete file
            Assert.True(CryptLink.Utility.WaitDeleteFile(testPath, timeoutLength));
            Assert.False(System.IO.File.Exists(testPath), "Deleted temp file no longer exists");
            Assert.GreaterOrEqual(DateTime.Now, minTestEnd);
        }

        public void LockFile(string FilePath, TimeSpan Length) {
            var w = System.IO.File.OpenWrite(FilePath);
            System.Threading.Thread.Sleep((int)Length.TotalMilliseconds);
            w.Close();
            w.Dispose();
        }
    }
}
