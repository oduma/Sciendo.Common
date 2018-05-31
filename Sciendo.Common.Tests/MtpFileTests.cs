using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using NUnit.Framework;
using Sciendo.Common.IO.MTP;

namespace Sciendo.Common.Tests
{
    [TestFixture]
    [Ignore("Requires device to be connected in order for it to work.")]
    public class MtpFileTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateFileNoPath()
        {
            MtpFile mtpFile = new MtpFile();
            mtpFile.Create("", null);
        }

        [Test]
        public void CreateFileOk()
        {
            MtpFile mtpFile = new MtpFile();
            mtpFile.Create(@"Xperia XA\SD Card\Music\abc1\abc2\myfile.bin", new byte[3] {2,23,34});
            Assert.True(mtpFile.Exists(@"Xperia XA\SD Card\Music\abc1\abc2\myfile.bin"));

        }
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateFileEmtpy()
        {
            MtpFile mtpFile = new MtpFile();
            mtpFile.Create(@"Xperia XA\SD Card\Music\abc1\abc2\myfile1.bin", null);

        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FileExistsNoFile()
        {
            MtpFile mtpFile = new MtpFile();
            Assert.True(mtpFile.Exists(""));
        }
        [Test]
        public void FileExistsOk()
        {
            MtpFile mtpFile = new MtpFile();
            Assert.True(mtpFile.Exists(@"Xperia XA\SD Card\Music\abc\wpd.sln"));
        }

        [Test]
        public void FileNotExists()
        {
            MtpFile mtpFile = new MtpFile();
            Assert.False(mtpFile.Exists(@"Xperia XA\SD Card\Music\abc\wpd111.sln"));
        }

        [Test]
        public void FileExistsPathNotExists()
        {
            MtpFile mtpFile = new MtpFile();
            Assert.False(mtpFile.Exists(@"Xperia XA\SD Card1\Music\abc\wpd.sln"));

        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DeleteFileNoPath()
        {
            MtpFile mtpFile = new MtpFile();
            mtpFile.Delete(null);

        }
        [Test]
        public void WriteTextFileOk()
        {
            var mtpFile = new MtpFile();
            mtpFile.WriteAllText(@"Xperia XA\SD Card\Music\abc\abc1\wpd.txt","abc is my fav group of letters.");
            Assert.True(mtpFile.Exists(@"Xperia XA\SD Card\Music\abc\abc1\wpd.txt"));
        }
    }
}
