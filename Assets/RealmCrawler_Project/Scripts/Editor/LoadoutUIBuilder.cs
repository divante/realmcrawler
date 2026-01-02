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
    public class LoadoutUIBuilder : EditorWindow
    {
        private const string BUILDER_VERSION = "2.0_VerticalList";
        
        [MenuItem("RealmCrawler/Build Loadout UI")]
        public static void ShowWindow()
        {
            GetWindow<LoadoutUIBuilder>("Loadout UI Builder");
        }

        private void OnGUI()
        {
            GUILayout.Label("Loadout UI Builder", EditorStyles.boldLabel);
            GUILayout.Space(10);

            EditorGUILayout.HelpBox(
                "This will create the complete loadout UI in the current scene.\n\n" +
                "Make sure you have Scene_Loadout open!\n\n" +
                "This will:\n" +
                "• Create Canvas with all UI elements\n" +
                "• Configure all components\n" +
                "• Wire up all script references\n" +
                "• Create equipment button prefab",
                MessageType.Info);

            GUILayout.Space(10);

            if (GUILayout.Button("Build Complete UI", GUILayout.Height(40)))
            {
                if (EditorUtility.DisplayDialog(
                    "Build Loadout UI",
                    "This will create the complete loadout UI in the current scene. Continue?",
                    "Yes, Build It!",
                    "Cancel"))
                {
                    BuildCompleteUI();
                }
            }

            GUILayout.Space(20);

            if (GUILayout.Button("Clean Up Existing UI", GUILayout.Height(30)))
            {
                if (EditorUtility.DisplayDialog(
                    "Clean Up UI",
                    "This will delete any existing 'LoadoutCanvas' in the scene. Continue?",
                    "Yes, Clean Up",
                    "Cancel"))
                {
                    CleanUpExistingUI();
                }
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
            Debug.Log("Building UI with VERTICAL LIST layout...");
            
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

            GameObject equipButtonPrefab = CreateEquipmentButtonPrefab();

            GameObject uiManager = new GameObject("UIManager");
            uiManager.transform.SetParent(canvasObj.transform, false);

            WireUpScripts(uiManager, topBar, equipmentPanel, statsPanel, cantripPanel, 
                         detailPopup, cantripTooltip, backButton, playButton, equipButtonPrefab);

            Selection.activeGameObject = canvasObj;
            EditorUtility.SetDirty(canvasObj);

            Debug.Log("✅ Loadout UI built successfully! Check the Scene hierarchy.");
        }

        private static void EnsureGameManager()
        {
            GameManager existingGM = FindFirstObjectByType<GameManager>();
            
            if (existingGM != null)
            {
                Debug.Log("GameManager already exists in scene.");
                
                if (existingGM.EquipmentDB == null)
                {
                    string dbPath = "Assets/RealmCrawler_Project/ScriptableObjects/Core/EquipmentDatabase.asset";
                    EquipmentDatabase db = AssetDatabase.LoadAssetAtPath<EquipmentDatabase>(dbPath);
                    
                    if (db != null)
                    {
                        SerializedObject so = new SerializedObject(existingGM);
                        SerializedProperty prop = so.FindProperty("equipmentDB");
                        prop.objectReferenceValue = db;
                        so.ApplyModifiedProperties();
                        
                        Debug.Log("✅ Assigned EquipmentDatabase to existing GameManager.");
                    }
                    else
                    {
                        Debug.LogWarning($"Could not find EquipmentDatabase at {dbPath}");
                    }
                }
                
                return;
            }

            GameObject gmObj = new GameObject("GameManager");
            GameManager gm = gmObj.AddComponent<GameManager>();
            
            string databasePath = "Assets/RealmCrawler_Project/ScriptableObjects/Core/EquipmentDatabase.asset";
            EquipmentDatabase database = AssetDatabase.LoadAssetAtPath<EquipmentDatabase>(databasePath);
            
            if (database != null)
            {
                SerializedObject so = new SerializedObject(gm);
                SerializedProperty prop = so.FindProperty("equipmentDB");
                prop.objectReferenceValue = database;
                so.ApplyModifiedProperties();
                
                Debug.Log("✅ Created GameManager and assigned EquipmentDatabase.");
            }
            else
            {
                Debug.LogError($"Could not find EquipmentDatabase at {databasePath}. Please create it or check the path.");
            }
            
            gm.ResetToDefaultLoadout();
            EditorUtility.SetDirty(gmObj);
        }

        private static GameObject CreateCanvas()
        {
            GameObject canvasObj = new GameObject("LoadoutCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            canvasObj.AddComponent<GraphicRaycaster>();

            if (FindFirstObjectByType<EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
            }

            return canvasObj;
        }

        private static GameObject CreateTopBar(Transform parent)
        {
            GameObject topBar = CreateUIPanel("TopBar", parent);
            RectTransform rt = topBar.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 1);
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = new Vector2(0, 60);

            SetPanelColor(topBar, new Color(0, 0, 0, 0.6f));

            GameObject costText = CreateTextMeshPro("LoadoutCostText", topBar.transform);
            ConfigureText(costText, "Current Loadout Cost: 0", 24, TextAlignmentOptions.MidlineLeft);
            SetAnchor(costText, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0.5f));
            costText.GetComponent<RectTransform>().anchoredPosition = new Vector2(50, 0);
            costText.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 40);

            GameObject soulsText = CreateTextMeshPro("CurrentSoulsText", topBar.transform);
            ConfigureText(soulsText, "Current Souls: 1000", 24, TextAlignmentOptions.MidlineRight);
            SetAnchor(soulsText, new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(1, 0.5f));
            soulsText.GetComponent<RectTransform>().anchoredPosition = new Vector2(-50, 0);
            soulsText.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 40);
            soulsText.GetComponent<TextMeshProUGUI>().color = new Color(1f, 0.84f, 0f);

            return topBar;
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

            GameObject scrollView = CreateScrollView("EquipmentGridScrollView", panel.transform);
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

        private static GameObject CreateScrollView(string name, Transform parent)
        {
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
            
            Debug.Log("Created Content with VERTICAL LAYOUT GROUP (not grid!)");

            ContentSizeFitter fitter = content.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scroll.content = contentRT;
            scroll.viewport = vpRT;
            scroll.horizontal = false;
            scroll.vertical = true;

            GameObject scrollbar = new GameObject("Scrollbar Vertical");
            scrollbar.transform.SetParent(scrollView.transform, false);
            RectTransform sbRT = scrollbar.AddComponent<RectTransform>();
            SetAnchor(scrollbar, new Vector2(1, 0), new Vector2(1, 1), new Vector2(1, 0.5f));
            sbRT.sizeDelta = new Vector2(20, 0);

            Image sbImg = scrollbar.AddComponent<Image>();
            sbImg.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

            Scrollbar sb = scrollbar.AddComponent<Scrollbar>();
            sb.direction = Scrollbar.Direction.BottomToTop;

            GameObject handle = new GameObject("Sliding Area");
            handle.transform.SetParent(scrollbar.transform, false);
            RectTransform handleRT = handle.AddComponent<RectTransform>();
            handleRT.anchorMin = Vector2.zero;
            handleRT.anchorMax = Vector2.one;
            handleRT.sizeDelta = new Vector2(-20, -20);

            GameObject handleChild = new GameObject("Handle");
            handleChild.transform.SetParent(handle.transform, false);
            RectTransform hcRT = handleChild.AddComponent<RectTransform>();
            hcRT.anchorMin = Vector2.zero;
            hcRT.anchorMax = Vector2.one;
            hcRT.sizeDelta = Vector2.zero;

            Image hcImg = handleChild.AddComponent<Image>();
            hcImg.color = new Color(0.8f, 0.8f, 0.8f, 0.8f);

            sb.handleRect = hcRT;
            sb.targetGraphic = hcImg;

            scroll.verticalScrollbar = sb;

            return scrollView;
        }

        private static GameObject CreateStatsPanel(Transform parent)
        {
            GameObject panel = CreateUIPanel("StatsPanel", parent);
            RectTransform rt = panel.GetComponent<RectTransform>();
            SetAnchor(panel, new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1));
            rt.anchoredPosition = new Vector2(-20, -150);
            rt.sizeDelta = new Vector2(300, 400);

            SetPanelColor(panel, new Color(0, 0, 0, 0.7f));

            GameObject label = CreateTextMeshPro("StatsLabel", panel.transform);
            ConfigureText(label, "STATS", 28, TextAlignmentOptions.Center);
            label.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
            label.GetComponent<TextMeshProUGUI>().color = new Color(1f, 0.84f, 0f);
            SetAnchor(label, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1));
            label.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -20);
            label.GetComponent<RectTransform>().sizeDelta = new Vector2(260, 40);

            string[] statNames = { "HealthStatText", "ManaStatText", "SpeedStatText", "XpRadiusStatText" };
            string[] statLabels = { "Health: 100", "Mana: 100", "Speed: 10.0", "XP Radius: 5.0" };
            float[] posY = { -80, -120, -160, -200 };

            for (int i = 0; i < statNames.Length; i++)
            {
                GameObject stat = CreateTextMeshPro(statNames[i], panel.transform);
                ConfigureText(stat, statLabels[i], 20, TextAlignmentOptions.Left);
                SetAnchor(stat, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1));
                stat.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, posY[i]);
                stat.GetComponent<RectTransform>().sizeDelta = new Vector2(260, 30);
            }

            return panel;
        }

        private static GameObject CreateCantripPanel(Transform parent)
        {
            GameObject panel = CreateUIPanel("CantripDisplayPanel", parent);
            RectTransform rt = panel.GetComponent<RectTransform>();
            SetAnchor(panel, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0));
            rt.anchoredPosition = new Vector2(0, 100);
            rt.sizeDelta = new Vector2(300, 160);

            SetPanelColor(panel, new Color(0, 0, 0, 0.7f));

            CreateCantripDisplay(panel.transform, "PrimaryHoverArea", "PrimaryCantripIcon", "PrimaryLabel", "Left Click", -80);
            CreateCantripDisplay(panel.transform, "SecondaryHoverArea", "SecondaryCantripIcon", "SecondaryLabel", "Right Click", 80);

            return panel;
        }

        private static void CreateCantripDisplay(Transform parent, string hoverName, string iconName, string labelName, string labelText, float posX)
        {
            GameObject hoverArea = new GameObject(hoverName);
            hoverArea.transform.SetParent(parent, false);
            RectTransform hoverRT = hoverArea.AddComponent<RectTransform>();
            hoverRT.sizeDelta = new Vector2(100, 100);
            hoverRT.anchoredPosition = new Vector2(posX, 20);

            Image hoverImg = hoverArea.AddComponent<Image>();
            hoverImg.color = new Color(1, 1, 1, 0);

            GameObject icon = new GameObject(iconName);
            icon.transform.SetParent(hoverArea.transform, false);
            RectTransform iconRT = icon.AddComponent<RectTransform>();
            iconRT.sizeDelta = new Vector2(80, 80);
            iconRT.anchoredPosition = Vector2.zero;

            Image iconImg = icon.AddComponent<Image>();
            iconImg.color = new Color(1, 1, 1, 0.3f);

            GameObject label = CreateTextMeshPro(labelName, hoverArea.transform);
            ConfigureText(label, labelText, 16, TextAlignmentOptions.Center);
            SetAnchor(label, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0));
            label.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -60);
            label.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 30);
        }

        private static GameObject CreateCantripTooltip(Transform parent)
        {
            GameObject tooltip = CreateUIPanel("CantripTooltip", parent);
            RectTransform rt = tooltip.GetComponent<RectTransform>();
            SetAnchor(tooltip, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0));
            rt.anchoredPosition = new Vector2(0, 280);
            rt.sizeDelta = new Vector2(300, 200);

            SetPanelColor(tooltip, new Color(0, 0, 0, 0.94f));

            GameObject title = CreateTextMeshPro("TooltipTitleText", tooltip.transform);
            ConfigureText(title, "Cantrip Name", 24, TextAlignmentOptions.Center);
            title.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
            SetAnchor(title, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1));
            title.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -20);
            title.GetComponent<RectTransform>().sizeDelta = new Vector2(280, 30);

            GameObject desc = CreateTextMeshPro("TooltipDescriptionText", tooltip.transform);
            ConfigureText(desc, "Description here", 16, TextAlignmentOptions.Center);
            SetAnchor(desc, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1));
            desc.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -80);
            desc.GetComponent<RectTransform>().sizeDelta = new Vector2(280, 80);

            GameObject stats = CreateTextMeshPro("TooltipStatsText", tooltip.transform);
            ConfigureText(stats, "Stats here", 14, TextAlignmentOptions.Left);
            SetAnchor(stats, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1));
            stats.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -150);
            stats.GetComponent<RectTransform>().sizeDelta = new Vector2(280, 60);

            tooltip.SetActive(false);

            return tooltip;
        }

        private static GameObject CreateDetailPopup(Transform parent)
        {
            GameObject popup = CreateUIPanel("EquipmentDetailPopup", parent);
            RectTransform rt = popup.GetComponent<RectTransform>();
            SetAnchor(popup, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = new Vector2(500, 600);

            SetPanelColor(popup, new Color(0.1f, 0.1f, 0.1f, 0.86f));

            GameObject icon = new GameObject("EquipmentIconImage");
            icon.transform.SetParent(popup.transform, false);
            RectTransform iconRT = icon.AddComponent<RectTransform>();
            SetAnchor(icon, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1));
            iconRT.anchoredPosition = new Vector2(0, -100);
            iconRT.sizeDelta = new Vector2(128, 128);
            Image iconImg = icon.AddComponent<Image>();
            iconImg.color = Color.white;

            GameObject name = CreateTextMeshPro("EquipmentNameText", popup.transform);
            ConfigureText(name, "Equipment Name", 32, TextAlignmentOptions.Center);
            name.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
            SetAnchor(name, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1));
            name.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -200);
            name.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 50);

            GameObject stats = CreateTextMeshPro("StatsText", popup.transform);
            ConfigureText(stats, "Stats", 20, TextAlignmentOptions.Left);
            SetAnchor(stats, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1));
            stats.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -280);
            stats.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 100);

            GameObject buff = CreateTextMeshPro("BuffText", popup.transform);
            ConfigureText(buff, "Buff", 20, TextAlignmentOptions.Left);
            SetAnchor(buff, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1));
            buff.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -380);
            buff.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 100);

            GameObject cost = CreateTextMeshPro("CostText", popup.transform);
            ConfigureText(cost, "Cost: 0", 24, TextAlignmentOptions.Center);
            cost.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
            SetAnchor(cost, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1));
            cost.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -460);
            cost.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 40);

            CreateButton(popup.transform, "PurchaseButton", "PURCHASE", new Vector2(-120, -520), new Vector2(180, 60));
            CreateButton(popup.transform, "BackButton", "BACK", new Vector2(120, -520), new Vector2(180, 60));

            popup.SetActive(false);

            return popup;
        }

        private static GameObject CreateBackButton(Transform parent)
        {
            GameObject button = CreateButton(parent, "BackButton", "BACK", Vector2.zero, new Vector2(120, 50));
            SetAnchor(button, new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1));
            button.GetComponent<RectTransform>().anchoredPosition = new Vector2(-20, -90);
            return button;
        }

        private static GameObject CreatePlayButton(Transform parent)
        {
            GameObject button = CreateButton(parent, "PlayButton", "PLAY", Vector2.zero, new Vector2(200, 80));
            SetAnchor(button, new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0));
            button.GetComponent<RectTransform>().anchoredPosition = new Vector2(-20, 20);
            
            TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();
            text.fontSize = 36;
            text.fontStyle = FontStyles.Bold;
            text.color = new Color(0.3f, 1f, 0.3f);

            return button;
        }

        private static GameObject CreateEquipmentButtonPrefab()
        {
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

            string prefabPath = "Assets/RealmCrawler_Project/Prefabs/UI/EquipmentButtonPrefab.prefab";
            string directory = System.IO.Path.GetDirectoryName(prefabPath);
            
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }

            if (System.IO.File.Exists(prefabPath))
            {
                AssetDatabase.DeleteAsset(prefabPath);
                Debug.Log($"Deleted old equipment button prefab");
            }

            GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(listItem, prefabPath);
            DestroyImmediate(listItem);

            Debug.Log($"Created NEW equipment list item prefab at: {prefabPath}");

            return savedPrefab;
        }

        private static void WireUpScripts(GameObject uiManager, GameObject topBar, GameObject equipmentPanel, 
                                         GameObject statsPanel, GameObject cantripPanel, GameObject detailPopup, 
                                         GameObject cantripTooltip, GameObject backButton, GameObject playButton,
                                         GameObject equipButtonPrefab)
        {
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

            GameObject content = equipmentPanel.transform.Find("EquipmentGridScrollView/Viewport/Content").gameObject;
            SetFieldValue(gridManager, "listContainer", content.transform);
            SetFieldValue(gridManager, "equipmentListItemPrefab", equipButtonPrefab);
            SetFieldValue(gridManager, "detailPopup", detailPopup.GetComponent<EquipmentDetailPopup>());
            
            Sprite defaultIcon = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
            SetFieldValue(gridManager, "defaultEquipmentIcon", defaultIcon);

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
            SetFieldValue(cantripDisplay, "tooltipPanel", cantripTooltip);
            SetFieldValue(cantripDisplay, "tooltipTitleText", cantripTooltip.transform.Find("TooltipTitleText").GetComponent<TextMeshProUGUI>());
            SetFieldValue(cantripDisplay, "tooltipDescriptionText", cantripTooltip.transform.Find("TooltipDescriptionText").GetComponent<TextMeshProUGUI>());
            SetFieldValue(cantripDisplay, "tooltipStatsText", cantripTooltip.transform.Find("TooltipStatsText").GetComponent<TextMeshProUGUI>());
            SetFieldValue(cantripDisplay, "primaryHoverArea", primaryHover.gameObject);
            SetFieldValue(cantripDisplay, "secondaryHoverArea", secondaryHover.gameObject);

            EditorUtility.SetDirty(uiManager);
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
            
            RectTransform rt = panel.AddComponent<RectTransform>();
            Image img = panel.AddComponent<Image>();
            
            return panel;
        }

        private static void SetPanelColor(GameObject panel, Color color)
        {
            Image img = panel.GetComponent<Image>();
            if (img != null)
            {
                img.color = color;
            }
        }

        private static GameObject CreateTextMeshPro(string name, Transform parent)
        {
            GameObject textObj = new GameObject(name);
            textObj.transform.SetParent(parent, false);
            
            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
            RectTransform rt = textObj.GetComponent<RectTransform>();
            
            return textObj;
        }

        private static void ConfigureText(GameObject textObj, string content, float fontSize, TextAlignmentOptions alignment)
        {
            TextMeshProUGUI text = textObj.GetComponent<TextMeshProUGUI>();
            text.text = content;
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.color = Color.white;
        }

        private static GameObject CreateButton(Transform parent, string name, string label, Vector2 pos, Vector2 size)
        {
            GameObject button = new GameObject(name);
            button.transform.SetParent(parent, false);

            RectTransform rt = button.AddComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;

            Image img = button.AddComponent<Image>();
            img.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);

            Button btn = button.AddComponent<Button>();

            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(button.transform, false);
            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
            text.text = label;
            text.fontSize = 20;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;

            RectTransform textRT = textObj.GetComponent<RectTransform>();
            textRT.anchorMin = Vector2.zero;
            textRT.anchorMax = Vector2.one;
            textRT.sizeDelta = Vector2.zero;

            return button;
        }

        private static void SetAnchor(GameObject obj, Vector2 min, Vector2 max, Vector2 pivot)
        {
            RectTransform rt = obj.GetComponent<RectTransform>();
            rt.anchorMin = min;
            rt.anchorMax = max;
            rt.pivot = pivot;
        }
    }
}
