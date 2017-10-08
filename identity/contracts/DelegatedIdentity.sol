pragma solidity ^0.4.4;

contract DelegatedIdentity {
    // remember owner
    address owner;

    // Any public keys
    mapping (address => bytes32) publicKeys;

    // And all available claims, traceable
    mapping (string => address) claims;

    function DelegatedIdentity() {
        owner = msg.sender;
    }

    function addClaim(string claim) {
        claims[claim] = msg.sender;
    }

    function checkClaim(string publicKey, string claim) returns (address authority) {
        if (sha256(publicKey) == publicKeys[msg.sender]) {
            // todo: find claim
            return claims[claim];
        }
    }

    function addRequester(address requester, string publicKey) {
        if (msg.sender == owner) {
            publicKeys[requester] = sha256(publicKey);
        }
    }
}