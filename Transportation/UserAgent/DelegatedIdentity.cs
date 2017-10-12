using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UserAgent
{
    public class DelegatedIdentity
    {
        private string senderAddress = "0x12890d2cce102216644c59dae5baed380d84830c";
        private string password = "password";

        private string contractAddress;
        private string abi = @"[{""constant"":false,""inputs"":[{""name"":""requester"",""type"":""address""},{""name"":""publicKey"",""type"":""string""}],""name"":""addRequester"",""outputs"":[],""payable"":false,""type"":""function""},{""constant"":false,""inputs"":[{""name"":""publicKey"",""type"":""string""},{""name"":""claim"",""type"":""string""}],""name"":""checkClaim"",""outputs"":[{""name"":""authority"",""type"":""address""}],""payable"":false,""type"":""function""},{""constant"":false,""inputs"":[{""name"":""claim"",""type"":""string""}],""name"":""addClaim"",""outputs"":[],""payable"":false,""type"":""function""},{""inputs"":[],""payable"":false,""type"":""constructor""}]";
        private string  byteCode = "0x6060604052341561000f57600080fd5b5b60008054600160a060020a03191633600160a060020a03161790555b5b61045c8061003c6000396000f300606060405263ffffffff7c0100000000000000000000000000000000000000000000000000000000600035041663056bef9e81146100535780631e6c593c146100b45780637b3ae1f114610163575b600080fd5b341561005e57600080fd5b6100b260048035600160a060020a03169060446024803590810190830135806020601f820181900481020160405190810160405281815292919060208401838380828437509496506101b695505050505050565b005b34156100bf57600080fd5b61014760046024813581810190830135806020601f8201819004810201604051908101604052818152929190602084018383808284378201915050505050509190803590602001908201803590602001908080601f01602080910402602001604051908101604052818152929190602084018383808284375094965061027295505050505050565b604051600160a060020a03909116815260200160405180910390f35b341561016e57600080fd5b6100b260046024813581810190830135806020601f8201819004810201604051908101604052818152929190602084018383808284375094965061039995505050505050565b005b60005433600160a060020a039081169116141561026d576002816000604051602001526040518082805190602001908083835b6020831061020957805182525b601f1990920191602091820191016101e9565b6001836020036101000a03801982511681845116808217855250505050505090500191505060206040518083038160008661646e5a03f1151561024b57600080fd5b50506040518051600160a060020a038416600090815260016020526040902055505b5b5050565b600160a060020a03331660009081526001602052604080822054906002908590849051602001526040518082805190602001908083835b602083106102c957805182525b601f1990920191602091820191016102a9565b6001836020036101000a03801982511681845116808217855250505050505090500191505060206040518083038160008661646e5a03f1151561030b57600080fd5b505060405180519050600019161415610392576002826040518082805190602001908083835b6020831061035157805182525b601f199092019160209182019101610331565b6001836020036101000a03801982511681845116808217855250505050505090500191505090815260200160405190819003902054600160a060020a031690505b5b92915050565b336002826040518082805190602001908083835b602083106103cd57805182525b601f1990920191602091820191016103ad565b6001836020036101000a038019825116818451168082178552505050505050905001915050908152602001604051908190039020805473ffffffffffffffffffffffffffffffffffffffff1916600160a060020a03929092169190911790555b505600a165627a7a723058200888c62d46467a98526c597309f5e4c029610dddc49d653c7cbab28cb47c31370029";

        private Nethereum.Web3.Web3 web3;
        private Nethereum.Contracts.Contract contract;

        public async Task Initialize()
        {
            web3 = new Nethereum.Web3.Web3();
            contract = web3.Eth.GetContract(abi, contractAddress);

            var unlockResult = await web3.Personal.UnlockAccount.SendRequestAsync(senderAddress, password, 120);
            var transactionHash = await web3.Eth.DeployContract.SendRequestAsync(abi, byteCode, senderAddress, new Nethereum.Hex.HexTypes.HexBigInteger(900000));
            var receipt = await MineAndGetReceiptAsync(web3, transactionHash);

            contractAddress = receipt.ContractAddress;
            contract = web3.Eth.GetContract(abi, contractAddress);
        }

        public async Task AddClaim(string claim)
        {
            var function = contract.GetFunction("addClaim");
            var transactionHash = await function.SendTransactionAsync(senderAddress, claim, new Nethereum.Hex.HexTypes.HexBigInteger(900000));
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
            var transactionHash = await function.SendTransactionAsync(senderAddress, requester, publicKey, new Nethereum.Hex.HexTypes.HexBigInteger(900000));
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
