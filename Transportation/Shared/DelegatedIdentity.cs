using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Shared
{
    public class DelegatedIdentity
    {
        private string abi = @"[{""constant"":false,""inputs"":[{""name"":""requester"",""type"":""address""},{""name"":""publicKey"",""type"":""string""}],""name"":""addRequester"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[{""name"":""publicKey"",""type"":""string""},{""name"":""claim"",""type"":""string""}],""name"":""checkClaim"",""outputs"":[{""name"":""authority"",""type"":""address""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""name"":""claim"",""type"":""string""}],""name"":""addClaim"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""inputs"":[{""name"":""delegate"",""type"":""address""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""constructor""}]";

        Nethereum.Web3.Accounts.Account agentAccount;
        private Nethereum.Web3.Web3 web3;
        private Nethereum.Contracts.Contract contract;

        public async Task Initialize(byte[] privatekey, string contractAddress)
        {
            agentAccount = new Nethereum.Web3.Accounts.Account(privatekey);
            web3 = new Nethereum.Web3.Web3(agentAccount);
            contract = web3.Eth.GetContract(abi, contractAddress);
        }

        public async Task AddClaim(string claim)
        {
            var function = contract.GetFunction("addClaim");
            var transactionHash = await function.SendTransactionAsync(agentAccount.Address, claim, new Nethereum.Hex.HexTypes.HexBigInteger(900000));
            await MineAndGetReceiptAsync(web3, transactionHash);
        }

        public async Task<string> CheckClaim(string publicKey, string claim)
        {
            var function = contract.GetFunction("checkClaim");
            return await function.CallAsync<string>(publicKey, claim);
        }

        public async Task AddRequester(string requester, string publicKey)
        {
            var function = contract.GetFunction("addRequester");
            var transactionHash = await function.SendTransactionAsync(agentAccount.Address, requester, publicKey, new Nethereum.Hex.HexTypes.HexBigInteger(900000));
            await MineAndGetReceiptAsync(web3, transactionHash);
        }

        private async Task<Nethereum.RPC.Eth.DTOs.TransactionReceipt> MineAndGetReceiptAsync(Nethereum.Web3.Web3 web3, string transactionHash)
        {
            var web3Geth = new Nethereum.Geth.Web3Geth();
            var miningResult = await web3Geth.Miner.Start.SendRequestAsync(6);
            var receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);

            while (receipt == null)
            {
                Thread.Sleep(1000);
                receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
            }

            miningResult = await web3Geth.Miner.Stop.SendRequestAsync();
            return receipt;
        }
    }
}
