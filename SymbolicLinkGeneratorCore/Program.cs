using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using SymbolicLinkGenerator.Shared;

namespace SymbolicLinkGeneratorCore
{
    internal class Program
    {
        static void Main(string[] args)
        {
            StartListening();
        }

        private static void StartListening()
        {

            //var pipeServer = new NamedPipeServerStream("SlgFilePipe", PipeDirection.In);
            var pipeSecurity = new PipeSecurity();
            pipeSecurity.AddAccessRule(new PipeAccessRule("Everyone", PipeAccessRights.ReadWrite, AccessControlType.Allow));
            while (true)
            {
                try
                {
                    using (var pipeServer = new NamedPipeServerStream("Global\\SlgFilePipe", PipeDirection.InOut,
                                                                      1, PipeTransmissionMode.Byte, PipeOptions.None, 
                                                                      1024, 1024, pipeSecurity))
                    {
                        Console.WriteLine("DBUG: 等待普通用户应用程序连接...");
                        pipeServer.WaitForConnection();
                        Console.WriteLine("DBUG: 普通用户应用程序已连接。");
                        //HandleClient(pipeServer);
                        HandleClientEx(pipeServer);
                    }
                    Console.WriteLine($"DBUG: 服务端关闭了管道");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"WARN: {ex.Message}");
                }
            }
        }

        private static void HandleClientEx(NamedPipeServerStream pipeServer)
        {
            const int JSON_ARRAY_END_FLAG = 93; // 93 = ']'
            using (var memoryStream = new MemoryStream())
            {
                byte[] buffer = new byte[1024];
                int bytesRead;
                while ((bytesRead = pipeServer.Read(buffer, 0, buffer.Length)) > 0)
                {
                    memoryStream.Write(buffer, 0, bytesRead);// 从缓冲区读取全部数据
                    // 判断发送结束
                    if (buffer[bytesRead - 1] == JSON_ARRAY_END_FLAG)
                        break;
                }

                string receivedMessage = Encoding.UTF8.GetString(memoryStream.ToArray());
                Console.WriteLine($"DBUG: 接收到的消息: {receivedMessage}");
            
                // 批量处理创建软连接
                try
                {
                    var helper = new DataHelper(receivedMessage);
                    int i = 0;
                    foreach (dtoSLGItem item in helper.Items)
                        i += TryMakeLink(item.Link, item.SourcePath) ? 1 : 0;
                    Console.WriteLine($"DBUG: 数据处理完毕, 共 {helper.Items.Length} 项，成功 {i} 项。");
                    
                    // 通过管道应答结果，返回创建的软连接数
                    buffer = BitConverter.GetBytes(i);
                    pipeServer.Write(buffer, 0, buffer.Length);
                    pipeServer.Flush(); // 确保数据被发送
                    Console.WriteLine($"DBUG: 向客户端应答 {i}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"WARN: {ex.Message}");
                }
            }
        }

        private static void HandleClient(NamedPipeServerStream server)
        {
            using (var reader = new StreamReader(server))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Console.WriteLine($"DBUG: 接收到的文件路径={line}");
                    // 在这里处理接收到的文件路径
                    var paths = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (paths.Length >= 1)
                        Task.Run(() =>
                        {
                            TryMakeLink(paths[0], paths[1]);
                            Console.WriteLine("DBUG: 数据处理完毕。");
                        });
                    else
                        Console.WriteLine($"DBUG: 接收到的参数错误");
                }
            }
        }

        public static bool TryMakeLink(string link, string target)
        {
            var startInfo = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                Arguments = $"/c mklink {(Directory.Exists(target) ? "/d " : "")}\"{link}\" \"{target}\"",
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };

            bool ret = false;
            try
            {
                var process = new Process();
                process.StartInfo = startInfo;
                process.Start();
                string stdOut = process.StandardOutput.ReadToEnd();
                string stdErr = process.StandardError.ReadToEnd();
                process.WaitForExit();

                ret = process.ExitCode == 0;
                process.Dispose();
                if (ret)
                    Console.WriteLine($"INFO: {stdOut}");
                else
                    Console.WriteLine($"ERRO: {stdErr}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WARN: {ex.Message}");
            }

            return ret;
        }
    }
}
