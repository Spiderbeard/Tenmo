using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface ITransferDao
    {
        Account GetBalance(int userId);
        List<User> GetAllUsers(int userId);
        List<Transfer> GetAllTransfers(int userId);
        Transfer GetTransfer(int userId, int transferId);
        bool SendMoney(int senderId, int recieverId, decimal amount);

        bool RequestMoney(int senderId, int receiverId, decimal amount);
        bool AcceptTransfer(int userId, int transferId);
        bool RejectTransfer(int userId, int transferId);

    }
}
