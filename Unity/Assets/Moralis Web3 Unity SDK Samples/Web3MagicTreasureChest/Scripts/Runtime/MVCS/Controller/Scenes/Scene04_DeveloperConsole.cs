using MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.View.UI.Scenes;
using System;
using System.Text;
using UnityEngine;
using MoralisUnity.Samples.Shared;
using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.Model;

namespace MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.Controller
{
    /// <summary>
    /// Replace with comments...
    /// </summary>
    public class Scene04_DeveloperConsole : MonoBehaviour
    {
        //  Properties ------------------------------------
 
        //  Fields ----------------------------------------
        [SerializeField]
        private Scene04_DeveloperConsoleUI _scene04_DeveloperConsoleUI;

        private StringBuilder _outputTextStringBuilder = new StringBuilder();

        //  Unity Methods----------------------------------
        protected async void Start()
        {
            bool hasMoralisUserAsync = await TheGameSingleton.Instance.HasMoralisUserAsync();
            if (!hasMoralisUserAsync)
            {
                throw new Exception("find existing user error");
            }


            TheGameSingleton.Instance.TheGameController.OnTheGameModelChanged.AddListener(OnModelChanged);
            TheGameSingleton.Instance.TheGameController.OnTheGameModelChangedRefresh();

            _scene04_DeveloperConsoleUI.UnregisterUserButtonUI.Button.onClick.AddListener(UnregisterUserButtonUI_OnClicked);
            _scene04_DeveloperConsoleUI.RegisterUserButtonUI.Button.onClick.AddListener(RegisterUserButtonUI_OnClicked);
            _scene04_DeveloperConsoleUI.AddGoldButtonUI.Button.onClick.AddListener(AddGoldButtonUI_OnClicked);
            _scene04_DeveloperConsoleUI.SpendGoldButtonUI.Button.onClick.AddListener(SpendGoldButtonUI_OnClicked);
            _scene04_DeveloperConsoleUI.AddTreasureButtonUI.Button.onClick.AddListener(AddTreasurePrizeButtonUI_OnClicked);
            _scene04_DeveloperConsoleUI.SellTreasureButtonUI.Button.onClick.AddListener(SellTreasurePrizeButtonUI_OnClicked);
            _scene04_DeveloperConsoleUI.BackButtonUI.Button.onClick.AddListener(BackButtonUI_OnClicked);

            RefreshUI();
            
        }


        //  General Methods -------------------------------
        private async UniTask RefreshUI()
        {
            _scene04_DeveloperConsoleUI.BackButtonUI.IsInteractable = true; // toggle some settings buttons, TODO

            _scene04_DeveloperConsoleUI.Text.text = _outputTextStringBuilder.ToString();
        }

        private async UniTask<bool> EnsureIsRegistered()
        {
            bool isRegistered = await TheGameSingleton.Instance.TheGameController.IsRegisteredUserAsync();
            if (!isRegistered)
            {
                _outputTextStringBuilder.Clear();
                _outputTextStringBuilder.AppendHeaderLine($"EnsureIsRegistered(). Failed.");
                await RefreshUI();
            }

            return isRegistered;
        }

        //  Event Handlers --------------------------------
        private async void OnModelChanged(TheGameModel theGameModel)
        {
            _scene04_DeveloperConsoleUI.TopUI.GoldCornerUI.Text.text = $"Gold {theGameModel.Gold.Value}/100";
            _scene04_DeveloperConsoleUI.TopUI.CollectionUI.Text.text = $"Treasure {theGameModel.TreasurePrizeDtos.Value.Count}/10";

            _outputTextStringBuilder.Clear();
            _outputTextStringBuilder.AppendHeaderLine($"OnModelChanged()");
            _outputTextStringBuilder.AppendHeaderLine($"Gold = {theGameModel.Gold.Value}");
            _outputTextStringBuilder.AppendHeaderLine($"TreasurePrizeDtos.Count = {theGameModel.TreasurePrizeDtos.Value.Count}");
            await RefreshUI();
        }


        private async void UnregisterUserButtonUI_OnClicked()
        {
            bool wasRegistered = await TheGameSingleton.Instance.TheGameController.IsRegisteredUserAsync();
            if (wasRegistered)
            {
                await TheGameSingleton.Instance.TheGameController.UnregisterUserAsync();
            }

            bool isRegistered = await TheGameSingleton.Instance.TheGameController.IsRegisteredUserAsync();

            _outputTextStringBuilder.Clear();
            _outputTextStringBuilder.AppendHeaderLine($"Register()");
            _outputTextStringBuilder.AppendHeaderLine($"wasRegistered = {wasRegistered}");
            _outputTextStringBuilder.AppendHeaderLine($"isRegistered = {isRegistered}");

            await RefreshUI();
        }

        private async void RegisterUserButtonUI_OnClicked()
        {
            bool wasRegistered = await TheGameSingleton.Instance.TheGameController.IsRegisteredUserAsync();
            if (!wasRegistered)
            {
                await TheGameSingleton.Instance.TheGameController.RegisterUserAsync();
            }

            bool isRegistered = await TheGameSingleton.Instance.TheGameController.IsRegisteredUserAsync();

            _outputTextStringBuilder.Clear();
            _outputTextStringBuilder.AppendHeaderLine($"Register()");
            _outputTextStringBuilder.AppendHeaderLine($"wasRegistered = {wasRegistered}");
            _outputTextStringBuilder.AppendHeaderLine($"isRegistered = {isRegistered}");

            await RefreshUI();
        }

        private async void AddGoldButtonUI_OnClicked()
        {
            if (!await EnsureIsRegistered())
            {
                return;
            }

            await TheGameSingleton.Instance.TheGameController.AddGold(1);

            _outputTextStringBuilder.AppendHeaderLine($"AddGold()");

            await RefreshUI();
        }



        private async void SpendGoldButtonUI_OnClicked()
        {
            if (!await EnsureIsRegistered())
            {
                return;
            }

            await TheGameSingleton.Instance.TheGameController.SpendGold(1);

            _outputTextStringBuilder.AppendHeaderLine($"SpendGold()");

            await RefreshUI();
        }

        private async void AddTreasurePrizeButtonUI_OnClicked()
        {
            if (!await EnsureIsRegistered())
            {
                return;
            }

            TreasurePrizeDto treasurePrizeDto = new TreasurePrizeDto("Blah again");

            await TheGameSingleton.Instance.TheGameController.AddTreasurePrize(treasurePrizeDto);

            _outputTextStringBuilder.AppendHeaderLine($"AddTreasurePrize()");
 
            await RefreshUI();
        }

        private async void SellTreasurePrizeButtonUI_OnClicked()
        {
            if (!await EnsureIsRegistered())
            {
                return;
            }

            TreasurePrizeDto treasurePrizeDto = new TreasurePrizeDto("Blah to delete, pull from existing list");
            await TheGameSingleton.Instance.TheGameController.SellTreasurePrize(treasurePrizeDto);

            _outputTextStringBuilder.AppendHeaderLine($"SellTreasurePrize()");

            await RefreshUI();
        }


        private void BackButtonUI_OnClicked()
        {
            TheGameSingleton.Instance.TheGameController.LoadSettingsSceneAsync();
        }
    }
}