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

    }
}
