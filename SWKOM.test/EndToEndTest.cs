using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using sws.SL.DTOs;

namespace SWKOM.test
{
    [TestFixture]
    public class EndToEndTest
    {
        [SetUp]
        public void SetUp() 
        {
            tearDownDocker();

            // -d to ensure the process continues and the test doesn't hangs
            Process dockerProcess = executeCommand("docker-compose",
                                                   "-f TestFiles/docker-compose.yml up -d");
            if (dockerProcess.ExitCode != 0)
            {
                throw new Exception($"Docker compose error: {dockerProcess.StandardError.ReadToEnd()}");
            
            }

            waitUntilElasticReady();

        }

        [TearDown]
        public void TearDown()
        {
            tearDownDocker();
        }

        [Test]
        public void UploadFileThroughWebApi_FindItWithSearch_Correct()
        {
            // Arrange
            string filename = "miau";
            string testFile = "TestFiles/HelloWorld.pdf";

            // Step 1: Upload test file using the UploadDocument endpoint
            // needed scaped \" because Window's curl is intolerant ): 
            string arguments = "-X POST \"http://localhost:8080/api/UploadDocument\" ";
            arguments += "-H  \"accept: */*\" ";
            arguments += "-H \"Content-Type: multipart/form-data\" ";
            arguments += "-F \"Name=miau\" ";
            arguments += $"-F \"File=@{testFile};type=application/pdf\"";

            Process curlUpload = executeCommand("curl", arguments);
            if (curlUpload.ExitCode != 0)
            {
               throw new Exception($"curlUpload error: {curlUpload.StandardError.ReadToEnd()}");
            }

            // Step 2: Use the query endpoint to check that the file is
            //         present in Elasticsearch and the database.
            string query = "Hello";

            // Retry logic to wait until OCR worker is done
            int maxRetries = 60;
            int currentRetry = 0;
            while(true)
            {
                Process curlCheck = executeCommand("curl", $"-X GET http://localhost:8080/api/Elasticsearch/{query} -H \"accept: text/plain\"");
                if (curlCheck.ExitCode == 0)
                {
                    string output = curlCheck.StandardOutput.ReadToEnd();
                    // Required because the return is "name" instead of "Name", and so on.
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true // Ensures case-insensitive deserialization
                    };
                    List<DocumentSearchDTO> matches;
                    try
                    {
                        matches = JsonSerializer.Deserialize<List<DocumentSearchDTO>>(output, options);
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"Something wrong with output: {output}");
                    }
                    if (matches.Count > 0)
                    {
                        Assert.That(matches[0].Name, Is.EqualTo(filename));
                        break;
                    }
                    else
                    {
                        // Retry
                        Thread.Sleep(1000);
                        ++currentRetry;
                        if (currentRetry >= maxRetries)
                        {
                            Assert.Fail($"After {maxRetries} the expected file {filename} is not found");
                        }
                    }
                }
                else
                {
                    throw new Exception($"curlCheck error: {curlCheck.StandardError.ReadToEnd()}");
                }
            }


        }

        private void tearDownDocker()
        {
            Process dockerDown = executeCommand("docker-compose",
                                                "-f TestFiles/docker-compose.yml down");
            if (dockerDown.ExitCode != 0)
            {
                throw new Exception($"Docker compose error: {dockerDown.StandardError.ReadToEnd()}");
            }
        }

        private Process executeCommand(string command, string arguments)
        {
            Process process = new Process();
            process.StartInfo.FileName = command;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.RedirectStandardError = true; // to read the output
            process.StartInfo.RedirectStandardOutput = true; // to read the output
            process.StartInfo.UseShellExecute = false; // required for redirection above
            process.StartInfo.CreateNoWindow = true; // TODO: findout
            // Start the process
            process.Start();
            process.WaitForExit();
            return process;
        }
        // Helper fn that queries the Elasticsearch container "health" endpoint until it reports to be "green".
        private void waitUntilElasticReady()
        {
            int maxRetries = 60 * 5;  // 5 minutes of retries
            int waitTime = 1000;  // 1000[ms] = 1 sec, between each retry
            int currentRetry = 0;
            while (true)
            {
                currentRetry++;
                if (currentRetry > maxRetries)
                {
                    throw new Exception($"Reached maximum number of retries: {maxRetries}");
                }
                Process curlHealth = executeCommand("curl", "-X GET \"http://localhost:9200/_cluster/health\"");
                if (curlHealth.ExitCode == 0)
                {
                    string output = curlHealth.StandardOutput.ReadToEnd(); // JSON reply
                    string status = JsonDocument.Parse(output).RootElement.GetProperty("status").GetString();
                    if (status == "green")
                    {
                        break; // All good
                    }
                    else
                    {
                        throw new Exception($"Unexpected Elasticsearch status: {status}");          
                    }
                }
                Thread.Sleep(waitTime); // wait 1 sec to retry connection

            }
        }

    }
}
