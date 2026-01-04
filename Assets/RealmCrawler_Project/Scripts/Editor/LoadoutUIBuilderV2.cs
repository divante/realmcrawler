using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using TMPro;
using RealmCrawler.UI;
using RealmCrawler.Core;
using RealmCrawler.Equipment;

namespace RealmCrawler.Editor
{
    public class LoadoutUIBuilderV2 : EditorWindow
    {
        private const string BUILDER_VERSION = "V2.0_ListLayout";
        
        [MenuItem("RealmCrawler/Build Loadout UI V2")]
        public static void ShowWindow()
        {
            GetWindow<LoadoutUIBuilderV2>("Loadout UI Builder V2");
        }

        private void OnGUI()
        {
            GUILayout.Label("Loadout UI Builder V2 - LIST LAYOUT", EditorStyles.boldLabel);
            GUILayout.Space(10);

            EditorGUILayout.HelpBox(
                "VERSION 2.0 - VERTICAL LIST LAYOUT\n\n" +
                "This will create the complete loadout UI with VERTICAL LIST (not grid).\n\n" +
                "Make sure you have Scene_Loadout open!\n\n" +
                "This will:\n" +
                "• Create Canvas with all UI elements\n" +
                "• Use VERTICAL LIST for equipment items\n" +
                "• Create list item prefab (Icon | Name | Cost)\n" +
                "• Wire up all script references",
                MessageType.Info);

            GUILayout.Space(10);

            if (GUILayout.Button("Build Complete UI", GUILayout.Height(40)))
            {
                if (EditorUtility.DisplayDialog(
                    "Build Loadout UI V2",
                    "This will create the complete loadout UI with VERTICAL LIST layout. Continue?",
                    "Yes, Build It!",
                    "Cancel"))
                {
                    BuildCompleteUI();
                }
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Clean Up Existing UI", GUILayout.Height(30)))
            {
                CleanUpExistingUI();
            }
        }

        private static void CleanUpExistingUI()
        {
            GameObject existingCanvas = GameObject.Find("LoadoutCanvas");
            if (existingCanvas != null)
            {
                DestroyImmediate(existingCanvas);
                Debug.Log("Cleaned up existing LoadoutCanvas");
            }
            else
            {
                Debug.Log("No existing LoadoutCanvas found");
            }
        }

        private static void BuildCompleteUI()
        {
            Debug.Log($"=== LOADOUT UI BUILDER {BUILDER_VERSION} ===");
            Debug.Log("Building UI with VERTICAL LIST layout (NOT GRID!)");
            
            CleanUpExistingUI();
            EnsureGameManager();

            GameObject canvasObj = CreateCanvas();
            
            GameObject topBar = CreateTopBar(canvasObj.transform);
            GameObject equipmentPanel = CreateEquipmentPanel(canvasObj.transform);
            GameObject statsPanel = CreateStatsPanel(canvasObj.transform);
            GameObject cantripPanel = CreateCantripPanel(canvasObj.transform);
            GameObject detailPopup = CreateDetailPopup(canvasObj.transform);
            GameObject cantripTooltip = CreateCantripTooltip(canvasObj.transform);
            GameObject backButton = CreateBackButton(canvasObj.transform);
            GameObject playButton = CreatePlayButton(canvasObj.transform);

            GameObject equipListItemPrefab = CreateEquipmentListItemPrefab();

            GameObject uiManager = new GameObject("UIManager");
            uiManager.transform.SetParent(canvasObj.transform, false);

            WireUpScripts(uiManager, topBar, equipmentPanel, statsPanel, cantripPanel, 
                         detailPopup, cantripTooltip, backButton, playButton, equipListItemPrefab);

            Debug.Log("=== LOADOUT UI BUILD COMPLETE (VERTICAL LIST) ===");
            Selection.activeGameObject = canvasObj;
        }

        private static void EnsureGameManager()
        {
            GameManager existingGM = FindFirstObjectByType<GameManager>();
            
            if (existingGM != null)
            {
                Debug.Log("GameManager already exists in scene");
                
                string databasePath = "Assets/RealmCrawler_Project/ScriptableObjects/Core/EquipmentDatabase.asset";
                EquipmentDatabase database = AssetDatabase.LoadAssetAtPath<EquipmentDatabase>(databasePath);
                
                if (database != null && existingGM.EquipmentDB == null)
                {
                    SerializedObject so = new SerializedObject(existingGM);
                    so.FindProperty("equipmentDB").objectReferenceValue = database;
                    so.ApplyModifiedProperties();
                    Debug.Log("Assigned EquipmentDatabase to existing GameManager");
                }
                
                return;
            }

            GameObject gmObj = new GameObject("GameManager");
            GameManager gm = gmObj.AddComponent<GameManager>();

            string databasePath2 = "Assets/RealmCrawler_Project/ScriptableObjects/Core/EquipmentDatabase.asset";
            EquipmentDatabase database2 = AssetDatabase.LoadAssetAtPath<EquipmentDatabase>(databasePath2);
            
            if (database2 != null)
            {
                SerializedObject so = new SerializedObject(gm);
                so.FindProperty("equipmentDB").objectReferenceValue = database2;
                so.ApplyModifiedProperties();
            }

            gm.ResetToDefaultLoadout();
            Debug.Log("Created GameManager and assigned EquipmentDatabase");
        }

        private static GameObject CreateCanvas()
        {
            GameObject canvasObj = new GameObject("LoadoutCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();

            GameObject eventSystem = GameObject.Find("EventSystem");
            if (eventSystem == null)
            {
                eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
            }

            return canvasObj;
        }

        private static GameObject CreateTopBar(Transform parent)
        {
            GameObject panel = CreateUIPanel("TopBar", parent);
            RectTransform rt = panel.GetComponent<RectTransform>();
            SetAnchor(panel, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1));
            rt.anchoredPosition = new Vector2(0, -25);
            rt.sizeDelta = new Vector2(800, 50);

            SetPanelColor(panel, new Color(0, 0, 0, 0.7f));

            GameObject loadoutCost = CreateTextMeshPro("LoadoutCostText", panel.transform);
            ConfigureText(loadoutCost, "Loadout Cost: 0 souls", 20, TextAlignmentOptions.Left);
            SetAnchor(loadoutCost, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0.5f));
            loadoutCost.GetComponent<RectTransform>().anchoredPosition = new Vector2(20, 0);
            loadoutCost.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 40);

