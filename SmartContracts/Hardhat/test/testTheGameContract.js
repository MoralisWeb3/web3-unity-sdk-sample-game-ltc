﻿///////////////////////////////////////////////////////////
// REQUIRES
///////////////////////////////////////////////////////////
const {
    time,
    loadFixture,
} = require("@nomicfoundation/hardhat-network-helpers");
const { anyValue } = require("@nomicfoundation/hardhat-chai-matchers/withArgs");
const { expect } = require("chai");
const { string } = require("hardhat/internal/core/params/argumentTypes");


///////////////////////////////////////////////////////////
// TEST
///////////////////////////////////////////////////////////
describe("The Game Contract", function ()
{
    async function deployTokenFixture() 
    {

        // Arrange
        const [owner, addr1, addr2] = await ethers.getSigners();

        // Gold
        const Gold = await ethers.getContractFactory("Gold");
        const gold = await Gold.deploy();
        
        // TreasurePrize
        const TreasurePrize = await ethers.getContractFactory("TreasurePrize");
        const treasurePrize = await TreasurePrize.deploy();

        // TheGameContract
        const TheGameContract = await ethers.getContractFactory("TheGameContract");
        const theGameContract = await TheGameContract.deploy(gold.address, treasurePrize.address);

        return { theGameContract, addr1, addr2 };
    
    }

    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Deploys with no exceptions", async function ()
    {
        // Arrange
        const { theGameContract, addr1 } = await loadFixture(deployTokenFixture);

        // Act

        // Expect
        expect(true).to.equal(true);
    }),


    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Sets getGold to 100 when deployed, register", async function ()
    {
        // Arrange
        const { theGameContract, addr1 } = await loadFixture(deployTokenFixture);
        await theGameContract.connect(addr1).register();

        // Act
        const goldBalance = await theGameContract.connect(addr1).getGold();

        // Expect
        expect(goldBalance).to.equal(100);
    })

    
    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Sets getGold to 10 when register, setGold 10", async function ()
    {
        // Arrange
        const { theGameContract, addr1 } = await loadFixture(deployTokenFixture);
        await theGameContract.connect(addr1).register();

        // Act
        await theGameContract.connect(addr1).setGold(10);
        const goldBalance = await theGameContract.connect(addr1).getGold();

        // Expect
        expect(goldBalance).to.equal(10);
    }),


    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Sets getGold to 105 when register, setGoldBy 10, setGoldBy -5", async function ()
    {
        // Arrange
        const { theGameContract, addr1 } = await loadFixture(deployTokenFixture);
        await theGameContract.connect(addr1).register();

        // Act
        await theGameContract.connect(addr1).setGoldBy(10);
        await theGameContract.connect(addr1).setGoldBy(-5);
        const goldBalance = await theGameContract.connect(addr1).getGold();

        // Expect
        expect(goldBalance).to.equal(105);
        
    }),

    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Sets isRegistered to false when deployed", async function ()
    {
        // Arrange
        const { theGameContract, addr1 } = await loadFixture(deployTokenFixture);

        // Act
        const isRegistered = await theGameContract.connect(addr1).isRegistered();

        // Expect
        expect(isRegistered).to.equal(false);
    }),

    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Sets isRegistered to true when register", async function ()
    {
        // Arrange
        const { theGameContract, addr1 } = await loadFixture(deployTokenFixture);
        await theGameContract.connect(addr1).register();

        // Act  
        const isRegistered = await theGameContract.connect(addr1).isRegistered();

        // Expect
        expect(isRegistered).to.equal(true);
    }),

    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Sets getGold to 100 when register", async function ()
    {
        // Arrange
        const { theGameContract, addr1 } = await loadFixture(deployTokenFixture);
        await theGameContract.connect(addr1).register();

        // Act
        const goldBalance = await theGameContract.connect(addr1).getGold();

        // Expect
        expect(goldBalance).to.equal(100);
    }),

    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Sets isRegistered to false when register, unregister", async function ()
    {
        // Arrange
        const { theGameContract, addr1 } = await loadFixture(deployTokenFixture);
        await theGameContract.connect(addr1).register();
        await theGameContract.connect(addr1).unregister();

        // Act
        const isRegistered = await theGameContract.connect(addr1).isRegistered();

        // Expect
        expect(isRegistered).to.equal(false);
    }),
    

    
    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Sets result to DEFAULTS when deployed, getRewardsHistory", async function ()
    {
        // Arrange
        const { theGameContract, addr1 } = await loadFixture(deployTokenFixture);
        await theGameContract.connect(addr1).register();

        // Act
        var reward = await theGameContract.connect(addr1).getRewardsHistory ();

        // Expect
        expect(reward.length).to.not.equal(0);
    }),

    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Sets isRegistered to true when startGameAndGiveRewards (1)", async function ()
    {
        // Arrange
        const { theGameContract, addr1 } = await loadFixture(deployTokenFixture);
        await theGameContract.connect(addr1).register();
        var goldAmount = 10;
        await theGameContract.connect(addr1).startGameAndGiveRewards (goldAmount);

        // Act
        var [rewardTitle, rewardType, rewardPrice ] = await theGameContract.connect(addr1).getRewardsHistory ();

        // Expect
        expect(rewardTitle).to.not.equal("");
        expect(rewardType).to.not.equal(0)
        expect(rewardPrice).to.not.equal(0)
    }),

    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Throws revertedWith for startGameAndGiveRewards (100) when getGold() less than 100", async function ()
    {
        // Arrange
        const { theGameContract, addr1 } = await loadFixture(deployTokenFixture);
        await theGameContract.connect(addr1).register();
        const goldBalance = await theGameContract.connect(addr1).getGold();

        // Act
        // Expect
        await expect 
            ( 
                theGameContract.connect(addr1).startGameAndGiveRewards (goldBalance + 10)
            )
            .to.be.revertedWith("getGold() must be >= goldAmount to start the game");
    }),

    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Sets r between min/max when randomRange (min, max, n)", async function ()
    {
        // Arrange
        const { theGameContract, addr1 } = await loadFixture(deployTokenFixture);
        var nonce = 0;
        var min = 0;
        var max = 10;

        for (var i = 0; i< 100; i++)
        {
            // Act
            const r = await theGameContract.connect(addr1).randomRange(1, 10, nonce++);
            
            // Expect
            expect(r).to.greaterThanOrEqual(min).and.lessThanOrEqual(max);
        }
    }),

    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("getLastRegisteredAddress() is NOT addr1 when deployed", async function ()
    {
        // Arrange
        const { theGameContract, addr1 } = await loadFixture(deployTokenFixture);

        // Act
        const result = await theGameContract.connect(addr1).getLastRegisteredAddress();
        
        // Expect
        var string1 = String (result).toLocaleLowerCase();
        var string2 = String (addr1.address).toLocaleLowerCase();
        var areSame = string1.localeCompare(string2) == 0;
        expect(areSame).to.equal(false);
    }),

    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("getLastRegisteredAddress() is addr1 when deployed, register", async function ()
    {
        // Arrange
        const { theGameContract, addr1 } = await loadFixture(deployTokenFixture);
        await theGameContract.connect(addr1).register();

        // Act
        const result = await theGameContract.connect(addr1).getLastRegisteredAddress();
        
        // Expect
        var string1 = String (result).toLocaleLowerCase();
        var string2 = String (addr1.address).toLocaleLowerCase();
        var areSame = string1.localeCompare(string2) == 0;
        expect(areSame).to.equal(true);
    }),

    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("getMessage('blah') returns 'blah'", async function ()
    {
        // Arrange
        const { theGameContract, addr1 } = await loadFixture(deployTokenFixture);
        var messageIn = "blah";

        // Act
        const messageOut = await theGameContract.connect(addr1).getMessage(messageIn);
        
        // Expect
        expect(messageIn).to.equal(messageOut);
    }),

    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Calls safeReregisterAndBurnNfts() without error when deployed", async function ()
    {
        // Arrange
        const { theGameContract, addr1 } = await loadFixture(deployTokenFixture);
        var tokenIds = [];

        // Act
        const result = await theGameContract.connect(addr1).safeReregisterAndBurnNfts(tokenIds);
        
        // Expect
        expect(true).to.equal(true);
    }),

    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Calls safeReregisterAndBurnNfts() without error when register()", async function ()
    {
        // Arrange
        const { theGameContract, addr1 } = await loadFixture(deployTokenFixture);
        await theGameContract.connect(addr1).register();
        var tokenIds = [];

        // Act
        const result = await theGameContract.connect(addr1).safeReregisterAndBurnNfts(tokenIds);
        
        // Expect
        expect(true).to.equal(true);
    }),

    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Calls safeReregisterAndBurnNfts() without error when register()", async function ()
    {
        // Arrange
        const { theGameContract, addr1 } = await loadFixture(deployTokenFixture);
        await theGameContract.connect(addr1).register();
        var tokenIds = [];

        // Act
        const result = await theGameContract.connect(addr1).safeReregisterAndBurnNfts(tokenIds);
        
        // Expect
        expect(true).to.equal(true);
    })

});

