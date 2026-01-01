using Empire.NPC;
using Empire.NPC.Data.Enums;
using Empire.NPC.S1API_NPCs;
using Empire.Phone.Data;
using Empire.Quest;
using Empire.Utilities;
using Empire.Utilities.QuestHelpers;
using Empire_S1API.Utilities;
using MelonLoader;
using S1API.Console;
using S1API.GameTime;
using S1API.Utils;
using S1API.Money;
using S1API.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Empire.Phone
{

    public class EmpirePhoneApp : S1API.PhoneApp.PhoneApp
    {
        public static EmpirePhoneApp Instance { get; set; }
        protected override string AppName => "Empire";
        protected override string AppTitle => "Empire";
        protected override string IconLabel => "Empire";
        protected override string IconFileName => "";
		protected override Sprite? IconSprite => EmpireResourceLoader.LoadEmbeddedIcon("EmpireIcon.png");

		private List<QuestData> quests;
        private RectTransform questListContainer;
        private RectTransform buyerListContainer;
        private Text
            questTitle,
            questTask,
            questReward,
            deliveryStatus,
            acceptLabel,
            cancelLabel,
            refreshLabel,
            manageLabel;
        //relationsLabel,
        //productLabel,
        //shippingLabel;
        private Button
            acceptButton,
            cancelButton,
            refreshButton,
            manageButton;
            //productButton;
			//relationsButton,
			//shippingButton;
		private Text statusText;
        private Text managementTabLabel;
        public static int Index;

        private EmpireNPC selectedBuyer;
        private GameObject managementDetailPanel;

        public static string QuestImage;

        private GameObject messageContainer;
        private S1API.Quests.Quest? q;

        public static void Reset()
        {
            if (QuestDelivery.QuestActive && QuestDelivery.Active != null)
            {
                MelonLogger.Msg("[EmpireApp] Resetting active quest state.");
                QuestDelivery.Active.Cleanup();
            }     
            if (Instance.q != null)
            {
                MelonLogger.Msg("[EmpireApp] Resetting EmpirePhoneApp instance.");
                Instance.q = null;
            }
        }

        private Transform GetMessageParent()
        {
            // Prefer the explicit message container so temporary messages don't affect the main layout.
            if (messageContainer != null)
                return messageContainer.transform;
            // Fallback to the detail panel (where static content lives).
            if (managementDetailPanel != null)
                return managementDetailPanel.transform;
            // Final fallback: no parent available (UI factory should handle a null parent).
            return null;
        }

        protected override void OnCreated()
        {
            base.OnCreated();
            MelonLogger.Msg("[EmpirePhoneApp] OnCreated called");
            Instance = this;
            Reset();
            TimeManager.OnDayPass -= LoadQuests;
            TimeManager.OnDayPass += LoadQuests;
            MelonLogger.Msg("✅ TimeManager.OnDayPass event subscribed");
        }

		// No longer used
		//private void InitializeDealers()
		//{
		//    try
		//    {
		//        //JSONDeserializer.Initialize();
		//        //MelonLogger.Msg("✅ Dealers initialized");
		//        Contacts.Update();
		//        //MelonLogger.Msg($"Contacts.Buyers Count: {Contacts.Buyers.Count}");
		//    }
		//    catch (Exception ex)
		//    {
		//        MelonLogger.Error($"Failed to initialize dealers: {ex}");
		//    }
		//}

		protected override void OnCreatedUI(GameObject container)        {

            var bg = UIFactory.Panel("MainBG", container.transform, Color.black, fullAnchor: true);

            // Set the top bar title to "Deals" for the default quest screen.
            UIFactory.TopBar(name: "TopBar",
                parent: bg.transform,
                title: "Deals",
                topbarSize: 0.82f,
                paddingLeft: 75,
                paddingRight: 75,
                paddingTop: 0,
                paddingBottom: 35);

            // Status text below top bar
            statusText = UIFactory.Text("Status", "", bg.transform, 14);
            statusText.rectTransform.anchorMin = new Vector2(0.7f, 0.85f);
            statusText.rectTransform.anchorMax = new Vector2(0.98f, 0.9f);
            statusText.alignment = TextAnchor.MiddleRight;

            var leftPanel = UIFactory.Panel("QuestListPanel", bg.transform, new Color(0.1f, 0.1f, 0.1f),
                new Vector2(0.02f, 0.05f), new Vector2(0.49f, 0.82f));
            questListContainer = UIFactory.ScrollableVerticalList("QuestListScroll", leftPanel.transform, out _);
            UIFactory.FitContentHeight(questListContainer);

            var rightPanel = UIFactory.Panel("DetailPanel", bg.transform, new Color(0.12f, 0.12f, 0.12f),
                new Vector2(0.49f, 0f), new Vector2(0.98f, 0.82f));

            // Use vertical layout with padding and spacing like Tax & Wash
            UIFactory.VerticalLayoutOnGO(rightPanel, spacing: 14, padding: new RectOffset(24, 50, 15, 70));
            questTitle = UIFactory.Text("Title", "", rightPanel.transform, 24, TextAnchor.MiddleLeft, FontStyle.Bold);
            questTask = UIFactory.Text("Task", "", rightPanel.transform, 18, TextAnchor.MiddleLeft, FontStyle.Normal);
            questReward = UIFactory.Text("Reward", "", rightPanel.transform, 18, TextAnchor.MiddleLeft, FontStyle.Normal);

            deliveryStatus = UIFactory.Text("DeliveryStatus", "", rightPanel.transform, 16, TextAnchor.MiddleLeft, FontStyle.Italic);
            deliveryStatus.color = new Color(32, 0x82, 0xF6, 0xff);

            // Create a horizontal container for Refresh and Cancel
            var topButtonRow = UIFactory.Panel("TopButtonRow", rightPanel.transform, Color.clear);
            UIFactory.HorizontalLayoutOnGO(topButtonRow, spacing: 12);
            UIFactory.SetLayoutGroupPadding(topButtonRow.GetComponent<HorizontalLayoutGroup>(), 0, 0, 0, 0);

            // Accept Button (separate row)
            var acceptTuple = UIFactory.RoundedButtonWithLabel("AcceptBtn", "No quest selected", rightPanel.transform, new Color32(32, 0x82, 0xF6, 0xff), 460f, 60f, 22, Color.black);
            GameObject acceptGO = acceptTuple.Item1;
            Button acceptBtn = acceptTuple.Item2;
            Text acceptLbl = acceptTuple.Item3;
            acceptButton = acceptBtn;
            acceptLabel = acceptLbl;
            ButtonUtils.Disable(acceptBtn, acceptLabel, "No quest selected");

            // Cancel Button
            var cancelTuple = UIFactory.RoundedButtonWithLabel("CancelBtn", "Cancel current delivery", rightPanel.transform, new Color32(32, 0x82, 0xF6, 0xff), 460f, 60f, 22, Color.black);
            GameObject cancelGO = cancelTuple.Item1;
            Button cancelBtn = cancelTuple.Item2;
            Text cancelLbl = cancelTuple.Item3;
            cancelButton = cancelBtn;
            cancelLabel = cancelLbl;
            if (!QuestDelivery.QuestActive)
                ButtonUtils.Disable(cancelButton, cancelLabel, "No quest active");

            //Manage button
            var manageTuple = UIFactory.RoundedButtonWithLabel
                (
                    "ManageBtn",
                    "Manage",
                    bg.transform,
                    new Color(0.2f, 0.2f, 0.2f, 1f),
                    100, // Width 
                    40,  // Height
                    16,  // Font size
                    Color.white
                );
            GameObject manageGO = manageTuple.Item1;
            Button manageBtn = manageTuple.Item2;
            Text ManageLbl = manageTuple.Item3;
            manageButton = manageBtn;
            manageLabel = ManageLbl;

            ButtonUtils.AddListener(manageBtn, () => OpenManageUI(bg));

            // Set the position and size of the manage button
            var manageRect = manageGO.GetComponent<RectTransform>();
            manageRect.anchorMin = new Vector2(0.75f, 0.96f);
            manageRect.anchorMax = new Vector2(0.85f, 1f);
            manageRect.pivot = new Vector2(1f, 1f);
            manageRect.anchoredPosition = new Vector2(-10f, -10f);
            manageRect.sizeDelta = new Vector2(50, 25);

            // Refresh Button
            var refreshTuple = UIFactory.RoundedButtonWithLabel(
                "RefreshBtn",
                "Contact Dealers",
                bg.transform,
                new Color(0.2f, 0.2f, 0.2f, 1f),
                300,
                90,
                22,
                Color.white
            );
            GameObject refreshGO = refreshTuple.Item1;
            Button refreshBtn = refreshTuple.Item2;
            Text refreshLbl = refreshTuple.Item3;
            refreshButton = refreshBtn;
            refreshLabel = refreshLbl;

            ButtonUtils.AddListener(refreshButton, () => RefreshButton());

            // Set the position and size of the refresh button
            var refreshRect = refreshGO.GetComponent<RectTransform>();
            refreshRect.anchorMin = new Vector2(0.9f, 0.96f);
            refreshRect.anchorMax = new Vector2(1f, 1f);
            refreshRect.pivot = new Vector2(1f, 1f);
            refreshRect.anchoredPosition = new Vector2(-10f, -10f);
            refreshRect.sizeDelta = new Vector2(50, 25);
    //        if (Contacts.Buyers == null || Contacts.Buyers.Count == 0)
    //        {
				//MelonLogger.Msg(
	   //             $"[EmpirePhoneApp] Initializing dealers on UI creation because Contacts.Buyers is " +
	   //             $"{(Contacts.Buyers == null ? "null" : Contacts.Buyers.Count == 0 ? "empty" : "populated")}"
    //            );
				//InitializeDealers();
    //        }
            MelonCoroutines.Start(WaitForBuyerAndInitialize());
        }

        private void OpenManageUI(GameObject bg)
        {
            // Create main management modal panel
            var managementPanel = UIFactory.Panel("ManagementPanel", bg.transform, new Color(200 / 255f, 200 / 255f, 200 / 255f, 0.3f), fullAnchor: true);
            managementPanel.gameObject.SetActive(true);
            managementPanel.transform.SetAsLastSibling();


            // Create Top Bar panel (fixed pixel height to avoid anchor rounding issues)
            var topBar = UIFactory.Panel("ManageTopBar", managementPanel.transform, new Color(50 / 255f, 50 / 255f, 50 / 255f, 1f));
            var topBarRect = topBar.GetComponent<RectTransform>();
            // Make top bar a fixed-height strip anchored to top
            int topBarHeight = 110;
            topBarRect.anchorMin = new Vector2(0f, 1f);
            topBarRect.anchorMax = new Vector2(1f, 1f);
            topBarRect.pivot = new Vector2(0.5f, 1f);
            topBarRect.sizeDelta = new Vector2(0f, topBarHeight);
            topBarRect.anchoredPosition = new Vector2(0f, 0f);
            UIFactory.HorizontalLayoutOnGO(topBar, spacing: 20, padLeft: 20, padRight: 20, padTop: 10, padBottom: 10, alignment: TextAnchor.MiddleLeft);

            // Add Reputation, Product, and Shipping buttons to the top bar
            var repTuple = UIFactory.RoundedButtonWithLabel("RepButton", "Reputation", topBar.transform, new Color(0.2f, 0.2f, 0.2f, 1f), 120, 40, 16, Color.white);
            ButtonUtils.ClearListeners(repTuple.Item2);
            ButtonUtils.AddListener(repTuple.Item2, () => UpdateBuyerDetails("Reputation"));

            var prodTuple = UIFactory.RoundedButtonWithLabel("ProdButton", "Product", topBar.transform, new Color(0.2f, 0.2f, 0.2f, 1f), 120, 40, 16, Color.white);
            ButtonUtils.ClearListeners(prodTuple.Item2);
            ButtonUtils.AddListener(prodTuple.Item2, () => UpdateBuyerDetails("Product"));

            var shipTuple = UIFactory.RoundedButtonWithLabel("ShipButton", "Shipping", topBar.transform, new Color(0.2f, 0.2f, 0.2f, 1f), 120, 40, 16, Color.white);
            ButtonUtils.ClearListeners(shipTuple.Item2);
            ButtonUtils.AddListener(shipTuple.Item2, () => UpdateBuyerDetails("Shipping"));

            // Add Gifts tab button
            var giftsTuple = UIFactory.RoundedButtonWithLabel("GiftsButton", "Gifts", topBar.transform, new Color(0.2f, 0.2f, 0.2f, 1f), 120, 40, 16, Color.white);
            ButtonUtils.ClearListeners(giftsTuple.Item2);
            ButtonUtils.AddListener(giftsTuple.Item2, () => UpdateBuyerDetails("Gifts"));

            // Add Debt tab button
            var debtTuple = UIFactory.RoundedButtonWithLabel("DebtButton", "Debt", topBar.transform, new Color(0.2f, 0.2f, 0.2f, 1f), 120, 40, 16, Color.white);
            ButtonUtils.ClearListeners(debtTuple.Item2);
            ButtonUtils.AddListener(debtTuple.Item2, () => UpdateBuyerDetails("Debt"));

            var spacer = topBar.AddComponent<LayoutElement>();
            spacer.flexibleWidth = 1;

            // Create active tab label
            managementTabLabel = UIFactory.Text("ManagementTabLabel", "Reputation", topBar.transform, 20, TextAnchor.MiddleCenter, FontStyle.Bold);

            // Create Close button at the far right in top bar
            var closeTuple = UIFactory.RoundedButtonWithLabel("CloseButton", "X", topBar.transform, new Color32(235, 53, 56, 255), 50, 40, 16, Color.white);
            ButtonUtils.AddListener(closeTuple.Item2, () =>
            {
                Object.Destroy(managementPanel);
                RefreshQuestList(); // Refresh main list to reflect selection
            });
            closeTuple.Item1.GetComponent<RectTransform>().SetAsLastSibling();

            // Create Content Panel (fill entire modal — DO NOT offset here)
            var contentPanel = UIFactory.Panel("ManageContent", managementPanel.transform, new Color(0.12f, 0.12f, 0.12f, 1f));
            var contentRect = contentPanel.GetComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0f, 0f);
            contentRect.anchorMax = new Vector2(1f, 0.85f);
            contentRect.offsetMin = Vector2.zero;
            contentRect.offsetMax = Vector2.zero; // ensure full-height content; right panel will be offset instead

            // Left Panel: Buyer List (40% width)
            var leftPanel = UIFactory.Panel("BuyerListPanel", contentPanel.transform, new Color(0.1f, 0.1f, 0.1f, 1f));
            var leftRect = leftPanel.GetComponent<RectTransform>();
            leftRect.anchorMin = new Vector2(0, 0);
            leftRect.anchorMax = new Vector2(0.4f, 1);
            leftRect.offsetMin = Vector2.zero;
            leftRect.offsetMax = Vector2.zero;
            buyerListContainer = UIFactory.ScrollableVerticalList("BuyerListScroll", leftPanel.transform, out _);
            UIFactory.FitContentHeight(buyerListContainer);
            PopulateBuyerList(buyerListContainer);

            // Right Panel: Detail Display (60% width)
            var rightPanel = UIFactory.Panel("DetailPanel", contentPanel.transform, new Color(0.12f, 0.12f, 0.12f, 1f));
            var rightRect = rightPanel.GetComponent<RectTransform>();
            rightRect.anchorMin = new Vector2(0.4f, 0f); // left 40%, bottom
            rightRect.anchorMax = new Vector2(1f, 0.89f);   // right edge, top edge of contentPanel
            rightRect.offsetMin = Vector2.zero;
            rightRect.offsetMax = Vector2.zero; 
            UIFactory.VerticalLayoutOnGO(rightPanel, spacing: 14, padding: new RectOffset(24, 50, 15, 70));
            managementDetailPanel = rightPanel;

            // Create a transparent message container outside the VerticalLayoutGroup so messages don't affect layout
            messageContainer = UIFactory.Panel("MessageContainer", contentPanel.transform, new Color(0f, 0f, 0f, 0f));
            var msgRect = messageContainer.GetComponent<RectTransform>();
            // Align message container to the same horizontal area as the right panel and keep it anchored to the bottom
            msgRect.anchorMin = new Vector2(0.4f, 0f);
            msgRect.anchorMax = new Vector2(1f, 0f);
            msgRect.pivot = new Vector2(0.5f, 0f);
            // fixed height for messages (adjust if needed)
            msgRect.sizeDelta = new Vector2(0f, 70f);
            msgRect.anchoredPosition = new Vector2(0f, 10f);
            // Make sure the message container doesn't receive a visible background
            var msgImage = messageContainer.GetComponent<Image>();
            if (msgImage != null) msgImage.color = new Color(0f, 0f, 0f, 0f);
            // Ensure message container renders above content but below top bar
            messageContainer.transform.SetAsLastSibling();
            topBar.transform.SetAsLastSibling();

            //// Auto-select first buyer if any exists and update details
            //if (Contacts.Buyers.Values.Any(b => b.IsInitialized))
            //{
            //    if (selectedBuyer == null || !selectedBuyer.IsInitialized)
            //    {
            //        selectedBuyer = Contacts.Buyers.Values.First(b => b.IsInitialized);
            //    }
            //    UpdateBuyerDetails("Reputation");
            //}

			// Auto-select first *unlocked* buyer if none selected
			var firstUnlocked = Contacts.Buyers.Values.FirstOrDefault(b => b.IsUnlocked);

			if (firstUnlocked != null)
			{
				if (selectedBuyer == null || !selectedBuyer.IsUnlocked)
					selectedBuyer = firstUnlocked;

				UpdateBuyerDetails("Reputation");
			}
			else
			{
				// No unlocked buyers yet — show empty UI
				selectedBuyer = null;
				UpdateBuyerDetails("Reputation");
			}

		}

		private void UpdateBuyerDetails(string tab)
        {
            if (managementDetailPanel == null)
            {
                MelonLogger.Warning("[EmpirePhoneApp] UpdateBuyerDetails called but managementDetailPanel is null.");
                return;
            }

            // Clear detail panel
            for (int i = managementDetailPanel.transform.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(managementDetailPanel.transform.GetChild(i).gameObject);
            }

            if (selectedBuyer == null)
            {
                UIFactory.Text("NoSelectionText", "Select a contact from the list.", managementDetailPanel.transform, 18);
                return;
            }

            string content = "";
            if (tab == "Reputation")
            {
                var imagePath = selectedBuyer.Image ?? "fallback-dealer.png";
				UIFactory.SetIcon(EmpireResourceLoader.LoadEmbeddedIcon(imagePath), managementDetailPanel.transform);

                var icon = managementDetailPanel.transform.GetComponentInChildren<Image>();
                if (icon != null)
                    icon.GetComponent<RectTransform>().sizeDelta = new Vector2(127, 127);
                content = $"<b>Reputation:</b> {selectedBuyer.DealerSaveData.Reputation}";
                
                if (selectedBuyer.DealDays != null && selectedBuyer.DealDays.Count > 0)
                {
                    string daysStr = string.Join(", ", selectedBuyer.DealDays);
                    content += $"\n\n<b><color=#FFA500>Deal Days:</color></b> <color=#FFFFFF>{daysStr}</color>";
                }
                
                var pendingBuyers = Contacts.Buyers.Values
                    .Where(b => !b.IsUnlocked &&
                                b.UnlockRequirements != null &&
                                b.UnlockRequirements.Any(r => r.Name == selectedBuyer.DisplayName && r.MinRep > selectedBuyer.DealerSaveData.Reputation))
                    .ToList();
                
                if (pendingBuyers.Count > 0)
                {
                    content += "\n\n<b>Pending Unlocks:</b>\n";
                    foreach (var buyer in pendingBuyers)
                    {
                        var req = buyer.UnlockRequirements.FirstOrDefault(r => r.Name == selectedBuyer.DisplayName && r.MinRep > selectedBuyer.DealerSaveData.Reputation);
                        content += $"• {buyer.DisplayName}: Requires Rep {req?.MinRep}\n";
                    }
                }

                UIFactory.Text("DetailText", content, managementDetailPanel.transform, 18);
            }
            else if (tab == "Product")
            {
                content = selectedBuyer.GetDrugUnlockInfo();
                UIFactory.Text("ProductDetailText", content, managementDetailPanel.transform, 18);
            }
            else if (tab == "Shipping")
            {
                int currentTier = selectedBuyer.DealerSaveData.ShippingTier;
                string currentShipping = "";
                string nextShipping = "";
                double logResult = 0d;

                if (selectedBuyer.RepLogBase > 1)
                {
                    logResult = Math.Log((double)selectedBuyer.DealerSaveData.Reputation + 1, (double)selectedBuyer.RepLogBase);
                    if (logResult < 4) logResult = 0;
                    else logResult = logResult - 4;
                }
                logResult = Math.Round(logResult, 2);   //format to 2 decimal places

				if (selectedBuyer.Shippings != null && currentTier < selectedBuyer.Shippings.Count)
                {
                    var currentShip = selectedBuyer.Shippings[currentTier];
                    currentShipping = $"<b><color=#FF6347>Current Tier ({currentTier})</color></b>\n" +
                                      $"   • <i>Name:</i> <color=#FFFFFF>{currentShip.Name}</color>\n" +
                                      $"   • <i>Cost:</i> <color=#00FFFF>{currentShip.Cost}</color>\n" +
                                      $"   • <i>Unlock Rep:</i> <color=#00FF00>{currentShip.UnlockRep}</color>\n" +
                                      $"   • <i>Amounts:</i> <color=#FFFF00>{currentShip.MinAmount} - {currentShip.MaxAmount}</color>\n" +
                                      $"   • <i>Package:</i> <color=#FFFF00>{currentShip.StepAmount}</color>\n" +
                                      $"   • <i>Rep Multiple Bonus:</i> <color=#FFFF00>x {logResult}</color>\n" +
                                      $"   • <i>Deal Modifier:</i> <color=#FFA500>{string.Join(", ", currentShip.DealModifier)}</color>\n";
                }
                else
                {
                    currentShipping = "<b><color=#FF6347>Current shipping info not available.</color></b>";
                }

                if (selectedBuyer.Shippings != null && currentTier + 1 < selectedBuyer.Shippings.Count)
                {
                    var nextShip = selectedBuyer.Shippings[currentTier + 1];
                    nextShipping = $"<b><color=#FF6347>Next Tier ({currentTier + 1})</color></b>\n" +
                                   $"   • <i>Name:</i> <color=#FFFFFF>{nextShip.Name}</color>\n" +
                                   $"   • <i>Cost:</i> <color=#00FFFF>{nextShip.Cost}</color>\n" +
                                   $"   • <i>Unlock Rep:</i> <color=#00FF00>{nextShip.UnlockRep}</color>\n" +
                                   $"   • <i>Amounts:</i> <color=#FFFF00>{nextShip.MinAmount} - {nextShip.MaxAmount}</color>\n" +
                                   $"   • <i>Package:</i> <color=#FFFF00>{nextShip.StepAmount}</color>\n" +
                                   $"   • <i>Rep Multiple Bonus:</i> <color=#FFFF00>x {logResult}</color>\n" +
                                   $"   • <i>Deal Modifier:</i> <color=#FFA500>{string.Join(", ", nextShip.DealModifier)}</color>\n";
                }
                else
                {
                    nextShipping = "<b><color=#FF6347>Next Tier:</color></b> <color=#FFFFFF>Maximum tier unlocked.</color>";
                }
                content = currentShipping + "\n" + nextShipping;

                UIFactory.Text("ShippingDetailText", content, managementDetailPanel.transform, 18);

                if (selectedBuyer.Shippings != null && currentTier + 1 < selectedBuyer.Shippings.Count)
                {
                    var upgradeTuple = UIFactory.RoundedButtonWithLabel("UpgradeShippingButton", "Upgrade Shipping", managementDetailPanel.transform, new Color32(0, 123, 255, 255), 160, 42, 16, Color.white);
                    ButtonUtils.ClearListeners(upgradeTuple.Item2);
                    ButtonUtils.AddListener(upgradeTuple.Item2, () =>
                    {
                        var nextTierData = selectedBuyer.Shippings[currentTier + 1];
                        int cost = nextTierData.Cost;
                        if (Money.GetCashBalance() < cost)
                        {
                            var error = UIFactory.Text("UpgradeErrorText", "<color=#FF0000>Not enough cash (cost: " + cost + ").</color>", GetMessageParent(), 18);
                            MelonCoroutines.Start(BlinkMessage(error));
                            return;
                        }
                        if (selectedBuyer.DealerSaveData.Reputation < nextTierData.UnlockRep)
                        {
                            var error = UIFactory.Text("UpgradeErrorText", "<color=#FF0000>Not enough reputation (required: " + nextTierData.UnlockRep + ").</color>", GetMessageParent(), 18);
                            MelonCoroutines.Start(BlinkMessage(error));
                            return;
                        }
                        if (selectedBuyer.UpgradeShipping())
                        {
                            ConsoleHelper.RunCashCommand(-cost);
                            UpdateBuyerDetails("Shipping");
                        }
                        else
                        {
                            var error = UIFactory.Text("UpgradeErrorText", "<color=#FF0000>Maximum tier reached.</color>", GetMessageParent(), 18);
                            MelonCoroutines.Start(BlinkMessage(error));
                        }
                    });
                }
            }
            else if (tab == "Gifts")
            {
                // Capture the buyer at UI build time
                var capturedBuyer = selectedBuyer;

                // Show gift info above the button with clearer formatting
                UIFactory.Text("SpecialDetailText", $"<b>Special Gift</b>\nCost: ${capturedBuyer.Gift.Cost}\nReputation Gain: {capturedBuyer.Gift.Rep}", managementDetailPanel.transform, 18);

                // Use a smaller button so temporary messages appear below it instead of replacing UI
                var giftTuple = UIFactory.RoundedButtonWithLabel("GiftButton", "Give Gift", managementDetailPanel.transform, new Color32(0, 123, 255, 255), 460f, 60f, 22, Color.white);
                ButtonUtils.ClearListeners(giftTuple.Item2);
                ButtonUtils.AddListener(giftTuple.Item2, () =>
                {
                    int cost = capturedBuyer.Gift.Cost;
                    if (Money.GetCashBalance() < cost)
                    {
                        var error = UIFactory.Text("SpecialErrorText", "<color=#FF0000>Not enough cash for gift.</color>", GetMessageParent(), 18);
                        MelonCoroutines.Start(BlinkMessage(error));
                        return;
                    }

                    // Use console command for consistent cash handling and trigger game systems
                    ConsoleHelper.RunCashCommand(-cost);
                    capturedBuyer.GiveReputation(capturedBuyer.Gift.Rep);

                    var successMessage = UIFactory.Text("SpecialSuccessText", $"Gift given! Reputation increased by {capturedBuyer.Gift.Rep}.", GetMessageParent(), 18);
                    MelonCoroutines.Start(BlinkMessage(successMessage));

                    // Refresh source data and re-resolve selectedBuyer to the possibly updated instance
                    Contacts.Update();
                    if (Contacts.Buyers.TryGetValue(capturedBuyer.DisplayName, out var updated) && updated.IsInitialized)
                        selectedBuyer = updated;
                    else
                        selectedBuyer = capturedBuyer;

                    // Rebuild the UI for this tab
                    UpdateBuyerDetails("Gifts");
                });

                // Reward area: capture rewardManager and buyer
                var rewardManager = capturedBuyer.RewardManager;
                string rewardType = rewardManager.GetRewardType();

                if (capturedBuyer.Reward?.Args != null && capturedBuyer.Reward.Args.Count > 0)
                {
                    rewardType += " - " + string.Join(" ", capturedBuyer.Reward.Args) + " - Reward will be given after 10 secs";
                }
                else
                {
                    rewardType = "No reward available";
                }

                UIFactory.Text("RewardTypeText", $"Reward Type: {rewardType}", managementDetailPanel.transform, 18);
                var rewardButtonTuple = UIFactory.RoundedButtonWithLabel("RewardButton", "Claim Reward", managementDetailPanel.transform, new Color32(0, 123, 255, 255), 460f, 60f, 22, Color.white);
                ButtonUtils.ClearListeners(rewardButtonTuple.Item2);
                ButtonUtils.AddListener(rewardButtonTuple.Item2, () =>
                {
                    if (!rewardManager.isRewardAvailable)
                    {
                        var error = UIFactory.Text("RewardResultText", "<color=#FF0000>Reward not available today.</color>", GetMessageParent(), 18);
                        MelonCoroutines.Start(BlinkMessage(error));
                        return;
                    }
                    if (string.IsNullOrEmpty(rewardManager.GetRewardType()))
                    {
                        var error = UIFactory.Text("RewardResultText", "<color=#FF0000>No reward available from this contact.</color>", GetMessageParent(), 18);
                        MelonCoroutines.Start(BlinkMessage(error));
                        return;
                    }
                    if (capturedBuyer.Reward?.unlockRep > 0 && capturedBuyer.DealerSaveData.Reputation < capturedBuyer.Reward.unlockRep)
                    {
                        var error = UIFactory.Text("RewardResultText", $"<color=#FF0000>Insufficient reputation. Required: {capturedBuyer.Reward.unlockRep}</color>", GetMessageParent(), 18);
                        MelonCoroutines.Start(BlinkMessage(error));
                        return;
                    }

                    rewardManager.GiveReward();
                    var success = UIFactory.Text("RewardResultText", "<color=#00FF00>Reward will be given in 10 secs!</color>", GetMessageParent(), 18);
                    MelonCoroutines.Start(BlinkMessage(success));

                    Contacts.Update();
                    //if (Contacts.Buyers.TryGetValue(capturedBuyer.DealerId, out var updatedAfterReward) && updatedAfterReward.IsInitialized)
                    //    selectedBuyer = updatedAfterReward;
                    //else
                    //    selectedBuyer = capturedBuyer;

                    UpdateBuyerDetails("Gifts");
                });
            }
            else if (tab == "Debt")
            {
                if (selectedBuyer.Debt == null || selectedBuyer.DealerSaveData.DebtRemaining <= 0)
                {
                    UIFactory.Text("NoDebtText", "No outstanding debt with this contact.", managementDetailPanel.transform, 18);
                }
                else
                {
                    int elapsedDays = TimeManager.ElapsedDays;
                    int nextPaymentDay = (int)Math.Ceiling((elapsedDays + 1) / 7.0) * 7;
                    float weeklyPayment = selectedBuyer.Debt.DayMultiple * (float)Math.Pow(nextPaymentDay, selectedBuyer.Debt.DayExponent);
                    weeklyPayment -= selectedBuyer.DealerSaveData.DebtPaidThisWeek;
                    if (weeklyPayment < 0) weeklyPayment = 0;
                    weeklyPayment = Mathf.Min(weeklyPayment, selectedBuyer.DealerSaveData.DebtRemaining);

                    string debtInfo = $"<b><color=#FF6347>Total Debt:</color></b> <color=#FFFFFF>${selectedBuyer.DealerSaveData.DebtRemaining:N0}</color>\n" +
                                      $"<b><color=#FFA500>Next Weekly Payment:</color></b> <color=#FFFFFF>${weeklyPayment:N0}</color>\n" +
                                      $"<b><color=#FFA500>Interest Rate:</color></b> <color=#FFFFFF>{selectedBuyer.Debt.InterestRate:P1}</color>\n" +
                                      $"<b><color=#00FF00>Paid This Week:</color></b> <color=#FFFFFF>${selectedBuyer.DealerSaveData.DebtPaidThisWeek:N0}</color>";

                    UIFactory.Text("DebtInfoText", debtInfo, managementDetailPanel.transform, 18);

                    // Capture buyer for both label calculation and the click handler
                    var debtBuyer = selectedBuyer;

                    // Compute displayed payment amount based on current remaining debt (10% or $1000 min), capped at remaining debt
                    int displayedPaymentAmount = (int)Mathf.Max(1000f, debtBuyer.DealerSaveData.DebtRemaining * 0.1f);
                    displayedPaymentAmount = (int)Mathf.Floor(Mathf.Min(displayedPaymentAmount, debtBuyer.DealerSaveData.DebtRemaining));

                    var payDebtTuple = UIFactory.RoundedButtonWithLabel("PayDebtButton", $"Pay ${displayedPaymentAmount:N0}", managementDetailPanel.transform, new Color32(0, 123, 255, 255), 220f, 42f, 22, Color.white);
                    ButtonUtils.ClearListeners(payDebtTuple.Item2);

                    // Capture buyer for the closure (listener recalculates to ensure correctness at click time)
                    ButtonUtils.AddListener(payDebtTuple.Item2, () =>
                    {
                        int paymentAmount = (int)Mathf.Max(1000f, debtBuyer.DealerSaveData.DebtRemaining * 0.1f);
                        paymentAmount = (int)Mathf.Floor(Mathf.Min(paymentAmount, debtBuyer.DealerSaveData.DebtRemaining));

                        if (Money.GetCashBalance() < paymentAmount)
                        {
                            var error = UIFactory.Text("DebtErrorText", "<color=#FF0000>Not enough cash to make payment.</color>", GetMessageParent(), 18);
                            MelonCoroutines.Start(BlinkMessage(error));
                            return;
                        }

                        // Use console command to change cash so other systems react consistently
                        ConsoleHelper.RunCashCommand(-(int)paymentAmount);

                        // Apply payment to the captured buyer
                        debtBuyer.DealerSaveData.DebtRemaining = (int)(debtBuyer.DealerSaveData.DebtRemaining - paymentAmount);
                        debtBuyer.DealerSaveData.DebtPaidThisWeek = (int)(debtBuyer.DealerSaveData.DebtPaidThisWeek+paymentAmount);

                        var success = UIFactory.Text("DebtSuccessText", $"<color=#00FF00>Paid ${paymentAmount:N0}.</color>", GetMessageParent(), 18);
                        MelonCoroutines.Start(BlinkMessage(success));

                        // Refresh source data and re-resolve selectedBuyer
                        Contacts.Update();
                        if (Contacts.Buyers.TryGetValue(debtBuyer.DisplayName, out var updatedBuyer) && updatedBuyer.IsInitialized)
                            selectedBuyer = updatedBuyer;
                        else
                            selectedBuyer = debtBuyer;
                        debtBuyer.DebtManager.SendDebtMessage(paymentAmount,"payment"); // Ensure debt manager recalculates interest and next payment
                        debtBuyer.DebtManager.CheckIfPaidThisWeek(); // Check if debt is weekly paid
                        UpdateBuyerDetails("Debt");
                    });

                    // New button: fixed payment of $1,000 per press (caps to remaining debt)
                    var payThousandTuple = UIFactory.RoundedButtonWithLabel("PayThousandButton", "Pay $1,000", managementDetailPanel.transform, new Color32(0, 123, 255, 255), 220f, 42f, 22, Color.white);
                    ButtonUtils.ClearListeners(payThousandTuple.Item2);
                    ButtonUtils.AddListener(payThousandTuple.Item2, () =>
                    {
                        int fixedPayment = 1000;
                        // Cap the fixed payment to remaining debt
                        int paymentAmount = (int)Mathf.Floor(Mathf.Min(fixedPayment, debtBuyer.DealerSaveData.DebtRemaining));

                        if (paymentAmount <= 0)
                        {
                            var info = UIFactory.Text("DebtZeroText", "<color=#FFFF00>No debt remaining to pay.</color>", GetMessageParent(), 18);
                            MelonCoroutines.Start(BlinkMessage(info));
                            UpdateBuyerDetails("Debt");
                            return;
                        }

                        if (Money.GetCashBalance() < paymentAmount)
                        {
                            var error = UIFactory.Text("DebtThousandErrorText", "<color=#FF0000>Not enough cash to pay $1,000.</color>", GetMessageParent(), 18);
                            MelonCoroutines.Start(BlinkMessage(error));
                            return;
                        }

                        // Use console command to change cash so other systems react consistently
                        ConsoleHelper.RunCashCommand(-paymentAmount);

                        // Apply payment to the captured buyer
                        debtBuyer.DealerSaveData.DebtRemaining = (int)(debtBuyer.DealerSaveData.DebtRemaining - paymentAmount);
                        debtBuyer.DealerSaveData.DebtPaidThisWeek = (int)(debtBuyer.DealerSaveData.DebtPaidThisWeek + paymentAmount);

                        var success = UIFactory.Text("DebtThousandSuccessText", $"<color=#00FF00>Paid ${paymentAmount:N0}.</color>", GetMessageParent(), 18);
                        MelonCoroutines.Start(BlinkMessage(success));

                        // Refresh source data and re-resolve selectedBuyer
                        Contacts.Update();
                        if (Contacts.Buyers.TryGetValue(debtBuyer.DisplayName, out var updatedBuyer2) && updatedBuyer2.IsInitialized)
                            selectedBuyer = updatedBuyer2;
                        else
                            selectedBuyer = debtBuyer;

                        debtBuyer.DebtManager.SendDebtMessage(paymentAmount, "payment");
                        debtBuyer.DebtManager.CheckIfPaidThisWeek();
                        UpdateBuyerDetails("Debt");
                    });
                }
            }


            if (managementTabLabel != null)
                managementTabLabel.text = tab;

            // After updating details, refresh the buyer list to apply selection
            if (buyerListContainer != null) PopulateBuyerList(buyerListContainer);
        }


        private System.Collections.IEnumerator BlinkMessage(Text message)
        {
            yield return new WaitForSeconds(2f);
            if (message != null)
            {
                Object.Destroy(message.gameObject);
            }
        }
        private System.Collections.IEnumerator WaitForBuyerAndInitialize()
        {
            MelonLogger.Msg("PhoneApp-WaitForBuyerAndInitialize-Waiting for Contacts to be initialized...");
            
            // First wait for all NPCs to be registered
            while (!Contacts.IsInitialized)
            {
                yield return null;
            }
            MelonLogger.Msg("PhoneApp: Contacts.IsInitialized is true, now waiting for full processing...");

            // CRITICAL: Wait for UpdateCoroutine to complete processing (unlocks, intros, UnlockDrug calls)
            // This prevents the race condition where LoadQuests runs before UnlockedDrugs is populated
            while (!Contacts.AreContactsFullyProcessed)
            {
                yield return null;
            }

            MelonLogger.Msg("Dealers and Buyers initialized successfully.");
            LoadQuests();
        }

        private void RefreshButton()
        {
            int refreshCost = 0;
            foreach (var buyer in Contacts.Buyers.Values)
            {
                string currentDay = TimeManager.CurrentDay.ToString();
                MelonLogger.Msg($"RefreshButton(): Current Day: {currentDay}.");
                if (buyer.DealDays != null && buyer.DealDays.Contains(currentDay) && buyer.IsUnlocked)
                {
                    refreshCost += buyer.RefreshCost;
                }
            }
            if (Money.GetCashBalance() < refreshCost)
            {
                deliveryStatus.text = $"You need {refreshCost} cash to refresh the list.";
                return;
            }
            LoadQuests();
            ConsoleHelper.RunCashCommand(-refreshCost);
        }

        public static void ClearChildren(Transform parent)
        {
            if (parent == null) return;

            UIFactory.ClearChildren(parent);    //  use S1API function
			//for (int i = parent.childCount - 1; i >= 0; i--)
			//{
			//    Object.Destroy(parent.GetChild(i).gameObject);
			//}
		}
        private void LoadQuests()
        {
            quests = new List<QuestData>();
            foreach (var buyer in Contacts.Buyers.Values)
            {
                string currentDay = TimeManager.CurrentDay.ToString();
                MelonLogger.Msg($"Checking dealer {buyer.DisplayName} for quests on day {currentDay}.");
				
                // Skip buyers that aren't unlocked yet (unlock requirements not met)
                if (!buyer.IsUnlocked)
                {
                    MelonLogger.Msg($"Skipping dealer {buyer.DisplayName}: Not unlocked yet (unlock requirements not met).");
                    continue;
                }
                
				if (buyer.DealDays == null || !buyer.DealDays.Contains(currentDay) || !buyer.IsInitialized)
                {
                    MelonLogger.Msg($"Skipping dealer {buyer.DisplayName}: DealDays not set or does not include {currentDay}, or dealer not initialized: IsInitialized -> {buyer.IsInitialized}.");
					continue;
                }

                var dealerSaveData = buyer.DealerSaveData;
                MelonLogger.Msg($"Dealer {buyer.DisplayName} - ShippingTier: {dealerSaveData.ShippingTier}.");
				if (dealerSaveData.ShippingTier < 0 || dealerSaveData.ShippingTier >= buyer.Shippings.Count)
                {
                    MelonLogger.Error($"[EmpirePhoneApp] Invalid ShippingTier {dealerSaveData.ShippingTier} for dealer {buyer.DisplayName}. Shippings.Count={buyer.Shippings.Count}");
                    continue;
                }

                var drugTypes = dealerSaveData.UnlockedDrugs.Select(d => d.Type).Distinct().OrderBy(_ => UnityEngine.Random.value).ToArray();
                MelonLogger.Msg($"Dealer {buyer.DisplayName} has unlocked drugs: {string.Join(", ", drugTypes)}.");
				if (drugTypes.Any())
                {
                    MelonLogger.Msg($"Generating quest for dealer {buyer.DisplayName} with drug {drugTypes.First()}. drugTypes.Any(): {drugTypes.Any()}");
					GenerateQuest(buyer, drugTypes.First());
                }
            }
            MelonLogger.Msg($"✅ Total quests loaded: {quests.Count}");
            RefreshQuestList();
        }

        int RoundToHalfMSD(int value)
        {
            if (value == 0) return 0;
            int digits = (int)Math.Floor(Math.Log10(value)) + 1;
            int keep = (digits + 1) / 2;
            int roundFactor = (int)Math.Pow(10, digits - keep);
            return ((value + roundFactor - 1) / roundFactor) * roundFactor;
        }

		private void GenerateQuest(EmpireNPC buyer, string drugType)
		{
			// --- Validate drug availability -----------------------------------------
			var unlockedDrug = buyer.DealerSaveData.UnlockedDrugs
				.FirstOrDefault(d => d.Type == drugType);

            if (unlockedDrug == null)
            {
                MelonLogger.Msg($"Unlocked drug '{drugType}' not found for dealer '{buyer.DisplayName}'.");
                return;
            }

			if (unlockedDrug.Qualities.Count == 0)
			{
				MelonLogger.Error(
					$"[GenerateQuest] No qualities unlocked for '{unlockedDrug.Type}' on dealer '{buyer.DisplayName}'.");
				return;
			}

			// --- Builder -------------------------------------------------------------
			var builder = new QuestBuilder(buyer, unlockedDrug);

			// --- Amount --------------------------------------------------------------
			int amount = builder.CalculateAmount();

			// --- Quality -------------------------------------------------------------
			var (qualityName, qualityMult, qualityColor) = builder.SelectQuality();

			// --- Random numbers ------------------------------------------------------
			float randomNum1 = UnityEngine.Random.Range(EmpireConfig.RandomRanges[0], EmpireConfig.RandomRanges[1]);
			float randomNum2 = UnityEngine.Random.Range(EmpireConfig.RandomRanges[2], EmpireConfig.RandomRanges[3]);
			float randomNum3 = UnityEngine.Random.Range(EmpireConfig.RandomRanges[4], EmpireConfig.RandomRanges[5]);
			float randomNum4 = UnityEngine.Random.Range(EmpireConfig.RandomRanges[6], EmpireConfig.RandomRanges[7]);

			// --- Effects -------------------------------------------------------------
			var (necessary, necessaryMult, optional, optionalMult, temp11, temp21)
				= builder.SelectEffects(randomNum1);

			// --- Deal time -----------------------------------------------------------
			int idx = UnityEngine.Random.Range(0, buyer.Deals.Count);
			var shipping = buyer.Shippings[buyer.DealerSaveData.ShippingTier];

			int dealTime = (int)(buyer.Deals[idx][0] * shipping.DealModifier[0]);
			float dealTimeMult = (float)(buyer.Deals[idx][1] * shipping.DealModifier[1]);

			// --- Aggregate multipliers ----------------------------------------------
			float aggMin = (1 + qualityMult) * temp11 * dealTimeMult * randomNum4;
			float aggMax = (1 + qualityMult) * temp21 * dealTimeMult * randomNum4;

			// --- Effect description --------------------------------------------------
			string effectDesc = BuildEffectDescription(necessary, optional);

			// --- Dialogue index ------------------------------------------------------
			int dialogueIndex = RandomUtils.RangeInt(0, buyer.EmpireDialogue.DealStart.Count);

			// --- Create quest --------------------------------------------------------
			var quest = new QuestData
			{
				Title = $"{buyer.DisplayName} wants {drugType} delivered.",
				Task =
					$"Deliver <color=#FF0004>{amount}x</color> " +
					$"<color={qualityColor}>{qualityName}</color> " +
					$"<color=#FF0004>{drugType}</color> " +
					$"with effects: {effectDesc}",

				ProductID = drugType,
				AmountRequired = (uint)amount,
				DealerName = buyer.DisplayName,
				QuestImage = buyer.Image,

				BaseDollar = RoundToHalfMSD((int)(unlockedDrug.BaseDollar * amount / randomNum4)),
				BaseRep = RoundToHalfMSD((int)(unlockedDrug.BaseRep * randomNum2)),
				BaseXp = RoundToHalfMSD((int)(unlockedDrug.BaseXp * randomNum3)),

				RepMult = unlockedDrug.RepMult * randomNum2,
				XpMult = unlockedDrug.XpMult * randomNum3,

				DollarMultiplierMin = (float)Math.Round(aggMin, 2),
				DollarMultiplierMax = (float)Math.Round(aggMax, 2),

				DealTime = dealTime,
				DealTimeMult = dealTimeMult * randomNum4,

				Penalties = new List<int>
		        {
			        RoundToHalfMSD((int)(buyer.Deals[idx][2] * shipping.DealModifier[2] * randomNum1)),
			        RoundToHalfMSD((int)(buyer.Deals[idx][3] * shipping.DealModifier[3] * randomNum2))
		        },

				Quality = qualityName,
				QualityMult = qualityMult,

				NecessaryEffects = necessary,
				NecessaryEffectMult = necessaryMult,
				OptionalEffects = optional,
				OptionalEffectMult = optionalMult,

				Index = Index++,
				DialogueIndex = dialogueIndex
			};

			quests.Add(quest);
		}

		private string BuildEffectDescription(List<string> necessary, List<string> optional)
		{
			var sb = new System.Text.StringBuilder();

			// Necessary effects = red
			if (necessary.Count > 0)
			{
				var colored = necessary.Select(e =>
					$"<color=#FF0004>{e}</color>");
				sb.Append($"Required: {string.Join(", ", colored)}");
			}

			// Optional effects = cyan
			if (optional.Count > 0)
			{
				if (sb.Length > 0)
					sb.Append("; ");

				var colored = optional.Select(e =>
					$"<color=#00FFFF>{e}</color>");
				sb.Append($"Optional: {string.Join(", ", colored)}");
			}

			return sb.Length == 0 ? "none" : sb.ToString();
		}

		private void CancelCurrentQuest(QuestData quest)
        {
            var active = QuestDelivery.Active;
            if (active == null) return;
            try
            {
                active.ForceCancel();
                deliveryStatus.text = "🚫 Delivery canceled.";
                ButtonUtils.Disable(cancelButton, cancelLabel, "Canceled");
                ButtonUtils.Enable(acceptButton, acceptLabel, "Accept Delivery");
                RefreshQuestList();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"❌ CancelCurrentQuest() exception: {ex}");
            }
        }

        private void RefreshQuestList()
        {
            ClearChildren(questListContainer);
            Index = 0;
            foreach (var quest in quests)
            {
                if (quest == null) continue;

                var row = UIFactory.CreateQuestRow(quest.DealerName, questListContainer, out var iconPanel, out var textPanel);
                quest.Index = row.transform.GetSiblingIndex();

                var image = EmpireResourceLoader.LoadEmbeddedIcon(quest.QuestImage ?? "EmpireIcon_quest.png");
				//var image = ImageUtils.LoadImage(quest.QuestImage != null ? Path.Combine(MelonEnvironment.ModsDirectory, "Empire", quest.QuestImage) : Path.Combine(MelonEnvironment.ModsDirectory, "Empire", "EmpireIcon_quest.png"));
                UIFactory.SetIcon(image, iconPanel.transform);
                var questIcon = iconPanel.transform.GetComponentInChildren<Image>();
                if (questIcon != null) questIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(128, 128);

                // Apply highlight if this quest's buyer is the selected one
                if (selectedBuyer != null && quest.DealerName == selectedBuyer.DisplayName)
                {
                    row.GetComponent<Image>().color = Color.black;
                }

                ButtonUtils.AddListener(row.GetComponent<Button>(), () => OnSelectQuest(quest));
                UIFactory.CreateTextBlock(textPanel.transform, quest.Title, quest.Task, false);
            }
        }

        private void PopulateBuyerList(RectTransform container)
        {
            ClearChildren(container);
            var buyers = Contacts.Buyers.Values.Where(b => b.IsInitialized && b.IsUnlocked).ToList();

            // Find the default color from a temporary object to ensure consistency
            GameObject tempRow = UIFactory.CreateQuestRow("temp", container, out _, out _);
            Color defaultColor = tempRow.GetComponent<Image>().color;
            Object.Destroy(tempRow);


            foreach (var buyer in buyers)
            {
                var row = UIFactory.CreateQuestRow(buyer.DisplayName, container, out var iconPanel, out var textPanel);
                var rowImage = row.GetComponent<Image>();

                // Set row background color based on selection
                rowImage.color = (selectedBuyer != null && selectedBuyer.DisplayName == buyer.DisplayName) ? Color.black : defaultColor;

                var image = EmpireResourceLoader.LoadEmbeddedIcon(buyer.Image ?? "fallback-dealer.png");
				//var image = ImageUtils.LoadImage(Path.Combine(MelonEnvironment.ModsDirectory, "Empire", buyer.Image ?? "EmpireIcon_quest.png"));
                UIFactory.SetIcon(image, iconPanel.transform);

                ButtonUtils.AddListener(row.GetComponent<Button>(), () =>
                {
                    selectedBuyer = buyer;
                    UpdateBuyerDetails(managementTabLabel != null ? managementTabLabel.text : "Reputation");
                    // The list is repopulated inside UpdateBuyerDetails, which will handle the color update.
                });

                UIFactory.CreateTextBlock(textPanel.transform, buyer.DisplayName, "", false);
            }
        }

        private void OnSelectQuest(QuestData quest)
        {
            selectedBuyer = Contacts.GetBuyer(quest.DealerName);
            RefreshQuestList(); // Redraw list to show new selection highlight

            var Buyer = Contacts.GetBuyer(quest.DealerName);
            var dialogue = Buyer.SendCustomMessage(DialogueType.DealStart, quest.ProductID, (int)quest.AmountRequired, quest.Quality, quest.NecessaryEffects, quest.OptionalEffects, 0, true, quest.DialogueIndex);
            questTitle.text = quest.Title;
            questTask.text = $"{dialogue}";
            
            
            questReward.text =
                $"<b><color=#FFD700>Rewards:</color></b> <color=#00FF00>${quest.BaseDollar} / {quest.BaseDollar / quest.AmountRequired} per piece</color> + <i>Price x</i> (<color=#00FFFF>{quest.DollarMultiplierMin}</color> - <color=#00FFFF>{quest.DollarMultiplierMax}</color>)\n" +
                $"<b><color=#FFD700>Reputation:</color></b> <color=#00FF00>{quest.BaseRep}</color> + Rewards x <color=#00FFFF>{Math.Round(quest.RepMult, 4)}</color>\n" +
                $"<b><color=#FFD700>XP:</color></b> <color=#00FF00>{quest.BaseXp}</color> + Rewards x <color=#00FFFF>{Math.Round(quest.XpMult, 4)}</color>\n\n" +
                $"<b><color=#FF6347>Deal Expiry:</color></b> <color=#FFA500>{quest.DealTime} Days</color>\n" +
                $"<b><color=#FF6347>Failure Penalties:</color></b> <color=#FF0000>${quest.Penalties[0]}</color> + <color=#FF4500>{quest.Penalties[1]} Rep</color>";

            deliveryStatus.text = "";
            if (!QuestDelivery.QuestActive)
            {
                ButtonUtils.Enable(acceptButton, acceptLabel, "Accept Delivery");
                ButtonUtils.ClearListeners(acceptButton);
                ButtonUtils.AddListener(acceptButton, () => AcceptQuest(quest));
            }
            else
            {
                ButtonUtils.Disable(acceptButton, acceptLabel, "In progress");
                ButtonUtils.ClearListeners(acceptButton);
            }

            ButtonUtils.Enable(cancelButton, cancelLabel, "Cancel current delivery");
            ButtonUtils.ClearListeners(cancelButton);
            ButtonUtils.AddListener(cancelButton, () => CancelCurrentQuest(quest));
        }

        public void OnQuestComplete()
        {
            ButtonUtils.ClearListeners(cancelButton);
            ButtonUtils.Disable(cancelButton, cancelLabel, "No quest active");
        }

        private void AcceptQuest(QuestData quest)
        {
            if (QuestDelivery.QuestActive)
            {
                deliveryStatus.text = "⚠️ Finish your current job first!";
                ButtonUtils.Disable(acceptButton, acceptLabel, "In Progress");
                return;
            }
            var Buyer = Contacts.GetBuyer(quest.DealerName);
            Buyer?.SendCustomMessage(DialogueType.Accept, quest.ProductID, (int)quest.AmountRequired, quest.Quality, quest.NecessaryEffects, quest.OptionalEffects);
            deliveryStatus.text = "📦 Delivery started!";
            ButtonUtils.Disable(acceptButton, acceptLabel, "In Progress");

            q = S1API.Quests.QuestManager.CreateQuest<QuestDelivery>();
            if (q is QuestDelivery delivery)
            {
                delivery.Data.ProductID = quest.ProductID;
                delivery.Data.RequiredAmount = quest.AmountRequired;
                delivery.Data.DealerName = quest.DealerName;
                delivery.Data.QuestImage = quest.QuestImage;
                delivery.Data.Reward = quest.BaseDollar;
                delivery.Data.RepReward = quest.BaseRep;
                delivery.Data.XpReward = quest.BaseXp;
                delivery.Data.RepMult = quest.RepMult;
                delivery.Data.XpMult = quest.XpMult;
                delivery.Data.Task = quest.Task;
                delivery.Data.DealTime = quest.DealTime;
                delivery.Data.DealTimeMult = quest.DealTimeMult;
                delivery.Data.Penalties = quest.Penalties;
                delivery.Data.Quality = quest.Quality;
                delivery.Data.QualityMult = quest.QualityMult;
                delivery.Data.NecessaryEffects = quest.NecessaryEffects;
                delivery.Data.OptionalEffects = quest.OptionalEffects;
                delivery.Data.NecessaryEffectMult = quest.NecessaryEffectMult;
                delivery.Data.OptionalEffectMult = quest.OptionalEffectMult;
                QuestDelivery.Active = delivery;
            }
            else
            {
                MelonLogger.Error("❌ Failed to create QuestDelivery instance - Accept Quest.");
                return;
            }

            quests.Remove(quest);
            RefreshQuestList();
            acceptButton.interactable = false;
            ButtonUtils.Enable(cancelButton, cancelLabel, "Cancel Current Delivery");
        }
    }
}