            GameObject currentSouls = CreateTextMeshPro("CurrentSoulsText", panel.transform);
            ConfigureText(currentSouls, "Current Souls: 0", 20, TextAlignmentOptions.Right);
            SetAnchor(currentSouls, new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(1, 0.5f));
            currentSouls.GetComponent<RectTransform>().anchoredPosition = new Vector2(-20, 0);
            currentSouls.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 40);

            return panel;
        }

        private static GameObject CreateEquipmentPanel(Transform parent)
        {
            GameObject panel = CreateUIPanel("EquipmentPanel", parent);
            RectTransform rt = panel.GetComponent<RectTransform>();
            SetAnchor(panel, new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1));
            rt.anchoredPosition = new Vector2(20, -80);
            rt.sizeDelta = new Vector2(400, 700);

            SetPanelColor(panel, new Color(0, 0, 0, 0.7f));

            GameObject label = CreateTextMeshPro("EquipmentLabel", panel.transform);
            ConfigureText(label, "EQUIPMENT", 28, TextAlignmentOptions.Center);
            label.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
            label.GetComponent<TextMeshProUGUI>().color = new Color(1f, 0.84f, 0f);
            SetAnchor(label, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1));
            label.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -20);
            label.GetComponent<RectTransform>().sizeDelta = new Vector2(360, 40);

            GameObject tabContainer = new GameObject("TabContainer");
            tabContainer.transform.SetParent(panel.transform, false);
            RectTransform tabRT = tabContainer.AddComponent<RectTransform>();
            SetAnchor(tabContainer, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1));
            tabRT.anchoredPosition = new Vector2(0, -70);
            tabRT.sizeDelta = new Vector2(360, 60);

            string[] tabs = { "H", "C", "B", "R", "W" };
            float[] positions = { -140, -70, 0, 70, 140 };
            string[] names = { "HatTabButton", "CloakTabButton", "BootsTabButton", "ReliquaryTabButton", "WeaponTabButton" };

            for (int i = 0; i < tabs.Length; i++)
            {
                CreateTabButton(tabContainer.transform, names[i], tabs[i], positions[i]);
            }

            GameObject scrollView = CreateScrollViewWithVerticalList("EquipmentListScrollView", panel.transform);
            RectTransform scrollRT = scrollView.GetComponent<RectTransform>();
            SetAnchor(scrollView, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1));
            scrollRT.anchoredPosition = new Vector2(0, -150);
            scrollRT.sizeDelta = new Vector2(360, 520);

            return panel;
        }

        private static void CreateTabButton(Transform parent, string name, string label, float posX)
        {
            GameObject button = new GameObject(name);
            button.transform.SetParent(parent, false);

            RectTransform rt = button.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(60, 60);
            rt.anchoredPosition = new Vector2(posX, 0);

            Image img = button.AddComponent<Image>();
            img.color = Color.white;

            Button btn = button.AddComponent<Button>();

            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(button.transform, false);
            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
            text.text = label;
            text.fontSize = 32;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.black;

            RectTransform textRT = textObj.GetComponent<RectTransform>();
            textRT.anchorMin = Vector2.zero;
            textRT.anchorMax = Vector2.one;
            textRT.sizeDelta = Vector2.zero;
        }

        private static GameObject CreateScrollViewWithVerticalList(string name, Transform parent)
        {
            Debug.Log($"Creating {name} with VERTICAL LAYOUT GROUP");
            
            GameObject scrollView = new GameObject(name);
            scrollView.transform.SetParent(parent, false);

            RectTransform rt = scrollView.AddComponent<RectTransform>();
            Image img = scrollView.AddComponent<Image>();
            img.color = new Color(0.1f, 0.1f, 0.1f, 0.5f);
            ScrollRect scroll = scrollView.AddComponent<ScrollRect>();

            GameObject viewport = new GameObject("Viewport");
            viewport.transform.SetParent(scrollView.transform, false);
            RectTransform vpRT = viewport.AddComponent<RectTransform>();
            vpRT.anchorMin = Vector2.zero;
            vpRT.anchorMax = Vector2.one;
            vpRT.sizeDelta = Vector2.zero;
            Image vpImg = viewport.AddComponent<Image>();
            vpImg.color = Color.clear;
            viewport.AddComponent<Mask>().showMaskGraphic = false;

            GameObject content = new GameObject("Content");
            content.transform.SetParent(viewport.transform, false);
            RectTransform contentRT = content.AddComponent<RectTransform>();
            contentRT.anchorMin = new Vector2(0, 1);
            contentRT.anchorMax = new Vector2(1, 1);
            contentRT.pivot = new Vector2(0.5f, 1);
            contentRT.sizeDelta = new Vector2(0, 0);

            VerticalLayoutGroup verticalLayout = content.AddComponent<VerticalLayoutGroup>();
            verticalLayout.spacing = 5f;
            verticalLayout.childAlignment = TextAnchor.UpperCenter;
            verticalLayout.childForceExpandWidth = true;
            verticalLayout.childForceExpandHeight = false;
            verticalLayout.childControlWidth = true;
            verticalLayout.childControlHeight = false;
            verticalLayout.padding = new RectOffset(5, 5, 5, 5);
            
            Debug.Log("✓ Content created with VERTICAL LAYOUT GROUP (spacing: 5, expand width: true, expand height: false)");

            ContentSizeFitter fitter = content.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scroll.content = contentRT;
            scroll.viewport = vpRT;
            scroll.horizontal = false;
            scroll.vertical = true;

            GameObject scrollbar = new GameObject("Scrollbar Vertical");
            scrollbar.transform.SetParent(scrollView.transform, false);
            RectTransform scrollbarRT = scrollbar.AddComponent<RectTransform>();
            scrollbarRT.anchorMin = new Vector2(1, 0);
            scrollbarRT.anchorMax = new Vector2(1, 1);
            scrollbarRT.pivot = new Vector2(1, 1);
            scrollbarRT.sizeDelta = new Vector2(20, 0);
            scrollbarRT.anchoredPosition = Vector2.zero;

            Image scrollbarBG = scrollbar.AddComponent<Image>();
            scrollbarBG.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);

            Scrollbar scrollbarComp = scrollbar.AddComponent<Scrollbar>();
            scrollbarComp.direction = Scrollbar.Direction.BottomToTop;

            GameObject slidingArea = new GameObject("Sliding Area");
            slidingArea.transform.SetParent(scrollbar.transform, false);
            RectTransform slidingRT = slidingArea.AddComponent<RectTransform>();
            slidingRT.anchorMin = Vector2.zero;
            slidingRT.anchorMax = Vector2.one;
            slidingRT.sizeDelta = new Vector2(-20, -20);

            GameObject handle = new GameObject("Handle");
            handle.transform.SetParent(slidingArea.transform, false);
            RectTransform handleRT = handle.AddComponent<RectTransform>();
            handleRT.anchorMin = Vector2.zero;
            handleRT.anchorMax = Vector2.one;
            handleRT.sizeDelta = new Vector2(20, 20);

            Image handleImg = handle.AddComponent<Image>();
            handleImg.color = new Color(0.5f, 0.5f, 0.5f, 1f);

            scrollbarComp.targetGraphic = handleImg;
            scrollbarComp.handleRect = handleRT;

            scroll.verticalScrollbar = scrollbarComp;
            scroll.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;

            return scrollView;
        }

        private static GameObject CreateStatsPanel(Transform parent)
        {
            GameObject panel = CreateUIPanel("StatsPanel", parent);
            RectTransform rt = panel.GetComponent<RectTransform>();
            SetAnchor(panel, new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1));
            rt.anchoredPosition = new Vector2(-20, -80);
            rt.sizeDelta = new Vector2(300, 200);

            SetPanelColor(panel, new Color(0, 0, 0, 0.7f));

            GameObject label = CreateTextMeshPro("StatsLabel", panel.transform);
            ConfigureText(label, "STATS", 24, TextAlignmentOptions.Center);
            label.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
            label.GetComponent<TextMeshProUGUI>().color = new Color(1f, 0.84f, 0f);
            SetAnchor(label, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1));
            label.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -20);
            label.GetComponent<RectTransform>().sizeDelta = new Vector2(260, 30);

            string[] statNames = { "Health", "Mana", "Speed", "XP Radius" };
            string[] statFieldNames = { "HealthStatText", "ManaStatText", "SpeedStatText", "XpRadiusStatText" };
            float startY = -60;

            for (int i = 0; i < statNames.Length; i++)
            {
                GameObject statText = CreateTextMeshPro(statFieldNames[i], panel.transform);
                ConfigureText(statText, $"{statNames[i]}: 0", 18, TextAlignmentOptions.Left);
                SetAnchor(statText, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1));
                statText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, startY - (i * 30));
                statText.GetComponent<RectTransform>().sizeDelta = new Vector2(260, 25);
            }

            return panel;
        }

        private static GameObject CreateCantripPanel(Transform parent)
        {
            GameObject panel = CreateUIPanel("CantripDisplayPanel", parent);
            RectTransform rt = panel.GetComponent<RectTransform>();
            SetAnchor(panel, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0));
            rt.anchoredPosition = new Vector2(0, 100);
            rt.sizeDelta = new Vector2(600, 80);

            SetPanelColor(panel, new Color(0, 0, 0, 0.7f));

            CreateCantripHoverArea(panel.transform, "PrimaryHoverArea", -150, "LEFT CLICK", "PrimaryCantripIcon", "PrimaryLabel");
            CreateCantripHoverArea(panel.transform, "SecondaryHoverArea", 150, "RIGHT CLICK", "SecondaryCantripIcon", "SecondaryLabel");

            return panel;
        }

        private static void CreateCantripHoverArea(Transform parent, string name, float xPos, string labelText, string iconName, string labelName)
        {
            GameObject hoverArea = new GameObject(name);
            hoverArea.transform.SetParent(parent, false);
            RectTransform hoverRT = hoverArea.AddComponent<RectTransform>();
            hoverRT.sizeDelta = new Vector2(250, 70);
            hoverRT.anchoredPosition = new Vector2(xPos, 0);

            GameObject icon = new GameObject(iconName);
            icon.transform.SetParent(hoverArea.transform, false);
            RectTransform iconRT = icon.AddComponent<RectTransform>();
            iconRT.sizeDelta = new Vector2(60, 60);
            iconRT.anchoredPosition = new Vector2(-80, 0);
            Image iconImg = icon.AddComponent<Image>();
            iconImg.color = new Color(1, 1, 1, 0.3f);

            GameObject label = CreateTextMeshPro(labelName, hoverArea.transform);
            ConfigureText(label, labelText, 16, TextAlignmentOptions.Left);
            label.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 60);
            label.GetComponent<RectTransform>().anchoredPosition = new Vector2(10, 0);

            EventTrigger trigger = hoverArea.AddComponent<EventTrigger>();
        }

        private static GameObject CreateDetailPopup(Transform parent)
        {
            GameObject popup = CreateUIPanel("EquipmentDetailPopup", parent);
            RectTransform rt = popup.GetComponent<RectTransform>();
            SetAnchor(popup, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            rt.sizeDelta = new Vector2(400, 500);

            SetPanelColor(popup, new Color(0, 0, 0, 0.9f));
            popup.SetActive(false);

            GameObject icon = new GameObject("EquipmentIconImage");
            icon.transform.SetParent(popup.transform, false);
            RectTransform iconRT = icon.AddComponent<RectTransform>();
            iconRT.sizeDelta = new Vector2(150, 150);
            SetAnchor(icon, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1));
            iconRT.anchoredPosition = new Vector2(0, -100);
            Image iconImg = icon.AddComponent<Image>();
            iconImg.color = Color.white;

            GameObject name = CreateTextMeshPro("EquipmentNameText", popup.transform);
            ConfigureText(name, "Equipment Name", 24, TextAlignmentOptions.Center);
            name.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
            SetAnchor(name, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1));
            name.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -190);
            name.GetComponent<RectTransform>().sizeDelta = new Vector2(350, 30);

            GameObject stats = CreateTextMeshPro("StatsText", popup.transform);
            ConfigureText(stats, "Stats: +10 Health", 18, TextAlignmentOptions.Left);
            SetAnchor(stats, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1));
            stats.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -240);
            stats.GetComponent<RectTransform>().sizeDelta = new Vector2(350, 50);

            GameObject buff = CreateTextMeshPro("BuffText", popup.transform);
            ConfigureText(buff, "Buff: Fire +20%", 18, TextAlignmentOptions.Left);
            SetAnchor(buff, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1));
            buff.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -300);
            buff.GetComponent<RectTransform>().sizeDelta = new Vector2(350, 50);

            GameObject cost = CreateTextMeshPro("CostText", popup.transform);
            ConfigureText(cost, "Cost: 100 souls", 20, TextAlignmentOptions.Center);
            cost.GetComponent<TextMeshProUGUI>().color = new Color(1f, 0.84f, 0f);
            SetAnchor(cost, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1));
            cost.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -360);
            cost.GetComponent<RectTransform>().sizeDelta = new Vector2(350, 30);

            GameObject purchaseBtn = CreateButton("PurchaseButton", popup.transform, "PURCHASE", new Vector2(0.3f, 0), new Vector2(-60, 40), new Vector2(150, 50));
            GameObject backBtn = CreateButton("BackButton", popup.transform, "BACK", new Vector2(0.7f, 0), new Vector2(60, 40), new Vector2(150, 50));

            return popup;
        }

        private static GameObject CreateCantripTooltip(Transform parent)
        {
            GameObject tooltip = CreateUIPanel("CantripTooltip", parent);
            RectTransform rt = tooltip.GetComponent<RectTransform>();
            SetAnchor(tooltip, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0));
            rt.anchoredPosition = new Vector2(0, 200);
            rt.sizeDelta = new Vector2(300, 150);

            SetPanelColor(tooltip, new Color(0, 0, 0, 0.9f));
            tooltip.SetActive(false);

            GameObject title = CreateTextMeshPro("TooltipTitleText", tooltip.transform);
            ConfigureText(title, "Cantrip Name", 20, TextAlignmentOptions.Center);
            title.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
            SetAnchor(title, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1));
            title.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -20);
            title.GetComponent<RectTransform>().sizeDelta = new Vector2(280, 30);

            GameObject desc = CreateTextMeshPro("TooltipDescriptionText", tooltip.transform);
            ConfigureText(desc, "Description", 14, TextAlignmentOptions.Left);
            SetAnchor(desc, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1));
            desc.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -60);
            desc.GetComponent<RectTransform>().sizeDelta = new Vector2(280, 40);

            GameObject stats = CreateTextMeshPro("TooltipStatsText", tooltip.transform);
            ConfigureText(stats, "Stats", 14, TextAlignmentOptions.Left);
            SetAnchor(stats, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1));
            stats.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -110);
            stats.GetComponent<RectTransform>().sizeDelta = new Vector2(280, 40);

            return tooltip;
        }

        private static GameObject CreateBackButton(Transform parent)
        {
            return CreateButton("BackButton", parent, "BACK", new Vector2(1, 0), new Vector2(-20, -90), new Vector2(150, 50));
        }

        private static GameObject CreatePlayButton(Transform parent)
        {
            return CreateButton("PlayButton", parent, "PLAY", new Vector2(1, 0), new Vector2(-20, 20), new Vector2(150, 50));
        }

        private static GameObject CreateEquipmentListItemPrefab()
        {
            Debug.Log("Creating NEW Equipment List Item Prefab with LayoutElement...");
            
            GameObject listItem = new GameObject("EquipmentListItem");
            RectTransform rt = listItem.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(400, 60);
            
            Image bg = listItem.AddComponent<Image>();
            bg.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            
            Button button = listItem.AddComponent<Button>();
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1f, 0.9f, 0.6f);
            colors.pressedColor = new Color(0.8f, 0.7f, 0.4f);
            button.colors = colors;
            
            LayoutElement layoutElem = listItem.AddComponent<LayoutElement>();
            layoutElem.minHeight = 60;
            layoutElem.preferredHeight = 60;
            Debug.Log("✓ Added LayoutElement with height 60");
            
            HorizontalLayoutGroup layoutGroup = listItem.AddComponent<HorizontalLayoutGroup>();
            layoutGroup.childAlignment = TextAnchor.MiddleLeft;
            layoutGroup.spacing = 10f;
            layoutGroup.padding = new RectOffset(10, 10, 5, 5);
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childForceExpandHeight = true;
            layoutGroup.childControlWidth = false;
            layoutGroup.childControlHeight = false;
            
            GameObject icon = new GameObject("IconImage");
            icon.transform.SetParent(listItem.transform, false);
            Image iconImage = icon.AddComponent<Image>();
            iconImage.color = Color.white;
            RectTransform iconRt = icon.GetComponent<RectTransform>();
            iconRt.sizeDelta = new Vector2(50, 50);
            LayoutElement iconLayout = icon.AddComponent<LayoutElement>();
            iconLayout.preferredWidth = 50;
            iconLayout.preferredHeight = 50;
            iconLayout.flexibleWidth = 0;
            
            GameObject nameObj = new GameObject("NameText");
            nameObj.transform.SetParent(listItem.transform, false);
            TextMeshProUGUI nameText = nameObj.AddComponent<TextMeshProUGUI>();
            nameText.text = "Equipment Name";
            nameText.fontSize = 18;
            nameText.color = Color.white;
            nameText.alignment = TextAlignmentOptions.Left;
            RectTransform nameRt = nameObj.GetComponent<RectTransform>();
            nameRt.sizeDelta = new Vector2(200, 50);
            LayoutElement nameLayout = nameObj.AddComponent<LayoutElement>();
            nameLayout.preferredWidth = 200;
            nameLayout.flexibleWidth = 1;
            
            GameObject costObj = new GameObject("CostText");
            costObj.transform.SetParent(listItem.transform, false);
            TextMeshProUGUI costText = costObj.AddComponent<TextMeshProUGUI>();
            costText.text = "100 souls";
            costText.fontSize = 16;
            costText.color = new Color(1f, 0.84f, 0f);
            costText.alignment = TextAlignmentOptions.Right;
            RectTransform costRt = costObj.GetComponent<RectTransform>();
            costRt.sizeDelta = new Vector2(100, 50);
            LayoutElement costLayout = costObj.AddComponent<LayoutElement>();
            costLayout.preferredWidth = 100;
            costLayout.flexibleWidth = 0;

            string prefabPath = "Assets/RealmCrawler_Project/Prefabs/UI/EquipmentListItemPrefab.prefab";
            string directory = System.IO.Path.GetDirectoryName(prefabPath);
            
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }

            if (System.IO.File.Exists(prefabPath))
            {
                AssetDatabase.DeleteAsset(prefabPath);
                Debug.Log("✓ Deleted old prefab");
            }

            GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(listItem, prefabPath);
            DestroyImmediate(listItem);

            Debug.Log($"✓ Created NEW list item prefab at: {prefabPath}");

            return savedPrefab;
        }

        private static void WireUpScripts(GameObject uiManager, GameObject topBar, GameObject equipmentPanel, 
                                         GameObject statsPanel, GameObject cantripPanel, GameObject detailPopup, 
                                         GameObject cantripTooltip, GameObject backButton, GameObject playButton,
                                         GameObject equipListItemPrefab)
        {
            Debug.Log("Wiring up scripts for VERTICAL LIST layout...");
            
            LoadoutUIManager loadoutUI = uiManager.AddComponent<LoadoutUIManager>();
            EquipmentGridManager gridManager = uiManager.AddComponent<EquipmentGridManager>();
            
            SetFieldValue(loadoutUI, "loadoutCostText", topBar.transform.Find("LoadoutCostText").GetComponent<TextMeshProUGUI>());
            SetFieldValue(loadoutUI, "currentSoulsText", topBar.transform.Find("CurrentSoulsText").GetComponent<TextMeshProUGUI>());
            SetFieldValue(loadoutUI, "healthStatText", statsPanel.transform.Find("HealthStatText").GetComponent<TextMeshProUGUI>());
            SetFieldValue(loadoutUI, "manaStatText", statsPanel.transform.Find("ManaStatText").GetComponent<TextMeshProUGUI>());
            SetFieldValue(loadoutUI, "speedStatText", statsPanel.transform.Find("SpeedStatText").GetComponent<TextMeshProUGUI>());
            SetFieldValue(loadoutUI, "xpRadiusStatText", statsPanel.transform.Find("XpRadiusStatText").GetComponent<TextMeshProUGUI>());
            SetFieldValue(loadoutUI, "playButton", playButton.GetComponent<Button>());
            SetFieldValue(loadoutUI, "backButton", backButton.GetComponent<Button>());
            SetFieldValue(loadoutUI, "cantripDisplay", cantripPanel.GetComponent<CantripDisplayPanel>());
            SetFieldValue(loadoutUI, "equipmentGrid", gridManager);

            Transform tabContainer = equipmentPanel.transform.Find("TabContainer");
            SetFieldValue(gridManager, "hatTabButton", tabContainer.Find("HatTabButton").GetComponent<Button>());
            SetFieldValue(gridManager, "cloakTabButton", tabContainer.Find("CloakTabButton").GetComponent<Button>());
            SetFieldValue(gridManager, "bootsTabButton", tabContainer.Find("BootsTabButton").GetComponent<Button>());
            SetFieldValue(gridManager, "reliquaryTabButton", tabContainer.Find("ReliquaryTabButton").GetComponent<Button>());
            SetFieldValue(gridManager, "weaponTabButton", tabContainer.Find("WeaponTabButton").GetComponent<Button>());

            GameObject content = equipmentPanel.transform.Find("EquipmentListScrollView/Viewport/Content").gameObject;
            SetFieldValue(gridManager, "listContainer", content.transform);
            SetFieldValue(gridManager, "equipmentListItemPrefab", equipListItemPrefab);
            SetFieldValue(gridManager, "detailPopup", detailPopup.GetComponent<EquipmentDetailPopup>());
            
            Sprite defaultIcon = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
            SetFieldValue(gridManager, "defaultEquipmentIcon", defaultIcon);
            
            Debug.Log("✓ Wired listContainer and equipmentListItemPrefab");

            EquipmentDetailPopup popup = detailPopup.AddComponent<EquipmentDetailPopup>();
            SetFieldValue(popup, "popupPanel", detailPopup);
            SetFieldValue(popup, "equipmentIconImage", detailPopup.transform.Find("EquipmentIconImage").GetComponent<Image>());
            SetFieldValue(popup, "equipmentNameText", detailPopup.transform.Find("EquipmentNameText").GetComponent<TextMeshProUGUI>());
            SetFieldValue(popup, "statsText", detailPopup.transform.Find("StatsText").GetComponent<TextMeshProUGUI>());
            SetFieldValue(popup, "buffText", detailPopup.transform.Find("BuffText").GetComponent<TextMeshProUGUI>());
            SetFieldValue(popup, "costText", detailPopup.transform.Find("CostText").GetComponent<TextMeshProUGUI>());
            SetFieldValue(popup, "purchaseButton", detailPopup.transform.Find("PurchaseButton").GetComponent<Button>());
            SetFieldValue(popup, "backButton", detailPopup.transform.Find("BackButton").GetComponent<Button>());
            SetFieldValue(popup, "loadoutUIManager", loadoutUI);

            CantripDisplayPanel cantripDisplay = cantripPanel.AddComponent<CantripDisplayPanel>();
            Transform primaryHover = cantripPanel.transform.Find("PrimaryHoverArea");
            Transform secondaryHover = cantripPanel.transform.Find("SecondaryHoverArea");
            
            SetFieldValue(cantripDisplay, "primaryCantripIcon", primaryHover.Find("PrimaryCantripIcon").GetComponent<Image>());
            SetFieldValue(cantripDisplay, "secondaryCantripIcon", secondaryHover.Find("SecondaryCantripIcon").GetComponent<Image>());
            SetFieldValue(cantripDisplay, "primaryLabel", primaryHover.Find("PrimaryLabel").GetComponent<TextMeshProUGUI>());
            SetFieldValue(cantripDisplay, "secondaryLabel", secondaryHover.Find("SecondaryLabel").GetComponent<TextMeshProUGUI>());

            Debug.Log("✓ All scripts wired up successfully!");
        }

        private static void SetFieldValue(Object obj, string fieldName, Object value)
        {
            SerializedObject so = new SerializedObject(obj);
            SerializedProperty prop = so.FindProperty(fieldName);
            if (prop != null)
            {
                prop.objectReferenceValue = value;
                so.ApplyModifiedProperties();
            }
        }

        private static GameObject CreateUIPanel(string name, Transform parent)
        {
            GameObject panel = new GameObject(name);
            panel.transform.SetParent(parent, false);
            panel.AddComponent<RectTransform>();
            panel.AddComponent<Image>();
            return panel;
        }

        private static void SetPanelColor(GameObject panel, Color color)
        {
            Image img = panel.GetComponent<Image>();
            if (img != null)
                img.color = color;
        }

        private static GameObject CreateTextMeshPro(string name, Transform parent)
        {
            GameObject textObj = new GameObject(name);
            textObj.transform.SetParent(parent, false);
            textObj.AddComponent<RectTransform>();
            textObj.AddComponent<TextMeshProUGUI>();
            return textObj;
        }

        private static void ConfigureText(GameObject textObj, string text, int fontSize, TextAlignmentOptions alignment)
        {
            TextMeshProUGUI tmp = textObj.GetComponent<TextMeshProUGUI>();
            if (tmp != null)
            {
                tmp.text = text;
                tmp.fontSize = fontSize;
                tmp.alignment = alignment;
                tmp.color = Color.white;
            }
        }

        private static GameObject CreateButton(string name, Transform parent, string text, Vector2 anchor, Vector2 position, Vector2 size)
        {
            GameObject buttonObj = new GameObject(name);
            buttonObj.transform.SetParent(parent, false);
            
            RectTransform rt = buttonObj.AddComponent<RectTransform>();
            SetAnchor(buttonObj, anchor, anchor, anchor);
            rt.anchoredPosition = position;
            rt.sizeDelta = size;

            Image img = buttonObj.AddComponent<Image>();
            img.color = new Color(0.2f, 0.6f, 1f, 1f);

            Button btn = buttonObj.AddComponent<Button>();

            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform, false);
            TextMeshProUGUI tmpText = textObj.AddComponent<TextMeshProUGUI>();
            tmpText.text = text;
            tmpText.fontSize = 20;
            tmpText.alignment = TextAlignmentOptions.Center;
            tmpText.color = Color.white;
            tmpText.fontStyle = FontStyles.Bold;

            RectTransform textRT = textObj.GetComponent<RectTransform>();
            textRT.anchorMin = Vector2.zero;
            textRT.anchorMax = Vector2.one;
            textRT.sizeDelta = Vector2.zero;

            return buttonObj;
        }

        private static void SetAnchor(GameObject obj, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot)
        {
            RectTransform rt = obj.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin = anchorMin;
                rt.anchorMax = anchorMax;
                rt.pivot = pivot;
            }
        }
    }
}
