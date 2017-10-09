var di = artifacts.require("./DelegatedIdentity.sol");

module.exports = function(deployer) {
  deployer.deploy(di);
};
