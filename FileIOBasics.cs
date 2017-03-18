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

            //delete file
            System.IO.File.Delete(testPath);
            Assert.False(System.IO.File.Exists(testPath), "Deleted temp file no longer exists");
        }


    }
}
