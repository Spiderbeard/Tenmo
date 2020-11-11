using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using TenmoClient.Data;

namespace TenmoClient

{
    class Program : AsciiHelper
    {

        private static readonly ConsoleService consoleService = new ConsoleService();
        private static readonly AuthService authService = new AuthService();
        private static readonly TransferService transferService = new TransferService();
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Run();
        }
        private static void Run()
        {
            int loginRegister = -1;
            while (loginRegister != 1 && loginRegister != 2)
            {
                PrintIntroLogo();
                Console.WriteLine("Welcome to TEnmo!");
                Console.WriteLine("1: Login");
                Console.WriteLine("2: Register");
                Console.Write("Please choose an option: ");
                if (!int.TryParse(Console.ReadLine(), out loginRegister))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (loginRegister == 1)
                {
                    while (!UserService.IsLoggedIn()) //will keep looping until user is logged in
                    {
                        LoginUser loginUser = consoleService.PromptForLogin();
                        API_User user = authService.Login(loginUser);




                        if (user != null)
                        {
                            UserService.SetLogin(user);
                        }

                    }
                }
                else if (loginRegister == 2)
                {
                    bool isRegistered = false;
                    while (!isRegistered) //will keep looping until user is registered
                    {
                        LoginUser registerUser = consoleService.PromptForLogin();
                        isRegistered = authService.Register(registerUser);
                        if (isRegistered)
                        {
                            Console.WriteLine("");
                            Console.WriteLine("Registration successful. You can now log in.");
                            loginRegister = -1; //reset outer loop to allow choice for login
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid selection.");
                }
            }
            Console.Clear();
            MenuSelection();
        }
        private static void MenuSelection()
        {
            int menuSelection = -1;
            while (menuSelection != 0)
            {
                //Console.Clear();
                PrintLogo();
                Console.WriteLine("");
                Console.WriteLine("Welcome to TEnmo! Please make a selection: ");
                Console.WriteLine("1: View your current balance");
                Console.WriteLine("2: View your past transfers");
                Console.WriteLine("3: View your pending requests");
                Console.WriteLine("4: Send TE bucks");
                Console.WriteLine("5: Request TE bucks");
                Console.WriteLine("6: Log in as different user");
                Console.WriteLine("0: Exit");
                Console.WriteLine("---------");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out menuSelection))
                {
                    Console.Clear();
                    Console.WriteLine("Invalid input. Please enter only a number.");
                    SomethingWentWrong();
                    Console.Clear();
                    MenuSelection();
                }
                else if (menuSelection == 1)
                {
                    PrintBalance();

                }
                else if (menuSelection == 2)
                {
                    ViewAllTransfers();

                }
                else if (menuSelection == 3)
                {
                    PendingTransfersMenu();
                }
                else if (menuSelection == 4)
                {
                    SendMoney();
                    // send money
                }
                else if (menuSelection == 5)
                {
                    RequestMoney();
                }
                else if (menuSelection == 6)
                {
                    Console.WriteLine("");
                    UserService.SetLogin(new API_User()); //wipe out previous login info
                    Console.Clear();
                    LoggingOut();
                    System.Threading.Thread.Sleep(2000);
                    Console.Clear();
                    Run(); //return to entry point
                }
                else
                {
                    Console.Clear();
                    LoggingOut();
                    System.Threading.Thread.Sleep(2000);
                    Environment.Exit(0);
                }
            }
        }
        private static void PrintTransfers(List<Transfer> allTransfers)
        {
            Console.Clear();
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine("Transfer");
            Console.WriteLine("IDs               From/To               Amount");
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine();

            if (allTransfers.Count == 0)
            {
                Console.WriteLine("There are no transfers associated with this User");
                Console.WriteLine();
            }
            else
            {
                foreach (Transfer trans in allTransfers)
                {
                    if (trans.account_To_ID == UserService.GetUserId())
                    {
                        string id = $"Id: {trans.transfer_ID}";
                        string name = $"From: {trans.account_From_UserName}";
                        string amount = $" {trans.AmountToTransfer:C2}";


                        Console.WriteLine(String.Format("{0,-9} |   {1,-22} | {2,6} ", id, name, amount));
                    }
                    else
                    {
                        string id = $"Id: {trans.transfer_ID}";
                        string name = $"To: {trans.account_To_UserName}";
                        string amount = $" {trans.AmountToTransfer:C2}";

                        Console.WriteLine(String.Format("{0,-9} |   {1,-22} | {2,6}", id, name, amount));
                    }
                }
            }
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine();
            Console.Write("Please enter the Id of which you would like to view the details (0 to exit): ");
        }
        private static void PrintUsers(List<Account> allUsers)
        {
            Console.Clear();
            Console.WriteLine("--------------------------------------------------------------");
            foreach (Account trans in allUsers)
            {
                Console.WriteLine($"Id : {trans.User_ID}   Username : {trans.UserName} ");
            }
            Console.WriteLine("--------------------------------------------------------------");
        }

