using Microsoft.AspNetCore.Identity.UI.V3.Pages.Internal.Account;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Transactions;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    //Paddle Faster I hear banjos
    public class TransferDAO : ITransferDao
    {
        private string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=tenmo;Integrated Security=True";
        private TransactionScope transaction;

        public List<Transfer> GetAllTransfers(int userId)
        {
            List<Transfer> allTransfers = new List<Transfer>();
            string selectToTransfers = $"select transfer_id,transfer_type_id,transfer_status_id,account_to,account_from,username,amount  from transfers join users on account_to = user_id where account_from = @userId or account_to =@userId except select transfer_id,transfer_type_id,transfer_status_id,account_to,account_from,username,amount  from transfers join users on account_to = user_id  where account_to = @userId";
            string selectFromTransfers = $"select transfer_id,transfer_type_id,transfer_status_id,account_to,account_from,username,amount  from transfers join users on account_from = user_id where account_from = @userId  or account_to = @userId except select transfer_id,transfer_type_id,transfer_status_id,account_to,account_from,username,amount  from transfers join users on account_from = user_id  where account_from = @userId ";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(selectToTransfers, connection);
                    command.Parameters.AddWithValue("@userId", userId);
                    SqlDataReader readerTo = command.ExecuteReader();
                    while (readerTo.Read())
                    {
                        Transfer transfer = GetToTransfer(readerTo, userId);
                        allTransfers.Add(transfer);
                    }

                }
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(selectToTransfers, connection);
                    command = new SqlCommand(selectFromTransfers, connection);
                    command.Parameters.AddWithValue("@userId", userId);
                    SqlDataReader readerFrom = command.ExecuteReader();
                    while (readerFrom.Read())
                    {
                        Transfer transfer = GetFromTransfer(readerFrom, userId);
                        allTransfers.Add(transfer);

                    }
                }
            }
            catch (Exception e)
            {

                throw;
            }
            return allTransfers;
        }

        public List<User> GetAllUsers(int userId)
        {
            List<User> allUsers = new List<User>();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sqlstring = $"Select user_id, username from users where user_id != @userId";
                    SqlCommand command = new SqlCommand(sqlstring, connection);
                    command.Parameters.AddWithValue("@userId", userId);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        User user = GetUser(reader);
                        allUsers.Add(user);

                    }

                }

            }
            catch (Exception e)
            {

                throw;
            }
            return allUsers;
        }

        public Account GetBalance(int userId)
        {
            Account account = new Account();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sqlstring = $"Select balance from accounts where user_id = @userId";
                    SqlCommand command = new SqlCommand(sqlstring, connection);
                    command.Parameters.AddWithValue("@userId", userId);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        account = GetAccountBalance(reader);

                    }
                }
            }
            catch (Exception e)
            {

                throw;
            }
            return account;
        }

        public Transfer GetTransfer(int userId, int transferId)
        {
            Transfer transfer = new Transfer();
            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sqlstring = $"Select * from transfers where user_id = {userId} and transfer_id = @transfer_id";

                    SqlCommand command = new SqlCommand(sqlstring, connection);
                    command.Parameters.AddWithValue("@transfer_id", transferId);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        transfer = GetToTransfer(reader, userId);
                    }
                }
            }
            catch (Exception e)
            {

                throw;
            }
            return transfer;
        }
        private void AddMoney(int senderId, decimal amount)
        {



            try
            {
                Account account = new Account();
                account = GetBalance(senderId);
                account.Balance += amount;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string updateAccount = $"update accounts set balance = @amount where user_id = @senderId";

                    SqlCommand command = new SqlCommand(updateAccount, connection);
                    command.Parameters.AddWithValue("@amount", account.Balance);
                    command.Parameters.AddWithValue("@senderId", senderId);
                    int rowsAffected = command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {

                throw;
            }

        }
        private void SubtractMoney(int receiverId, decimal amount)
        {


            try
            {
                Account account = new Account();
                account = GetBalance(receiverId);
                account.Balance -= amount;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string updateAccount = $"update accounts set balance = @amount where user_id = @reciever";
                    SqlCommand command = new SqlCommand(updateAccount, connection);
                    command.Parameters.AddWithValue("@amount", account.Balance);
                    command.Parameters.AddWithValue("@reciever", receiverId);
                    int rowsAffected = command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {

                throw;
            }

        }
        private void AddToTransfer(int senderId, int receiverId, decimal amount)
        {
            string selectTransferType = "select transfer_type_id from transfer_types where transfer_type_desc = 'send'";
            string selectTransferStatus = "select transfer_status_id from transfer_statuses where transfer_status_desc = 'approved'";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string insertTransfer = $"insert into transfers values(({selectTransferType}),({selectTransferStatus}),@sender,@receiver,@amount)";
                    SqlCommand command = new SqlCommand(insertTransfer, connection);
                    command.Parameters.AddWithValue("@sender", senderId);
                    command.Parameters.AddWithValue("@receiver", receiverId);
                    command.Parameters.AddWithValue("@amount", amount);
                    int rowsaffected = command.ExecuteNonQuery();

                }
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public bool AcceptTransfer(int userId, int transferId)
        {

            bool wasAccepted = false;
            decimal amount = 0;
            int toAccount = 0;
            string selectTransferStatusId = "select transfer_status_id from transfer_statuses where transfer_status_desc = 'approved'";
            string updateTransferStatus = $"update transfers set transfer_status_id = ({selectTransferStatusId}) where transfer_id = {transferId} and account_to = {userId}";
            string selectAmount = $"select amount,account_to from transfers where transfer_id = {transferId} ";
            try
            {
                Account account = new Account();
                account = GetBalance(userId);
                using (transaction = new TransactionScope())
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(updateTransferStatus, connection);
                        int rowsAffected = command.ExecuteNonQuery();

                    }
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        SqlCommand sqlCommand = new SqlCommand(selectAmount, conn);
                        SqlDataReader reader = sqlCommand.ExecuteReader();
                        while (reader.Read())
                        {
                            amount = Convert.ToDecimal(reader["amount"]);
                            toAccount = Convert.ToInt32(reader["account_to"]);
                        }


                    }

                    if (account.Balance > amount)
                    {

                        wasAccepted = true;
                        SubtractMoney(userId, amount);
                        AddMoney(toAccount, amount);
                        transaction.Complete();

                    }
                    else
                    {
                        transaction.Dispose();
                    }


                }
            }
            catch (Exception e)
            {

                throw;
            }
            return wasAccepted;
        }


        public bool RejectTransfer(int userId, int transferId)
        {
            bool wasRejected = false;
            string selectTransferStatusId = "select transfer_status_id from transfer_statuses where transfer_status_desc = 'rejected'";
            string updateTransferStatus = $"update transfers set transfer_status_id = ({selectTransferStatusId}) where transfer_id = {transferId} and account_from = {userId}";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(updateTransferStatus, connection);
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        wasRejected = true;
                    }


                }
            }
            catch (Exception e)
            {

                throw;
            }
            return wasRejected;
        }


        public bool RequestMoney(int senderId, int receiverId, decimal amount)
        {
            bool transfer = true;
            string selectTypeId = "select transfer_type_id from transfer_types where transfer_type_desc = 'request'";
            string selectStatusId = "select transfer_status_id from transfer_statuses where transfer_status_desc = 'pending'";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sqlInsert = $"insert into transfers values(({selectTypeId}),({selectStatusId}),{senderId},{receiverId},{amount})";
                    SqlCommand command = new SqlCommand(sqlInsert, connection);
                    int rows = command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {

                throw;
            }
            return transfer;
        }
        public bool SendMoney(int senderId, int receiverId, decimal amount)
        {
            bool CanTransfer = false;
            Account senderAccount = new Account();



            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    senderAccount = GetBalance(senderId);


                    using (transaction = new TransactionScope())
                    {

                        if (amount > 0)
                        {

                            if (senderAccount.Balance > amount)
                            {

                                SubtractMoney(senderId, amount);
                                AddMoney(receiverId, amount);
                                AddToTransfer(senderId, receiverId, amount);
                                CanTransfer = true;
                                transaction.Complete();


                            }
                        }
                        else
                        {
                            transaction.Dispose();
                        }


                    }
                }

            }
            catch (Exception e)
            {

                throw;
            }
            return CanTransfer;
        }
        private Account GetAccountBalance(SqlDataReader reader)
        {
            Account account = new Account();

            account.Balance = Convert.ToDecimal(reader["balance"]);
            return account;
        }
        private User GetUser(SqlDataReader reader)
        {
            User user = new User();
            user.UserId = Convert.ToInt32(reader["user_id"]);
            user.Username = Convert.ToString(reader["username"]);
            return user;
        }
        private Transfer GetToTransfer(SqlDataReader reader, int userId)
        {
            Transfer transfer = new Transfer();
            transfer.transfer_ID = Convert.ToInt32(reader["transfer_id"]);
            transfer.type_ID = Convert.ToInt32(reader["transfer_type_id"]);
            transfer.status_ID = Convert.ToInt32(reader["transfer_status_id"]);
            transfer.account_From_ID = Convert.ToInt32(reader["account_from"]);
            transfer.account_To_ID = Convert.ToInt32(reader["account_to"]);
            transfer.account_To_UserName = Convert.ToString(reader["username"]);
            transfer.AmountToTransfer = Convert.ToDecimal(reader["amount"]);
            return transfer;
        }
        private Transfer GetFromTransfer(SqlDataReader reader, int userId)
        {
            Transfer transfer = new Transfer();
            transfer.transfer_ID = Convert.ToInt32(reader["transfer_id"]);
            transfer.type_ID = Convert.ToInt32(reader["transfer_type_id"]);
            transfer.status_ID = Convert.ToInt32(reader["transfer_status_id"]);
            transfer.account_From_ID = Convert.ToInt32(reader["account_from"]);
            transfer.account_To_ID = Convert.ToInt32(reader["account_to"]);
            transfer.account_From_UserName = Convert.ToString(reader["username"]);
            transfer.AmountToTransfer = Convert.ToDecimal(reader["amount"]);



            return transfer;







        }
    }
}
