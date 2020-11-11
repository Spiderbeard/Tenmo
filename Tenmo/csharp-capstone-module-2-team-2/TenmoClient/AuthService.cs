using RestSharp;
using RestSharp.Authenticators;
using System;
using TenmoClient.Data;

namespace TenmoClient
{
    public class AuthService
    {
        private readonly static string API_BASE_URL = "https://localhost:44315/";
        private readonly IRestClient client = new RestClient();

        //login endpoints
        public bool Register(LoginUser registerUser)
        {
            RestRequest request = new RestRequest(API_BASE_URL + "login/register");
            request.AddJsonBody(registerUser);
            IRestResponse<API_User> response = client.Post<API_User>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("An error occurred communicating with the server.");
                return false;
            }
            else if (!response.IsSuccessful)
            {
                if (!string.IsNullOrWhiteSpace(response.Data.Message))
                {
                    Console.WriteLine("An error message was received: " + response.Data.Message);
                }
                else
                {
                    Console.WriteLine("An error response was received from the server. The status code is " + (int)response.StatusCode);
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        public API_User Login(LoginUser loginUser)
        {
            Console.WriteLine();
            Console.Clear();
            LoggingIN();
            
            Console.WriteLine();
            RestRequest request = new RestRequest(API_BASE_URL + "login");
            request.AddJsonBody(loginUser);
            IRestResponse<API_User> response = client.Post<API_User>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("An error occurred communicating with the server.");
                return null;
            }
            else if (!response.IsSuccessful)
            {
                if (!string.IsNullOrWhiteSpace(response.Data.Message))
                {
                    Console.Clear();
                    Console.WriteLine("An error message was received: " + response.Data.Message);
                }
                else
                {
                    Console.WriteLine("An error response was received from the server. The status code is " + (int)response.StatusCode);
                }
                return null;
            }
            else
            {
                client.Authenticator = new JwtAuthenticator(response.Data.Token);
                return response.Data;
            }
        }private static void LoggingIN()
        {
            Console.Clear();

            Console.WriteLine(@"                ___       ___           ___           ___                       ___           ___     ");
            Console.WriteLine(@"               /\__\     /\  \         /\  \         /\  \          ___        /\__\         /\  \     ");
            Console.WriteLine(@"              /:/  /    /::\  \       /::\  \       /::\  \        /\  \      /::|  |       /::\  \    ");
            Console.WriteLine(@"             /:/  /    /:/\:\  \     /:/\:\  \     /:/\:\  \       \:\  \    /:|:|  |      /:/\:\  \    ");
            Console.WriteLine(@"            /:/  /    /:/  \:\  \   /:/  \:\  \   /:/  \:\  \      /::\__\  /:/|:|  |__   /:/  \:\  \  ");
            Console.WriteLine(@"           /:/__/    /:/__/ \:\__\ /:/__/_\:\__\ /:/__/_\:\__\  __/:/\/__/ /:/ |:| /\__\ /:/__/_\:\__\  ");
            Console.WriteLine(@"           \:\  \    \:\  \ /:/  / \:\  /\ \/__/ \:\  /\ \/__/ /\/:/  /    \/__|:|/:/  / \:\  /\ \/__/  ");
            Console.WriteLine(@"            \:\  \    \:\  /:/  /   \:\ \:\__\    \:\ \:\__\   \::/__/         |:/:/  /   \:\ \:\__\     ");
            Console.WriteLine(@"             \:\  \    \:\/:/  /     \:\/:/  /     \:\/:/  /    \:\__\         |::/  /     \:\/:/  /     ");
            Console.WriteLine(@"              \:\__\    \::/  /       \::/  /       \::/  /      \/__/         /:/  /       \::/  /       ");
            Console.WriteLine(@"               \/__/     \/__/         \/__/         \/__/                     \/__/         \/__/        ");
            Console.WriteLine();
            Console.WriteLine(@"                                                          ___                                                 ");
            Console.WriteLine(@"                                              ___        /\__\                                                ");
            Console.WriteLine(@"                                             /\  \      /::|  |                                               ");
            Console.WriteLine(@"                                             \:\  \    /:|:|  |                                               ");
            Console.WriteLine(@"                                             /::\__\  /:/|:|  |__                                             ");
            Console.WriteLine(@"                                          __/:/\/__/ /:/ |:| /\__\                                 ");
            Console.WriteLine(@"                                         /\/:/  /    \/__|:|/:/  /                                  ");
            Console.WriteLine(@"                                         \::/__/         |:/:/  /                                    ");
            Console.WriteLine(@"                                          \:\__\         |::/  /                                     ");
            Console.WriteLine(@"                                           \/__/         /:/  /                                       ");
            Console.WriteLine(@"                                                         \/__/                                         ");
        }
        
    }
}
