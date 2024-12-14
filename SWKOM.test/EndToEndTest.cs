using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SWKOM.test
{
    [TestFixture]
    public class EndToEndTest
    {
        [SetUp]
        public void SetUp() 
        {
            Process _dockerProcess;
            _dockerProcess = new Process();
            _dockerProcess.StartInfo.FileName = "docker-compose";
            _dockerProcess.StartInfo.Arguments = "-f TestFiles/docker-compose.yml up -d";
            _dockerProcess.StartInfo.RedirectStandardError = true; // to read the output
            _dockerProcess.StartInfo.UseShellExecute = false; // required for redirection above
            _dockerProcess.StartInfo.CreateNoWindow = true; // TODO: findout

            // Start the process
            _dockerProcess.Start();
            _dockerProcess.WaitForExit();
            if (_dockerProcess.ExitCode != 0)
            {
                throw new Exception($"Docker compose error: {_dockerProcess.StandardError.ReadToEnd()}");
            }

        }

        [TearDown]
        public void TearDown()
        {
            Process dockerDown = new Process();
            dockerDown.StartInfo.FileName = "docker-compose";
            dockerDown.StartInfo.Arguments = "-f TestFiles/docker-compose.yml down";
            dockerDown.StartInfo.RedirectStandardOutput = true; // to read the output
            dockerDown.StartInfo.UseShellExecute = false; // required for redirection above
            dockerDown.StartInfo.CreateNoWindow = true; // TODO: findout

            // Start the process
            dockerDown.Start();
            if (dockerDown.ExitCode != 0)
            {
                throw new Exception($"Docker compose error: {dockerDown.StandardError.ReadToEnd()}");
            }
            dockerDown.WaitForExit();
        }

        [Test]
        public void UploadFileThroughWebApi_FindItWithSearch_Correct()
        {
            Console.WriteLine("DummyTesting");
        }
    }
}
