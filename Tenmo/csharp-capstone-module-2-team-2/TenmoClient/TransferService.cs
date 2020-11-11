using System;
using System.Collections.Generic;
using System.Text;
using RestSharp;
using RestSharp.Authenticators;
using TenmoClient.Data;
using System.Net;

namespace TenmoClient
{
    public class TransferService
    {
        private readonly static string API_BASE_URL = "https://localhost:44315/";
        private readonly IRestClient client = new RestClient();



        public Account GetBalance(int userId)
        {
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());


            try
            {
                RestRequest request = new RestRequest(API_BASE_URL + "Transfer/" + userId);
                IRestResponse<Account> response = client.Get<Account>(request);

                return response.Data;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public List<Transfer> GetAllTransfers(int id)
        {
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            try
            {
                RestRequest request = new RestRequest(API_BASE_URL + "Transfer/transfers/" + id);
                IRestResponse<List<Transfer>> response = client.Get<List<Transfer>>(request);
                return response.Data;
            }
            catch (Exception e)
            {

                return null;
            }
        }

        public Transfer GetTransfer(int UserId, int TransferId)
        {
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            try
            {
                RestRequest request = new RestRequest(API_BASE_URL + "Transfer/" + UserId + "/transfers/" + TransferId);
                IRestResponse<Transfer> response = client.Get<Transfer>(request);
                return response.Data;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public List<Account> GetAllUsers(int userId)
        {
            
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            try
            {
                RestRequest request = new RestRequest(API_BASE_URL + "Transfer/userid/"+ userId);
               
                IRestResponse<List<Account>> response = client.Get<List<Account>>(request);
                return response.Data;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public bool SendMoney(Transfer newTransfer)
        {
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            try
            {
                RestRequest request = new RestRequest(API_BASE_URL + "Transfer");
                request.AddJsonBody(newTransfer);
                IRestResponse response = client.Put(request);
                if ( response.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public bool RequestMoney(Transfer newTransfer)
        {
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            try
            {
                RestRequest request = new RestRequest(API_BASE_URL + "Transfer");
                request.AddJsonBody(newTransfer);
                IRestResponse response = client.Post(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }else
                {
                    return false;
                }
            }
            catch (Exception e)
            {

                return false;
            }
        }
        public bool ApproveTransfer(int userId,int transferId)
        {
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            try
            {
                RestRequest request = new RestRequest(API_BASE_URL + "Transfer/approve/" + transferId + "/" + userId);
                IRestResponse response = client.Put(request);
                if(response.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }else { return false; }
            }
            catch (Exception)
            {

                return false;
            }
        }
        public bool RejectTransfer(int userId,int transferId)
        {
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            try
            {
                RestRequest request = new RestRequest(API_BASE_URL + "Transfer/rejected/" + transferId + "/" + userId);
                IRestResponse response = client.Put(request);
                if(response.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }else { return false; }
            }
            catch (Exception)
            {

                return false;
            }
        }
    }
}
