using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AtomicTransactions.Models;
using Algorand;
using Algorand.Client;
using Algorand.V2;

namespace AtomicTransactions.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Atomic()
        {
            AlgodApi algodApiInstance = new AlgodApi("https://testnet-algorand.api.purestake.io/ps2", "B3SU4KcVKi94Jap2VXkK83xx38bsv95K5UZm2lab");
            var acc1 = "SLNSI3HGMEGJIYOTV5C3UIGRY5JW3HOO3LB43NIQ257LZBTAPJWOVANN2Y";
            var acc2 = "7STWPM7PSKYYVG4Z3HPL6GYZ3U6KTV4C6BESLVDULLCHNYSN6UHCZDPOEQ";
            var key = "scissors ill later clump toward machine corn exotic hungry hockey cover popular ugly episode across account peasant find dutch turn tennis mule educate abstract tackle";
            var accountAddress = "DCEXC2QRLIMCECYG5LD2ECOMFSN6E4M65QTZPYQZF2U6XWF44MO7UC5DKI";
            Account src = new Account(key);
            Algorand.V2.Model.TransactionParametersResponse transParams;
            try
            {
                transParams = algodApiInstance.TransactionParams();
            }
            catch (ApiException e)
            {
                throw new Exception("Could not get params", e);
            }
            // let's create a transaction group
            var amount = Utils.AlgosToMicroalgos(1);
            var tx = Utils.GetPaymentTransaction(new Address(accountAddress), new Address(acc1), amount, "pay message", transParams);
            var tx2 = Utils.GetPaymentTransaction(new Address(accountAddress), new Address(acc2), amount, "pay message", transParams);
            //SignedTransaction signedTx2 = src.SignTransactionWithFeePerByte(tx2, feePerByte);
            Digest gid = TxGroup.ComputeGroupID(new Transaction[] { tx, tx2 });
            tx.AssignGroupID(gid);
            tx2.AssignGroupID(gid);
            // already updated the groupid, sign
            var signedTx = src.SignTransaction(tx);
            var signedTx2 = src.SignTransaction(tx2);
            try
            {
                //contact the signed msgpack
                List<byte> byteList = new List<byte>(Algorand.Encoder.EncodeToMsgPack(signedTx));
                byteList.AddRange(Algorand.Encoder.EncodeToMsgPack(signedTx2));
                var id = algodApiInstance.RawTransaction(byteList.ToArray());
                Console.WriteLine("Successfully sent tx group with first tx id: " + id);
            }
            catch (ApiException e)
            {
                // This is generally expected, but should give us an informative error message.
                Console.WriteLine("Exception when calling algod#rawTransaction: " + e.Message);
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
