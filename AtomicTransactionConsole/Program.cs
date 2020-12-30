using Algorand;
using Algorand.Client;
using Algorand.V2;
using System;
using System.Collections.Generic;

namespace AtomicTransactionConsole
{
    public class Program
    {
        static void Main(string[] args)
        {
            //Account Addresses Created


            //DCEXC2QRLIMCECYG5LD2ECOMFSN6E4M65QTZPYQZF2U6XWF44MO7UC5DKI
            //scissors ill later clump toward machine corn exotic hungry hockey cover popular ugly episode across account peasant find dutch turn tennis mule educate abstract tackle
            //SLNSI3HGMEGJIYOTV5C3UIGRY5JW3HOO3LB43NIQ257LZBTAPJWOVANN2Y
            //noise office hip latin giggle december adjust casual shoe rabbit stable home write river asset year tourist patch swamp trust snap orchard size ability project
            //7STWPM7PSKYYVG4Z3HPL6GYZ3U6KTV4C6BESLVDULLCHNYSN6UHCZDPOEQ
            //misery desert drift net lawsuit stay dry spoon river blur spoon hire speak supply ice magnet oxygen engage trap put eternal chicken fall about april

            //Performing Atomic Transactions

            //PureStake Configuration with AlgodApi
            AlgodApi algodApiInstance = new AlgodApi("https://testnet-algorand.api.purestake.io/ps2", "B3SU4KcVKi94Jap2VXkK83xx38bsv95K5UZm2lab");
            //First Account Address - acc1
            var acc1 = "SLNSI3HGMEGJIYOTV5C3UIGRY5JW3HOO3LB43NIQ257LZBTAPJWOVANN2Y";
            //Second Account Address - acc2
            var acc2 = "7STWPM7PSKYYVG4Z3HPL6GYZ3U6KTV4C6BESLVDULLCHNYSN6UHCZDPOEQ";
            //Key of the account that will fund other Accounts
            var key = "scissors ill later clump toward machine corn exotic hungry hockey cover popular ugly episode across account peasant find dutch turn tennis mule educate abstract tackle";
            //Account that will fund other accounts
            var accountAddress = "DCEXC2QRLIMCECYG5LD2ECOMFSN6E4M65QTZPYQZF2U6XWF44MO7UC5DKI";
            Account src = new Account(key);
            //Creating a transactionParams on the V2
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

            //amount to fund other accounts
            var amount = Utils.AlgosToMicroalgos(1);
            //First Transaction
            var tx = Utils.GetPaymentTransaction(new Address(accountAddress), new Address(acc1), amount, "pay message", transParams);
            //Second Transaction
            var tx2 = Utils.GetPaymentTransaction(new Address(accountAddress), new Address(acc2), amount, "pay message", transParams);
            //SignedTransaction signedTx2 = src.SignTransactionWithFeePerByte(tx2, feePerByte);
            //Grouping the Transactions (Atomic Transactions)
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
            Console.WriteLine("You have successefully arrived the end of this test, please press and key to exist.");
            Console.ReadKey();
        }
    }
}
