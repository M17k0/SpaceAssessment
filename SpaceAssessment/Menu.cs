using SpaceAssessment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceAssessment
{
    internal static class Menu
    {
        private static Dictionary<string, int> spaceportDistances = new()
        {
            { "Kourou, French Guyana", 575 },
            { "Cape Canaveral, USA", 3162 },
            { "Kodiak, USA", 6425 },
            { "Tanegashima, Japan", 3968 },
            { "Mahia, New Zealand", 4334 }
        };

        private static string folderPath = "";
        private static string senderEmail = "";
        private static string senderEmailPassword = "";
        private static string receiverEmail = "";

        public static void MenuLoop(string[] args)
        {
            if (!ParseArguements(args))
            {
                Console.WriteLine("Input arguements were not valid, you will have to add them manualy!");
                Console.WriteLine();
            }

            WheatherAnalysisResult? analysisResult = null;
            string? savedReportFilePath = null;

            int option = -1;
            do
            {
                Console.WriteLine("Please select an option:");
                Console.WriteLine("1. Set .csv files folder path.");
                Console.WriteLine("2. Set sender email.");
                Console.WriteLine("3. Set sender email password.");
                Console.WriteLine("4. Set receiver email.");
                Console.WriteLine("5. Make launch analysis.");
                Console.WriteLine("6. Send email.");
                Console.WriteLine("7. Exit.");

                var line = Console.ReadLine();
                if (!int.TryParse(line, out option)
                    || (option < 0 || option > 7))
                {
                    Console.WriteLine("Invalid option. Please try again.");
                    continue;
                }

                switch (option)
                {
                    case 1:
                        {
                            Console.WriteLine("Please write the .csv folder path:");
                            var path = Console.ReadLine();
                            while (!ParseFolderPath(path))
                            {
                                Console.WriteLine("Path is invalid or doesn't contain all location .csv files!");
                                Console.WriteLine("Enter another path:");
                                path = Console.ReadLine();
                            }
                        }
                        break;
                    case 2:
                        {
                            Console.WriteLine("Please write the sender email:");
                            var email = Console.ReadLine();
                            while (!ParseSenderEmail(email))
                            {
                                Console.WriteLine("Invalid email!");
                                Console.WriteLine("Enter another email:");
                                email = Console.ReadLine();
                            }
                        }
                        break;
                    case 3:
                        {
                            Console.WriteLine("Please write the sender password:");

                            while (true)
                            {
                                string password = "";
                                while (true)
                                {
                                    var key = Console.ReadKey(true);
                                    if (key.Key == ConsoleKey.Enter)
                                        break;
                                    else if (key.Key == ConsoleKey.Backspace)
                                    {
                                        if (password.Length != 0)
                                            password = password.Substring(0, password.Length - 1);
                                    }
                                    else
                                        password += key.KeyChar;
                                }

                                if (!ParseSenderEmailPassword(password))
                                    Console.WriteLine("Please enter a password:");
                                else
                                    break;
                            }
                        }
                        break;
                    case 4:
                        {
                            Console.WriteLine("Please write the receiver email:");
                            var email = Console.ReadLine();
                            while (!ParseReceiverEmail(email))
                            {
                                Console.WriteLine("Invalid email!");
                                Console.WriteLine("Enter another email:");
                                email = Console.ReadLine();
                            }
                        }
                        break;
                    case 5:
                        {
                            if (string.IsNullOrWhiteSpace(folderPath))
                            {
                                Console.WriteLine("Folder path is not specified!");
                                Console.WriteLine();
                                continue;
                            }

                            var spaceports = CsvParser.ReadSpaceportData(folderPath, spaceportDistances);

                            analysisResult = WeatherAnalyser.MakeAnalysis(spaceports);

                            var reportFile = CsvWriter.WriteToCsv(analysisResult);
                            if (reportFile == null)
                            {
                                Console.WriteLine("Couldn't open file for writing.");
                                Console.WriteLine();
                                continue;
                            }

                            savedReportFilePath = reportFile;

                            Console.WriteLine("Report created successfully in:");
                            Console.WriteLine(reportFile);
                            Console.WriteLine();
                        }
                        break;
                    case 6:
                        {
                            if (analysisResult == null
                                || string.IsNullOrWhiteSpace(savedReportFilePath))
                            {
                                Console.WriteLine("You should create an analysis before sending the email!");
                                Console.WriteLine();
                                continue;
                            }

                            if (string.IsNullOrWhiteSpace(senderEmail)
                                || string.IsNullOrEmpty(senderEmailPassword)
                                || string.IsNullOrWhiteSpace(receiverEmail))
                            {
                                Console.WriteLine("You should specify sender email, password and receiver email!");
                                Console.WriteLine();
                                continue;
                            }

                            if (MailSender.SendReportMail(analysisResult, savedReportFilePath, senderEmail, senderEmailPassword, receiverEmail))
                            {
                                Console.WriteLine("Mail sent successfully!");
                                Console.WriteLine();
                            }
                            else
                            {
                                Console.WriteLine("Couldn't send mail!");
                                Console.WriteLine();
                            }
                        }
                        break;
                    default: break;
                }

            } while (option != 7);

        }

        private static bool ParseArguements(string[] args)
        {
            if (args.Length != 4)
                return false;

            if (!ParseFolderPath(args[0]))
                return false;

            if (!ParseSenderEmail(args[1]))
                return false;

            if (!ParseSenderEmailPassword(args[2]))
                return false;

            if (!ParseReceiverEmail(args[3]))
                return false;

            return true;
        }

        private static bool ParseFolderPath(string? path)
        {
            try
            {
                if (!Directory.Exists(path))
                    return false;

                var fileNames = spaceportDistances.Keys.Select(l => l + ".csv").ToList();
                var files = Directory.GetFiles(path).ToHashSet();
                if (!fileNames.All(fileName => files.Any(f => f.EndsWith(fileName, StringComparison.OrdinalIgnoreCase))))
                    return false;

                folderPath = path;

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool ParseSenderEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email)
                || !email.Contains("@"))
                return false;

            senderEmail = email;

            return true;
        }

        private static bool ParseSenderEmailPassword(string? password)
        {
            if (string.IsNullOrEmpty(password)
                || password.Length == 0)
                return false;

            senderEmailPassword = password;

            return true;
        }

        private static bool ParseReceiverEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email)
                || !email.Contains("@"))
                return false;

            receiverEmail = email;

            return true;
        }
    }
}
