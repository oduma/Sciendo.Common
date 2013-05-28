using System;
using System.IO;
using NUnit.Framework;
using Sciendo.Common.Logging;

namespace Sciendo.Common.Tests
{
    [TestFixture]
    public class LoggingManagerTests
    {
        [SetUp]
        public void SetUp()
        {
            if (File.Exists("SciendoCoreDebug.log"))
            {
                var fs = File.Create("SciendoCoreDebug.log");
                fs.Flush();
                fs.Close();
            }

            if (File.Exists("SciendoCoreSystemError.log"))
            {
                var fs = File.Create("SciendoCoreSystemError.log");
                fs.Flush();
                fs.Close();
            }

            if (File.Exists("SciendoCorePerformance.log"))
            {
                var fs = File.Create("SciendoCorePerformance.log");
                fs.Flush();
                fs.Close();
            }
        }
        [Test]
        public void LogDebug_Ok()
        {
            var expectedText = "I wrote something in here";
            LoggingManager.Debug(expectedText);
            using (TextReader tr = File.OpenText("SciendoCoreDebug.log"))
            {
                var lineLogged = tr.ReadLine();
                Assert.True(lineLogged.Contains(expectedText));
            }
        }

        [Test]
        public void LogException_Ok()
        {
            var expectedText = "my excpetion";
            try
            {
                throw new Exception(expectedText);
            }
            catch (Exception ex)
            {
                LoggingManager.LogSciendoSystemError(ex);
                using (TextReader tr = File.OpenText("SciendoCoreSystemError.log"))
                {
                    var lineLogged = tr.ReadLine();
                    Assert.True(lineLogged.Contains(expectedText));
                }
            }
        }
        [Test]
        public void LogExceptionWithMessage_Ok()
        {
            var expectedText = "my excpetion";
            var expectedExplicitMessage = "my explicit message";
            try
            {
                throw new Exception(expectedText);
            }
            catch (Exception ex)
            {
                LoggingManager.LogSciendoSystemError(expectedExplicitMessage,ex);
                using (TextReader tr = File.OpenText("SciendoCoreSystemError.log"))
                {
                    var lineLogged = tr.ReadLine();
                    Assert.True(lineLogged.Contains(expectedText));
                    Assert.True(lineLogged.Contains(expectedExplicitMessage));
                }
            }
        }
        [Test]
        public void LogPerformnace_Ok()
        {
            using (LoggingManager.LogSciendoPerformance())
            {
                for (int i = 0; i < 1000; i++)
                {
                    ;
                }
            }
            using (TextReader tr = File.OpenText("SciendoCorePerformance.log"))
            {
                var lineLogged = tr.ReadLine();
                Assert.True(lineLogged.Contains("Started"));
                lineLogged = tr.ReadLine();
                Assert.True(lineLogged.Contains("Finished"));
            }

        }
        [Test]
        public void LogPerformanceWithInfo_Ok()
        {
            var expectedText = "my text";
            using (LoggingManager.LogSciendoPerformance(expectedText))
            {
                for (int i = 0; i < 1000; i++)
                {
                    ;
                }
            }
            using (TextReader tr = File.OpenText("SciendoCorePerformance.log"))
            {
                var lineLogged = tr.ReadLine();
                Assert.True(lineLogged.Contains("Started"));
                Assert.True(lineLogged.Contains(expectedText));
                lineLogged = tr.ReadLine();
                Assert.True(lineLogged.Contains("Finished"));
                Assert.True(lineLogged.Contains(expectedText));

            }

        }

    }
}
