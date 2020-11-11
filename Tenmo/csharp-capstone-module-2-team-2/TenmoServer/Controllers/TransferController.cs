using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;
using TenmoServer.DAO;
using TenmoServer.Models;


namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class TransferController : ControllerBase
    {
        private string IdiotMessage = "You done fucked up again";
        private readonly ITransferDao transferDAO;

        public TransferController(ITransferDao _transferDao)
        {
            transferDAO = _transferDao;
        }

        [HttpGet("userid/{userId}")]
        public ActionResult<List<Account>> GetList(int userId)
        {
            try
            {
                return Ok(transferDAO.GetAllUsers(userId));
            }
            catch (Exception e)
            {

                return BadRequest(IdiotMessage);
            }

        }

        [HttpGet("{Userid}")]
        public ActionResult<Account> GetCurrentBalance(int Userid)
        {
            try
            {
                return Ok(transferDAO.GetBalance(Userid));
            }
            catch (Exception e)
            {

                return BadRequest(IdiotMessage);
            }

        }

        [HttpGet("transfers/{id}")]
        public ActionResult<IList<Transfer>> GetListTransfers(int id)
        {
            try
            {
                return Ok(transferDAO.GetAllTransfers(id));
            }
            catch (Exception)
            {

                return BadRequest(IdiotMessage);
            }

        }

        [HttpGet("{Userid}/tranfers/{Transferid}")]
        public ActionResult<Transfer> GetTransfer(int Userid, int Transferid)
        {
            try
            {
                return Ok(transferDAO.GetTransfer(Userid, Transferid));
            }
            catch (Exception)
            {

                return BadRequest(IdiotMessage);
            }
        }
        [HttpPut]
        public ActionResult SendMoney(Transfer newTransfer)
        {
            try
            {
                if (transferDAO.SendMoney(newTransfer.account_From_ID, newTransfer.account_To_ID, newTransfer.AmountToTransfer))
                {
                    return Ok();

                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }
       
        [HttpPut("rejected/{transferId}/{userId}")]
        public ActionResult RejectTransfer(int userId , int transferId)
        {
            try
            {
                if(transferDAO.RejectTransfer(userId,transferId))
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }

            }
            catch (Exception e)
            {

                throw;
            }
        }
        
        [HttpPut("approve/{transferId}/{userid}")]
        public ActionResult ApproveTransfer(int userId, int transferId)
        {
            try
            {
                if (transferDAO.AcceptTransfer(userId,transferId))
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception)
            {

                return BadRequest();
            }
        }
        
        [HttpPost]
        public ActionResult RequestMoney(Transfer newTransfer)
        {

            try
            {
                if (transferDAO.RequestMoney(newTransfer.account_From_ID, newTransfer.account_To_ID, newTransfer.AmountToTransfer))
                {
                    return Ok();
                }
                else
                {

                    return BadRequest();
                }
            }
            catch (Exception e)
            {

                throw;
            }
        }
        
    }
}