        private static void PrintBalance()
        {
            Console.Clear();
            Account account = transferService.GetBalance(UserService.GetUserId());
            Console.WriteLine("---------------------------------------------------");
            string message = $"Your balance is {account.Balance:C2}";
            Console.WriteLine(message);
            Console.WriteLine("---------------------------------------------------");
            System.Threading.Thread.Sleep(3000);
            Console.Clear();
        }
        private static void PrintTransferDetails(List<Transfer> allTransfers, int transferToView)
        {
            Console.WriteLine();
            Console.WriteLine("--------------------------------------------------------------");
            Console.WriteLine("Transfer details");
            Console.WriteLine("--------------------------------------------------------------");
            bool found = false;
            foreach (Transfer trans in allTransfers)
            {
                
                if (trans.transfer_ID == transferToView )
                {
                    Console.WriteLine($"Id : {trans.transfer_ID}");
                    if (trans.account_From_UserName == null)
                    {
                        Console.WriteLine($"From : {UserService.GetName()} (Me)");
                        Console.WriteLine($"To : {trans.account_To_UserName}");
                    }
                    else if (trans.account_To_UserName == null)
                    {
                        Console.WriteLine($"From : {trans.account_From_UserName}");
                        Console.WriteLine($"To : {UserService.GetName()} (Me)");
                    }
                    else { Console.WriteLine("The clouds... they're beige"); }
                    if (trans.type_ID == 1)
                    {
                        Console.WriteLine($"Type : Request");
                    }
                    else { Console.WriteLine("Type : Send"); }

                    if (trans.status_ID == 1)
                    {
                        Console.WriteLine("Status : Pending");
                    }
                    else if (trans.status_ID == 2)
                    {
                        Console.WriteLine($"Status : Approved");
                    }
                    else
                    {
                        Console.WriteLine("Status: Rejected");
                    }
                    Console.WriteLine($"Amount : {trans.AmountToTransfer:C2}");
                    Console.WriteLine("--------------------------------------------------------------");
                    found = true;
                    break;
                }
                                
            }
            if ( !found)
            {
                Console.WriteLine("Sorry you do not have access to this transfer.");
            }
            
        }
        private static void ViewAllTransfers()
        {
            List<Transfer> allTransfers = transferService.GetAllTransfers(UserService.GetUserId());
            allTransfers = allTransfers.OrderBy(m => m.transfer_ID).ToList();
            PrintTransfers(allTransfers);
            int transferToView = -1;
            if (!int.TryParse(Console.ReadLine(), out transferToView))
            {
                Console.Clear();
                Console.WriteLine("Invalid Input. Please enter only a number.");
                Console.WriteLine();
                SomethingWentWrong();


            }
            else if (transferToView > 0)
            {
                do
                {
                    Console.Clear();
                    PrintTransferDetails(allTransfers, transferToView);
                    Console.WriteLine();
                    Console.Write("Would you like to view another transfer? (Y/N) : ");
                    string Continue = Console.ReadLine().ToLower();
                    if (Continue == "n" || Continue == "no")
                    {
                        transferToView = 0;
                        Console.Clear();
                    }
                    else
                    {
                        PrintTransfers(allTransfers);
                        transferToView = int.Parse(Console.ReadLine());
                        Console.Clear();
                    }
                } while (transferToView > 0);
            }
            else
            {
                Console.Clear();
                MenuSelection();
            }
        }
        private static void SendMoney()
        {
            int finish = -1;

            do
            {
                List<Account> allUsers = transferService.GetAllUsers(UserService.GetUserId());
                PrintUsers(allUsers);
                Console.Write("Please enter ID of user you wish to transfer money to: (0 to exit) ");

                int toAccount = -1;
                if (!int.TryParse(Console.ReadLine(), out toAccount))
                {
                    Console.Clear();
                    Console.WriteLine();
                    Console.WriteLine("Invalid entry.  Please enter a number. ");
                    SomethingWentWrong();
                    Console.Clear();

                }else if ( toAccount == 0)
                {
                    Console.Clear();
                    break;
                }

                if (toAccount > 0)
                {
                    if (!IsValidUserId(allUsers, toAccount))
                    {
                        Console.Clear();
                        Console.WriteLine();
                        Console.WriteLine("Invalid Id. Please enter correct ID number. ");
                        SomethingWentWrong();


                    }
                    Console.Write("Please enter amount to transfer: $");
                    decimal amount = -1;
                    if (!decimal.TryParse(Console.ReadLine(), out amount))
                    {
                        Console.Clear();
                        Console.WriteLine();
                        Console.WriteLine("Invalid entry. Please input numbers only.");
                        SomethingWentWrong();
                        toAccount = -1;

                    }
                    Transfer newTransfer = new Transfer();
                    newTransfer.account_From_ID = UserService.GetUserId();
                    newTransfer.account_To_ID = toAccount;
                    newTransfer.AmountToTransfer = amount;

                    if (amount > 0)
                    {
                        if (transferService.SendMoney(newTransfer))
                        {
                            Console.WriteLine();
                            Console.WriteLine("--------------------------------------------------------------");
                            Console.WriteLine("Transfer Complete.  ");
                            Console.WriteLine("--------------------------------------------------------------");
                            System.Threading.Thread.Sleep(3000);
                            Console.Clear();
                            toAccount = -1;
                            finish++;
                        }
                        else
                        {
                            Console.WriteLine("Insufficient Funds.");
                            System.Threading.Thread.Sleep(2000);
                            toAccount = -1;

                        }
                    }


                }  
            } while (finish != 0);
        }
        private static bool IsValidUserId(List<Account> allUsers, int userId)
        {
            bool isValid = false;
            foreach (Account acc in allUsers)
            {
                if (acc.User_ID == userId)
                {
                    isValid = true;
                }
            }
            return isValid;
        }
        private static void PendingTransfersMenu()
        {
            Console.Clear();
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine("Transfer");
            Console.WriteLine("IDs               From                  Amount");
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine();

            List<Transfer> allTransfers = transferService.GetAllTransfers(UserService.GetUserId());


            List<Transfer> myPendingTransfers = PrintPendingTransfers(allTransfers);

            if (myPendingTransfers.Count == 0)
            {
                Console.WriteLine("There are no pending transfers for this user.");
                Console.WriteLine("----------------------------------------------");
                System.Threading.Thread.Sleep(5000);
                Console.WriteLine();
                Console.Clear();
            }
            else
            {
                Console.WriteLine("----------------------------------------------");
                Console.WriteLine();
                Console.Write("Please enter the Id of the transfer you would like to view (0 to exit) : ");
                int transferToView = -1;
                if (!int.TryParse(Console.ReadLine(), out transferToView))
                {
                    Console.Clear();
                    Console.WriteLine();
                    SomethingWentWrong();
                }
                else if (transferToView > 0)
                {
                    PrintTransferDetails(myPendingTransfers, transferToView);

                    Console.Write("Would you like to Approve or Reject this transfer? (A/R or press enter to exit) :");
                    string proceed = Console.ReadLine().ToLower();

                    if (proceed == "a" || proceed == "approve")
                    {
                        bool success = transferService.ApproveTransfer(UserService.GetUserId(), transferToView);

                        if ( success)
                        {
                            Console.WriteLine();
                            Console.WriteLine("--------------------------------------------------------------");
                            Console.WriteLine("Transfer Approved.  ");
                            Console.WriteLine("--------------------------------------------------------------");
                            System.Threading.Thread.Sleep(3000);
                            Console.Clear();
                        }else
                        {
                            Console.WriteLine();
                            Console.WriteLine("--------------------------------------------------------------");
                            Console.WriteLine("Error .  ");
                            Console.WriteLine("--------------------------------------------------------------");
                            SomethingWentWrong();
                            System.Threading.Thread.Sleep(3000);
                            Console.Clear();
                        }
                    }
                    else if (proceed == "r" || proceed == "reject")
                    {
                        transferService.RejectTransfer(UserService.GetUserId(), transferToView);
                    }
                    else
                    {
                        Console.Clear();
                    }
                }else { Console.Clear(); }
            }
        }
        private static List<Transfer> PrintPendingTransfers(List<Transfer> allTransfers)
        {
            allTransfers = allTransfers.OrderBy(m => m.transfer_ID).ToList();
            List<Transfer> bob = new List<Transfer>();
            foreach (Transfer trans in allTransfers)
            {
                if (trans.status_ID == 1)
                {
                    string id = $"ID: {trans.transfer_ID}";
                    string name = $"From: {trans.account_From_UserName}";
                    string amount = $"{trans.AmountToTransfer:C2}";
                    Console.WriteLine(String.Format("{0,-9} |   {1,-22} | {2,6}", id, name, amount));
                    bob.Add(trans);
                }
            }
            return bob;
        }
        private static void RequestMoney()
        {
            int finish = -1;

            do
            {
                List<Account> allUsers = transferService.GetAllUsers(UserService.GetUserId());
                PrintUsers(allUsers);
                Console.Write("Please enter ID of user you wish to request money from: ");
                int fromAccount = -1;
                if (!int.TryParse(Console.ReadLine(), out fromAccount))
                {
                    Console.Clear();
                    Console.WriteLine();
                    Console.WriteLine("Invalid entry.  Please enter a number. ");
                    SomethingWentWrong();
                    Console.Clear();

                }
                if (fromAccount > 0)
                {
                    if (!IsValidUserId(allUsers, fromAccount))
                    {
                        Console.Clear();
                        Console.WriteLine();
                        Console.WriteLine("Invalid Id. Please enter correct ID number. ");
                        SomethingWentWrong();


                    }
                    Console.Write("Please enter amount to request: $");
                    decimal amount = -1;
                    if (!decimal.TryParse(Console.ReadLine(), out amount))
                    {
                        Console.Clear();
                        Console.WriteLine();
                        Console.WriteLine("Invalid entry. Please input numbers only.");
                        SomethingWentWrong();
                        fromAccount = -1;

                    }
                    Transfer newTransfer = new Transfer();
                    newTransfer.account_From_ID = fromAccount;
                    newTransfer.account_To_ID = UserService.GetUserId();
                    newTransfer.AmountToTransfer = amount;

                    if (amount > 0)
                    {
                        if (transferService.RequestMoney(newTransfer))
                        {
                            Console.WriteLine();
                            Console.WriteLine("--------------------------------------------------------------");
                            Console.WriteLine("Transfer Posted.  ");
                            Console.WriteLine("--------------------------------------------------------------");
                            System.Threading.Thread.Sleep(3000);
                            Console.Clear();
                            fromAccount = -1;
                            finish++;
                        }
                        else
                        {
                            Console.WriteLine("Insufficient Funds.");
                            System.Threading.Thread.Sleep(2000);
                            fromAccount = -1;

                        }
                    }


                }
            } while (finish != 0);
        }
    }
}
