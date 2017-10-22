using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using UserAgent.Interfaces;

namespace DigitalAssistant
{
    class Program
    {
        private static string abi = @"[{""constant"":false,""inputs"":[{""name"":""requester"",""type"":""address""},{""name"":""publicKey"",""type"":""string""}],""name"":""addRequester"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[{""name"":""publicKey"",""type"":""string""},{""name"":""claim"",""type"":""string""}],""name"":""checkClaim"",""outputs"":[{""name"":""authority"",""type"":""address""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""name"":""claim"",""type"":""string""}],""name"":""addClaim"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""inputs"":[{""name"":""delegate"",""type"":""address""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""constructor""}]";
        private static string byteCode = "0x6060604052341561000f57600080fd5b6040516020806104af8339810160405280805160008054600160a060020a03909216600160a060020a0319909216919091179055505061045b806100546000396000f3006060604052600436106100565763ffffffff7c0100000000000000000000000000000000000000000000000000000000600035041663056bef9e811461005b5780631e6c593c146100bc5780637b3ae1f11461016b575b600080fd5b341561006657600080fd5b6100ba60048035600160a060020a03169060446024803590810190830135806020601f820181900481020160405190810160405281815292919060208401838380828437509496506101bc95505050505050565b005b34156100c757600080fd5b61014f60046024813581810190830135806020601f8201819004810201604051908101604052818152929190602084018383808284378201915050505050509190803590602001908201803590602001908080601f01602080910402602001604051908101604052818152929190602084018383808284375094965061027695505050505050565b604051600160a060020a03909116815260200160405180910390f35b341561017657600080fd5b6100ba60046024813581810190830135806020601f8201819004810201604051908101604052818152929190602084018383808284375094965061039a95505050505050565b60005433600160a060020a0390811691161415610272576002816000604051602001526040518082805190602001908083835b6020831061020e5780518252601f1990920191602091820191016101ef565b6001836020036101000a03801982511681845116808217855250505050505090500191505060206040518083038160008661646e5a03f1151561025057600080fd5b50506040518051600160a060020a038416600090815260016020526040902055505b5050565b600160a060020a03331660009081526001602052604080822054906002908590849051602001526040518082805190602001908083835b602083106102cc5780518252601f1990920191602091820191016102ad565b6001836020036101000a03801982511681845116808217855250505050505090500191505060206040518083038160008661646e5a03f1151561030e57600080fd5b505060405180519050600019161415610394576002826040518082805190602001908083835b602083106103535780518252601f199092019160209182019101610334565b6001836020036101000a03801982511681845116808217855250505050505090500191505090815260200160405190819003902054600160a060020a031690505b92915050565b336002826040518082805190602001908083835b602083106103cd5780518252601f1990920191602091820191016103ae565b6001836020036101000a038019825116818451168082178552505050505050905001915050908152602001604051908190039020805473ffffffffffffffffffffffffffffffffffffffff1916600160a060020a0392909216919091179055505600a165627a7a72305820969da09fd8c772c547f5d61b0c11d6611df2595d75638a850ed3249b816f99bd0029";

        static void Main(string[] args)
        {
            // The Digital Assistant, main account. When starting TestRPC, this address needs to be updated
            var ecKey = new Nethereum.Signer.EthECKey("0xc6bd77dc9f1b685865155d05c5d5719f4e3c51bdac1d5f09172b2573773bbd04");
            var privateKey = ecKey.GetPrivateKeyAsBytes();
            var account = new Nethereum.Web3.Accounts.Account(privateKey);

            // A new account for the agent
            var agentKey = Nethereum.Signer.EthECKey.GenerateKey();
            var agentAccount = new Nethereum.Web3.Accounts.Account(agentKey.GetPrivateKeyAsBytes());
            Console.WriteLine($"Agent created: {agentAccount.Address}");

            // Connect to the Ethereum network in the Digital Assistant account context, to create the contract
            var web3 = new Nethereum.Web3.Web3(account);

            var transactionHash = web3.Eth.DeployContract.SendRequestAsync(abi, byteCode, account.Address, new Nethereum.Hex.HexTypes.HexBigInteger(900000), agentAccount.Address).Result;
            var receipt = MineAndGetReceipt(web3, transactionHash);
            var contractAddress = receipt.ContractAddress;

            Console.WriteLine($"Contract created: {contractAddress}");
            Console.ReadKey();

            var actor = ActorProxy.Create<IUserAgent>(ActorId.CreateRandom(), new Uri("fabric:/Transportation/UserAgentActorService"));
            var retval = actor.GetTransportAsync("add schedule here", agentKey.GetPrivateKeyAsBytes(), contractAddress);
            Console.WriteLine($"Result for getting transport data: {retval.Result}");
            Console.ReadKey();
        }

        /// <summary>
        /// Start a miner and get back the result. Not needed in a live environment, but we're in testrpc now
        /// </summary>
        /// <param name="web3"></param>
        /// <param name="transactionHash"></param>
        /// <returns></returns>
        private static Nethereum.RPC.Eth.DTOs.TransactionReceipt MineAndGetReceipt(Nethereum.Web3.Web3 web3, string transactionHash)
        {
            var web3Geth = new Nethereum.Geth.Web3Geth();
            var miningResult = web3Geth.Miner.Start.SendRequestAsync(6).Result;
            var receipt = web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash).Result;

            while (receipt == null)
            {
                Thread.Sleep(1000);
                receipt = web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash).Result;
            }

            miningResult = web3Geth.Miner.Stop.SendRequestAsync().Result;
            return receipt;
        }
    }
}
