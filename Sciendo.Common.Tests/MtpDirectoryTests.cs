using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Sciendo.Common.IO.MTP;

namespace Sciendo.Common.Tests
{
    [TestFixture(Ignore=false,IgnoreReason = "Integration Tests")]
    public class MtpDirectoryTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExistsWithNoPath()
        {
            MtpDirectory mtpDirectory= new MtpDirectory();
            mtpDirectory.Exists("");
        }

        [Test]
        public void ExistsReturnTrue()
        {
            MtpDirectory mtpDirectory= new MtpDirectory();
            Assert.True(mtpDirectory.Exists(@"Xperia XA\SD Card\Music"));
        }

        [Test]
        public void ExistsReturnFalseNoDevice()
        {
            MtpDirectory mtpDirectory = new MtpDirectory();
            Assert.False(mtpDirectory.Exists(@"Xperia XA1\SD Card\Music"));
        }
        [Test]
        public void ExistsReturnFalseNoBranchFolder()
        {
            MtpDirectory mtpDirectory = new MtpDirectory();
            Assert.False(mtpDirectory.Exists(@"Xperia XA\SD Card1\Music"));
        }
        [Test]
        public void ExistsReturnFalseNoLeafFolder()
        {
            MtpDirectory mtpDirectory = new MtpDirectory();
            Assert.False(mtpDirectory.Exists(@"Xperia XA\SD Card\Music1"));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateFolderNoPath()
        {
            MtpDirectory mtpDirectory = new MtpDirectory();
            mtpDirectory.CreateDirectory(null);
        }

        [Test]
        public void CreateFolderOk()
        {
            MtpDirectory mtpDirectory = new MtpDirectory();
            mtpDirectory.CreateDirectory(@"Xperia XA\SD Card\Music\abc1\abc2\abc3");
            Assert.True(mtpDirectory.Exists(@"Xperia XA\SD Card\Music\abc1\abc2\abc3"));
        }

        [Test]
        public void CreateDoubleFolder()
        {
            MtpDirectory mtpDirectory = new MtpDirectory();
            mtpDirectory.CreateDirectory(@"Xperia XA\SD Card\Music\abc2");
            Assert.True(mtpDirectory.Exists(@"Xperia XA\SD Card\Music\abc2"));
            mtpDirectory.CreateDirectory(@"Xperia XA\SD Card\Music\abc2");
            Assert.True(mtpDirectory.Exists(@"Xperia XA\SD Card\Music\abc2"));

        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DeleteNoPath()
        {
            MtpDirectory mtpDirectory = new MtpDirectory();
            mtpDirectory.Delete(string.Empty,true);
        }

        [Test]
        public void DeleteFolderOk()
        {
            MtpDirectory mtpDirectory = new MtpDirectory();
            mtpDirectory.CreateDirectory(@"Xperia XA\SD Card\Music\abc1\abc2\abc3");
            Assert.True(mtpDirectory.Exists(@"Xperia XA\SD Card\Music\abc1\abc2\abc3"));
            mtpDirectory.Delete(@"Xperia XA\SD Card\Music\abc1\abc2\abc3",false);
            Assert.False(mtpDirectory.Exists(@"Xperia XA\SD Card\Music\abc1\abc2\abc3"));
            Assert.True(mtpDirectory.Exists(@"Xperia XA\SD Card\Music\abc1\abc2"));
        }

        [Test]
        public void DeleteFolderAndContentsOk()
        {
            MtpDirectory mtpDirectory = new MtpDirectory();
            mtpDirectory.CreateDirectory(@"Xperia XA\SD Card\Music\abc1\abc2\abc3");
            mtpDirectory.Delete(@"Xperia XA\SD Card\Music\abc1\abc2", true);
            Assert.False(mtpDirectory.Exists(@"Xperia XA\SD Card\Music\abc1\abc2\abc3"));
            Assert.False(mtpDirectory.Exists(@"Xperia XA\SD Card\Music\abc1\abc2"));
            Assert.True(mtpDirectory.Exists(@"Xperia XA\SD Card\Music\abc1"));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetTopLevelNoPath()
        {
            MtpDirectory mtpDirectory = new MtpDirectory();
            mtpDirectory.GetTopLevel(string.Empty);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetTopLevelNoDevice()
        {
            MtpDirectory mtpDirectory = new MtpDirectory();
            mtpDirectory.GetTopLevel(@"Xperia XA1\SD Card\Music");
        }
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetTopLevelNoFolder()
        {
            MtpDirectory mtpDirectory = new MtpDirectory();
            mtpDirectory.GetTopLevel(@"Xperia XA\SD Card\Music111");
        }
        [Test]
        public void GetTopLevelOk()
        {
            MtpDirectory mtpDirectory = new MtpDirectory();
            var result = mtpDirectory.GetTopLevel(@"Xperia XA\SD Card\Music");
            Assert.AreEqual(5,result.Count());
            Assert.True(result.Any(f=>f=="abc"));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetFilesNoPath()
        {
            MtpDirectory mtpDirectory = new MtpDirectory();
            mtpDirectory.GetFiles(string.Empty,SearchOption.TopDirectoryOnly);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetFilesNoDevice()
        {
            MtpDirectory mtpDirectory = new MtpDirectory();
            mtpDirectory.GetFiles(@"Xperia XA1\SD Card\Music",SearchOption.TopDirectoryOnly);
        }
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetFilesNoFolder()
        {
            MtpDirectory mtpDirectory = new MtpDirectory();
            mtpDirectory.GetFiles(@"Xperia XA\SD Card\Music111", SearchOption.TopDirectoryOnly);
        }
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetFilesFilePassed()
        {
            MtpDirectory mtpDirectory = new MtpDirectory();
            mtpDirectory.GetFiles(@"Xperia XA\SD Card\Music\abc\f1.txt", SearchOption.TopDirectoryOnly);
        }
        [Test]
        public void GetFilesAllTopDirectoryOnlyOk()
        {
            MtpDirectory mtpDirectory = new MtpDirectory();
            var result = mtpDirectory.GetFiles(@"Xperia XA\SD Card\Music\abc",SearchOption.TopDirectoryOnly);
            Assert.AreEqual(1, result.Count());
            Assert.True(result.Any(f => f == @"Xperia XA\SD Card\Music\abc\f1.txt"));
        }
        [Test]
        public void GetFilesAllAllDirectoriesOk()
        {
            MtpDirectory mtpDirectory = new MtpDirectory();
            var result = mtpDirectory.GetFiles(@"Xperia XA\SD Card\Music\abc", SearchOption.AllDirectories);
            Assert.AreEqual(9, result.Count());
            Assert.True(result.Any(f => f == @"Xperia XA\SD Card\Music\abc\f1.txt"));
        }
        [Test]
        public void GetFilesFilteredTopDirectoryOnlyOk()
        {
            MtpDirectory mtpDirectory = new MtpDirectory();
            var result = mtpDirectory.GetFiles(@"Xperia XA\SD Card\Music\abc\abc1", SearchOption.TopDirectoryOnly,new [] {".t1",".t2"});
            Assert.AreEqual(1, result.Count());
            Assert.False(result.Any(f => f == @"Xperia XA\SD Card\Music\abc\abc1\f2.txt"));
            Assert.True(result.Any(f => f == @"Xperia XA\SD Card\Music\abc\abc1\f2.t2"));
        }
        [Test]
        public void GetFilesFilteredAllDirectoriesOk()
        {
            MtpDirectory mtpDirectory = new MtpDirectory();
            var result = mtpDirectory.GetFiles(@"Xperia XA\SD Card\Music\abc", SearchOption.AllDirectories, new[] { ".t1", ".t2" });
            Assert.AreEqual(3, result.Count());
            Assert.False(result.Any(f => f == @"Xperia XA\SD Card\Music\abc\abc1\f2.txt"));
            Assert.True(result.Any(f => f == @"Xperia XA\SD Card\Music\abc\abc1\f2.t2"));
            Assert.False(result.Any(f => f == @"Xperia XA\SD Card\Music\abc\f1.txt"));
        }
        [Test]
        public void GetFilesFilteredAllDirectoriesNoMatch()
        {
            MtpDirectory mtpDirectory = new MtpDirectory();
            var result = mtpDirectory.GetFiles(@"Xperia XA\SD Card\Music\abc\abc2", SearchOption.AllDirectories, new[] { ".t4", ".t5" });
            Assert.AreEqual(0, result.Count());
        }
    }
}
