using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MoralisUnity.Platform.Objects;
using MoralisUnity.Samples.Shared.Data.Types;
using MoralisUnity.Samples.Shared.Exceptions;
using MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.Controller;
using MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.Model;
using MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.Model.Data.Types;
using MoralisUnity.Web3Api.Models;
using UnityEngine;
using Nft = MoralisUnity.Samples.Shared.Data.Types.Nft;

namespace MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.Service
{
	/// <summary>
	/// Handles communication with external sources (e.g. database/servers/contracts)
	///		* See <see cref="TheGameSingleton"/> 
	/// </summary>
	public class TheGameContractService : ITheGameService
	{
		// Properties -------------------------------------
		public PendingMessage PendingMessageActive { get { return _endingMessageActive; }}
		public PendingMessage PendingMessagePassive { get { return _pendingMessagePassive; }}
        public PendingMessage PendingMessageExtraDelay { get { return _pendingMessageExtraDelay; }}
        public bool HasExtraDelay { get { return true; }}
        
		// Fields -----------------------------------------
		private readonly PendingMessage _endingMessageActive = new PendingMessage("Please confirm transaction\nin your wallet", 0);
		private readonly PendingMessage _pendingMessagePassive = new PendingMessage("Loading ...", 0);
        private readonly PendingMessage _pendingMessageExtraDelay = new PendingMessage("Waiting for transaction ...", 0);
		private readonly TheGameContract _theGameContract = null;
        
        // Based on trial and error (and current network traffic)
        //  This is how long it takes for the state to change on the blockchain
        private const int DelayExtraAfterStateChangeMilliseconds = 5000;

		// Initialization Methods -------------------------
		public TheGameContractService()
		{
			_theGameContract = new TheGameContract();
		}

        
        // DELAY Methods -------------------------
        public UniTask DoExtraDelayAsync()
        {
            return UniTask.Delay(DelayExtraAfterStateChangeMilliseconds);
        }

        
        // DEBUGGING Methods -------------------------
        public async UniTask<string> GetLastRegisteredAddress()
        {
            string result = await _theGameContract.getLastRegisteredAddress();
            return result;
        }
        
        // GETTER Methods -------------------------
        public async UniTask<bool> IsRegisteredAsync()
        {
            bool result = await _theGameContract.isRegistered();
            return result;
        }
        
        
        public async UniTask<Reward> GetRewardsHistoryAsync()
        {
            Reward result = await _theGameContract.GetRewardsHistory();
            return result;
        }


        public async UniTask<int> GetGoldAsync()
        {
            int result = await _theGameContract.getGold();
            return result;
        }
        
        
        public async UniTask<List<TreasurePrizeDto>> GetTreasurePrizesAsync()
        {
            // Create Method Return Value
            List<TreasurePrizeDto> treasurePrizeDtos = new List<TreasurePrizeDto>();

            // Check System Status
            bool hasMoralisUser = await TheGameSingleton.Instance.HasMoralisUserAsync();
            if (!hasMoralisUser)
            {
                // Sometimes, ONLY warn
                throw new RequiredMoralisUserException();
            }

            // Get NFT Info
            MoralisUser moralisUser = await TheGameSingleton.Instance.GetMoralisUserAsync();
            var nftCollection = await Moralis.Web3Api.Account.GetNFTsForContract(
                moralisUser.ethAddress,
                _theGameContract.TreasurePrizeContractAddress,
                _theGameContract.ChainList);

            Debug.Log("nftCollection.Result.Count: " + nftCollection.Result.Count);

            // Create Method Return Value
            foreach (NftOwner nftOwner in nftCollection.Result)
            {
                string ownerAddress = nftOwner.OwnerOf;
                string tokenIdString = nftOwner.TokenId;
                string metadata = nftOwner.TokenUri;
                TreasurePrizeDto treasurePrizeDto = Nft.CreateNewFromMetadata<TreasurePrizeDto>(ownerAddress, tokenIdString, metadata);
                treasurePrizeDtos.Add(treasurePrizeDto);
            }

            // Finalize Method Return Value
            return treasurePrizeDtos;
        }
        
        // SETTER Methods -------------------------
        public async UniTask RegisterAsync()
        {
            string result = await _theGameContract.Register();
            Debug.Log($"RegisterAsync() result = {result}");
        }


        public async UniTask StartGameAndGiveRewardsAsync(int goldAmount)
        {
            string result = await _theGameContract.StartGameAndGiveRewards(goldAmount);
            Debug.Log($"StartGameAndGiveRewardsAsync() result = {result}");
        }


        public async UniTask UnregisterAsync()
        {
            string result = await _theGameContract.Unregister();
            Debug.Log($"UnregisterAsync() result = {result}");
        }


        public async UniTask SetGoldAsync(int targetBalance)
        {
            string result = await _theGameContract.setGold(targetBalance);
            Debug.Log($"SetGoldAsync() result = {result}");
        }

        
        public async UniTask SetGoldByAsync(int deltaBalance)
        {
            string result = await _theGameContract.setGoldBy(deltaBalance);
            Debug.Log($"SetGoldByAsync() result = {result}");
        }


        public async UniTask AddTreasurePrizeAsync(TreasurePrizeDto treasurePrizeDto)
        {
            string result = await _theGameContract.MintNftAsync(treasurePrizeDto);
            Debug.Log($"AddTreasurePrizeAsync() result = {result}");
        }


        public async UniTask SellTreasurePrizeAsync(TreasurePrizeDto treasurePrizeDto)
        {
            string result = await _theGameContract.BurnNftAsync(treasurePrizeDto);
            Debug.Log($"SellTreasurePrizeAsync() result = {result}");
        }

        
        public async UniTask DeleteAllTreasurePrizeAsync()
        {
            List<TreasurePrizeDto> treasurePrizeDtos = await GetTreasurePrizesAsync();
                
            string result = await _theGameContract.BurnNftsAsync(treasurePrizeDtos);
            Debug.Log($"BurnAllTreasurePrizeAsync() result = {result}");
        }

        // Event Handlers ---------------------------------

    }
}
