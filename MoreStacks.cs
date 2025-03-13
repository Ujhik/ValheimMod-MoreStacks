using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using static UnityEngine.InputSystem.InputRemoting;

namespace MoreStacks {
    public class StackInfo {
        public string idStackToDuplicate { get; set; }
        public string idStackMaterial { get; set; }

        public Vector3 localScale { get; set; }

        public StackInfo(string idStackToDuplicate, string idStackMaterial)
            : this(idStackToDuplicate, idStackMaterial, Vector3.zero) { }

        public StackInfo(string idStackToDuplicate, string idStackMaterial, float uniformScale)
            : this(idStackToDuplicate, idStackMaterial, new Vector3(uniformScale, uniformScale, uniformScale)) { }

        public StackInfo(string idStackToDuplicate, string idStackMaterial, Vector3 localScale) {
            this.idStackToDuplicate = idStackToDuplicate;
            this.idStackMaterial = idStackMaterial;
            this.localScale = localScale;
        }
    }

    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    internal class MoreStacks : BaseUnityPlugin {
        public const string PluginGUID = $"ujhik.{PluginName}";
        public const string PluginName = "MoreStacks";
        public const string PluginVersion = "1.0.0";

        private static bool firstExecution = false;

        public List<StackInfo> stackInfoList = new List<StackInfo>();

        private List<CustomPiece> renderSprites = new List<CustomPiece>();

        private Harmony _harmony;

        public static CustomLocalization Localization = LocalizationManager.Instance.GetLocalization();

        private static ConfigEntry<bool> isSkipNight;
        private static ConfigEntry<float> testingSlider1;
        private static ConfigEntry<float> testingSlider2;
        private static ConfigEntry<float> testingSlider3;
        private static ConfigEntry<float> testingSlider4;

        public static event Action OnPrefabsRegistered;

        private void Awake() {
            CreateConfigValues();
            _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PluginGUID);

            InitializeStacks();

            PrefabManager.OnVanillaPrefabsAvailable += onVanillaPrefabsAvailable;
            PieceManager.OnPiecesRegistered += OnPiecesRegistered;
        }

        private void InitializeStacks() {
            stackInfoList = new List<StackInfo> {
                new StackInfo("coal_pile", "CopperOre"),
                new StackInfo("coal_pile", "IronOre"),
                new StackInfo("coal_pile", "TinOre"),
                new StackInfo("coal_pile", "SilverOre"),
                new StackInfo("stone_pile", "FlametalOre"),
                new StackInfo("stone_pile", "FlametalOreNew"),
                new StackInfo("blackmarble_pile", "CopperScrap", 0.6f),
                new StackInfo("blackmarble_pile", "BronzeScrap", 0.6f),
                new StackInfo("blackmarble_pile", "IronScrap", 0.6f),
                new StackInfo("blackmarble_pile", "BlackMetalScrap", 0.6f),
                new StackInfo("blackmarble_pile", "Resin", 0.5f),
                new StackInfo("stone_pile", "BeechSeeds", new Vector3(0.25f, 0.35f, 0.25f)),
                new StackInfo("grausten_pile", "Flint", 0.5f),
                new StackInfo("wood_fine_stack", "ElderBark", new Vector3(0.4f, 0.3f, 0.4f)),
                new StackInfo("stone_pile", "GreydwarfEye", new Vector3(0.15f, 0.25f, 0.15f)),
                new StackInfo("blackmarble_pile", "Honey", 0.5f),
                new StackInfo("bone_stack", "WitheredBone", 1.5f),
                new StackInfo("stone_pile", "Ooze", new Vector3(0.75f, 0.80f, 0.75f)),
                //new StackInfo("blackmarble_pile", "LeatherScraps", new Vector3(0.6f, 0.3f, 0.6f))

                // Guck
                // Blood bag
            };
        }

        private void CreateConfigValues() {
            ConfigurationManagerAttributes isAdminOnly = new ConfigurationManagerAttributes { IsAdminOnly = true };
            AcceptableValueRange<float> slider1Range = new AcceptableValueRange<float>(0, 1);
            AcceptableValueRange<float> sliderRange2 = new AcceptableValueRange<float>(0, 1);

            //testingSlider1 = Config.Bind("General",
            //    "testingSlider1",
            //    0.5f,
            //    new ConfigDescription("", slider1Range, isAdminOnly));

            //testingSlider2 = Config.Bind("General",
            //    "testingSlider2",
            //    0.5f,
            //    new ConfigDescription("", slider1Range, isAdminOnly));
            //testingSlider3 = Config.Bind("General",
            //    "testingSlider3",
            //    0.5f,
            //    new ConfigDescription("", sliderRange2, isAdminOnly));

            //testingSlider4 = Config.Bind("General",
            //    "testingSlider4",
            //    0.5f,
            //    new ConfigDescription("", sliderRange2, isAdminOnly));
        }

        [HarmonyPatch(typeof(EnvMan), "FixedUpdate")]
        public static class EnvMan_Update_Patch {
            private static void Prefix(ref long ___m_dayLengthSec, ref EnvMan ___s_instance) {
                GameObject newStack = PrefabManager.Instance.GetPrefab(getStackName("TrophySkeleton"));
                if (newStack != null) {
                    MeshRenderer newStackMeshRenderer = newStack.GetComponentInChildren<MeshRenderer>();
                    Material material = newStackMeshRenderer.sharedMaterial;

                    //material.SetTextureScale("_MainTex", new Vector2(testingSlider1.Value, testingSlider2.Value));
                    //material.SetTextureOffset("_MainTex", new Vector2(testingSlider3.Value, testingSlider4.Value)); 

                    //if (material.HasProperty("_Glossiness")) {
                    //    material.SetFloat("_Glossiness", testingSlider1.Value); // Max glossiness
                    //}
                    //if (material.HasProperty("_Metallic")) {
                    //    material.SetFloat("_Metallic", testingSlider2.Value); // Max metallic effect
                    //}
                    //if (material.HasProperty("_MetalGloss")) {
                    //    material.SetFloat("_MetalGloss", testingSlider3.Value); // Extra glossiness
                    //}
                    //if (material.HasProperty("_UseGlossMap")) {
                    //    material.SetFloat("_UseGlossMap", testingSlider4.Value); // Ensure gloss map is used
                    //}

                    //changeCullingDistanceLOD(newStack, testingSlider1.Value);
                }

                //if (!firstExecution && Time.frameCount % 1200 == 0) {
                //    firstExecution = true;
                //    LogAllLODGroups();

                //    GameObject honeyPile = GameObject.Find("OreStacks_Honey_pile(Clone)");
                //    logGameobject(honeyPile);
                //    GameObject newFromHoneyPile = honeyPile.transform.Find("New").gameObject;
                //    changeCullingDistanceLOD(newFromHoneyPile, 0.5f);
                //}
            }
        }

        

        private void OnDestroy() {
            _harmony?.UnpatchSelf();
        }

        private void onVanillaPrefabsAvailable() {
            //InitializeStacks();

            foreach (var stack in stackInfoList) {
                //Jotunn.Logger.LogError($"LOGGING STACK: {stack.idStackToDuplicate} : {stack.idStackMaterial}");
                generateNewStack(stack.idStackToDuplicate, stack.idStackMaterial, stack.localScale);
            }
        }

        public static string getStackName(string materialPrefabId) {
            return $"{PluginName}_{materialPrefabId}_pile";
        }

        private void generateNewStack(string prefabToDuplicateId, string materialPrefabId) {
            generateNewStack(prefabToDuplicateId, materialPrefabId, getStackName(materialPrefabId), Vector3.zero);
        }

        private void generateNewStack(string prefabToDuplicateId, string materialPrefabId, float newLocalScale) {
            generateNewStack(prefabToDuplicateId, materialPrefabId, getStackName(materialPrefabId), new Vector3(newLocalScale, newLocalScale, newLocalScale));
        }

        private void generateNewStack(string prefabToDuplicateId, string materialPrefabId, Vector3 newLocalScale) {
            generateNewStack(prefabToDuplicateId, materialPrefabId, getStackName(materialPrefabId), newLocalScale);
        }

        //Warning: Changing the newLocalScale in future versions can make alrealy built stacks to load the stack floating in the air
        private void generateNewStack(string prefabToDuplicateId, string materialPrefabId, string newStackId, Vector3 newLocalScale) {
            GameObject prefabToDuplicate = PrefabManager.Instance.GetPrefab(prefabToDuplicateId);

            if (prefabToDuplicate == null) {
                Jotunn.Logger.LogError($"Prefab to duplicate not found!: {prefabToDuplicateId}");
                return;
            }

            if(PrefabManager.Instance.GetPrefab(newStackId) != null) { // Already registered
                return;
            }

            GameObject newStack = PrefabManager.Instance.CreateClonedPrefab(newStackId, prefabToDuplicate);

            // Replace Material
            MeshRenderer newStackMeshRenderer = newStack.GetComponentInChildren<MeshRenderer>();
            GameObject materialPrefab = PrefabManager.Instance.GetPrefab(materialPrefabId);

            if (materialPrefab != null) {
                MeshRenderer oreMeshRenderer = materialPrefab.GetComponentInChildren<MeshRenderer>();

                if (oreMeshRenderer != null) {
                    newStackMeshRenderer.sharedMaterial = new Material(oreMeshRenderer.sharedMaterial); // Change material

                    Material material = newStackMeshRenderer.sharedMaterial;
                    adjustMaterial(material, materialPrefabId);

                    //ShaderHelper.ShaderDump(materialPrefab);
                }
                else {
                    Jotunn.Logger.LogError("MeshRenderer not found!");
                }
            }
            else {
                Jotunn.Logger.LogError(materialPrefabId + " prefab not found!");
            }

            //Obtaining the inventory name
            ItemDrop itemDrop = materialPrefab.GetComponent<ItemDrop>();
            string materialInventoryName = itemDrop.m_itemData.m_shared.m_name;

            // Register it as a new build piece
            CustomPiece newStackPiece = new CustomPiece(newStack, fixReference: true,
                new PieceConfig {
                    Name = materialInventoryName + " Pile",
                    Description = "A pile of " + materialInventoryName,
                    PieceTable = "Hammer",
                    Category = "Misc",
                    Requirements = new[]
                    {
                        new RequirementConfig(materialPrefabId, 50, 0, true)
                    }
                });

            if (!newLocalScale.Equals(Vector3.zero)) {
                newStack.transform.localScale = newLocalScale;
                changeCullingDistanceLOD(newStack);     // To avoid small stockpiles from being culled too early
            }

            PieceManager.Instance.AddPiece(newStackPiece);
            renderSprites.Add(newStackPiece);

            //LogLodAndRenderer(prefabToDuplicate, newStack);
        }

        static void logMaterialProperties(Material material) {
            Jotunn.Logger.LogError($"Material: {material.name}" + ": " + "Shader Name: " + material.shader.name);
            for (int i = 0; i < material.shader.GetPropertyCount(); i++) {
                string propertyName = material.shader.GetPropertyName(i);
                Jotunn.Logger.LogError($"Property: {propertyName}");
            }
        }

        static void changeCullingDistanceLOD(GameObject gameObject) {
            float lodDistance = 0.01f;
            changeCullingDistanceLOD(gameObject, lodDistance);
        }

        static void changeCullingDistanceLOD(GameObject gameObject, float newScreenRelativeTransitionHeight) {
            //Avoid small stockpiles from being culled at a small distance
            LODGroup lodGroup = gameObject.GetComponentInChildren<LODGroup>();

            //logGameobject(gameObject);
            //Jotunn.Logger.LogError($"LOD FOUNDDDDD: {lodGroup} ID: {lodGroup.GetInstanceID()}");

            Renderer renderer = gameObject.GetComponentInChildren<Renderer>();

            if (lodGroup == null) {
                lodGroup = gameObject.AddComponent<LODGroup>();
                lodGroup.SetLODs(new LOD[] { new LOD(0.1f, new Renderer[] { renderer }) });

                //Jotunn.Logger.LogError($"LOD group ADDED: {lodGroup}");
            }
            gameObject.SetActive(true);

            LOD[] lods = lodGroup.GetLODs();
            lods[lods.Length - 1].screenRelativeTransitionHeight = newScreenRelativeTransitionHeight;
            lods[lods.Length - 1].renderers = new Renderer[] { renderer };  // Important to set the renderer, because it is not set by default sometimes, like in blackmarble_pile

            lodGroup.SetLODs(lods);
            lodGroup.RecalculateBounds();
        }

        static void logGameobject(GameObject gameObject) {
            Jotunn.Logger.LogInfo($"Logging gameobject: {gameObject.name} from: {gameObject.transform.parent?.name}");
            foreach (Transform child in gameObject.transform) {
                Jotunn.Logger.LogInfo($"{gameObject.name} Child: {child.name} type: {child.GetType()}");
                logGameobject(child.gameObject);
            }
        }

        void LogLodAndRenderer(GameObject original, GameObject clone) {
            LODGroup originalLOD = original.GetComponent<LODGroup>();
            LODGroup cloneLOD = clone.GetComponent<LODGroup>();

            Renderer originalRenderer = original.GetComponentInChildren<Renderer>();
            Renderer cloneRenderer = clone.GetComponentInChildren<Renderer>();

            MeshRenderer originalMeshRenderer = original.GetComponentInChildren<MeshRenderer>();
            MeshRenderer cloneMeshRenderer = clone.GetComponentInChildren<MeshRenderer>();

            MeshFilter originalMeshFilter = original.GetComponentInChildren<MeshFilter>();
            MeshFilter cloneMeshFilter = clone.GetComponentInChildren<MeshFilter>();

            Jotunn.Logger.LogError($"Original LODGroup: {originalLOD?.GetInstanceID()} | Clone LODGroup: {cloneLOD?.GetInstanceID()}");
            Jotunn.Logger.LogError($"Same LODGroup? {originalLOD == cloneLOD}");

            Jotunn.Logger.LogError($"Original Renderer: {originalRenderer?.GetInstanceID()} | Clone Renderer: {cloneRenderer?.GetInstanceID()}");
            Jotunn.Logger.LogError($"Same Renderer? {originalRenderer == cloneRenderer}");

            Jotunn.Logger.LogError($"Original MeshRenderer: {originalMeshRenderer?.GetInstanceID()} | Clone Renderer: {cloneMeshRenderer?.GetInstanceID()}");
            Jotunn.Logger.LogError($"Same MeshRenderer? {originalMeshRenderer == cloneMeshRenderer}");

            Jotunn.Logger.LogError($"Original MeshFilter: {originalMeshFilter?.GetInstanceID()} | Clone MeshFilter: {cloneMeshFilter?.GetInstanceID()}");
            Jotunn.Logger.LogError($"Same MeshFilter? {originalMeshFilter?.GetInstanceID() == cloneMeshFilter?.GetInstanceID()}");

            Jotunn.Logger.LogError($"Original Renderer name: {originalRenderer?.name} | Clone Renderer name: {cloneRenderer?.name}");

            Component[] components = original.GetComponents<Component>();
            foreach (Component comp in components) {
                Debug.Log(comp.GetType().Name);
            }
        }

        private void adjustMaterial(Material material, string materialPrefabId) {
            switch (materialPrefabId) {
                case "TinOre":
                    if (material.HasProperty("_Glossiness")) {
                        material.SetFloat("_Glossiness", 0.5f); // Max glossiness
                    }
                    if (material.HasProperty("_Metallic")) {
                        material.SetFloat("_Metallic", 0.5f); // Max metallic effect
                    }
                    if (material.HasProperty("_MetalGloss")) {
                        material.SetFloat("_MetalGloss", 1.0f); // Extra glossiness
                    }
                    if (material.HasProperty("_UseGlossMap")) {
                        material.SetFloat("_UseGlossMap", 1.0f); // Ensure gloss map is used
                    }
                    break;
                case "SilverOre":
                    if (material.HasProperty("_Glossiness")) {
                        material.SetFloat("_Glossiness", 0.9f); // Max glossiness
                    }
                    if (material.HasProperty("_Metallic")) {
                        material.SetFloat("_Metallic", 0.6f); // Max metallic effect
                    }
                    if (material.HasProperty("_MetalGloss")) {
                        material.SetFloat("_MetalGloss", 1.0f); // Extra glossiness
                    }
                    if (material.HasProperty("_UseGlossMap")) {
                        material.SetFloat("_UseGlossMap", 1.0f); // Ensure gloss map is used
                    }
                    if (material.HasProperty("_Color")) {
                        material.SetColor("_Color", new Color(1.3f, 1.3f, 1.3f)); 
                    }
                    if (material.HasProperty("_EmissionColor")) {
                        material.SetColor("_EmissionColor", new Color(0.01f, 0.01f, 0.01f)); 
                    }
                    break;
                case "FlametalOreNew":
                    break;
                case "Resin":
                    // Ensure the material supports transparency
                    material.SetFloat("_Mode", 3); // 3 = Transparent Mode
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    //material.SetInt("_ZWrite", 0); // Disable depth writing for transparency
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");

                    // Apply transparency by changing the color alpha
                    if (material.HasProperty("_Color")) {
                        Color color = material.GetColor("_Color");
                        material.SetColor("_Color", new Color(color.r, color.g, color.b, 0.9f)); // Adjust alpha for translucency
                    }

                    // Ensure the render queue is set to Transparent
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                    break;
                case "BeechSeeds":
                    if (material.HasProperty("_MainTex")) {
                        material.SetTextureScale("_MainTex", new Vector2(0.7f, 1.5f)); 
                        material.SetTextureOffset("_MainTex", new Vector2(0.75f, 0.75f)); 
                    }
                    //if (material.HasProperty("_BumpMap")) {
                    //    material.SetTexture("_BumpMap", material.GetTexture("_MainTex")); // Use the same texture
                    //    material.SetFloat("_BumpScale", 0.1f); // Increase the bump effect
                    //}
                    //if (material.HasProperty("_DetailAlbedoMap")) {
                    //    material.SetTexture("_DetailAlbedoMap", material.GetTexture("_MainTex")); // Reuse same texture
                    //    material.SetTextureScale("_DetailAlbedoMap", new Vector2(10f, 10f)); // Adds extra fine details
                    //}
                    break;
                case "ElderBark":
                    if (material.HasProperty("_MainTex")) {
                        material.SetTextureScale("_MainTex", new Vector2(1.12f, 0.75f)); 
                        material.SetTextureOffset("_MainTex", new Vector2(0.67f, 0.14f)); 
                    }
                    break;
                case "Honey":
                    if (material.HasProperty("_Glossiness")) {
                        material.SetFloat("_Glossiness", 0.9f);
                    }
                    if (material.HasProperty("_Metallic")) {
                        material.SetFloat("_Metallic", 0.3f);
                    }
                    if (material.HasProperty("_MainTex")) {
                        material.SetTextureScale("_MainTex", new Vector2(0.4f, 0.34f)); 
                        material.SetTextureOffset("_MainTex", new Vector2(0f, 0.05f)); 
                    }
                    break;
                case "LeatherScraps":
                    if (material.HasProperty("_MainTex")) {
                        material.SetTextureScale("_MainTex", new Vector2(0.69f, 0.82f)); 
                        material.SetTextureOffset("_MainTex", new Vector2(0.21f, 0.3f)); 
                    }
                    if (material.HasProperty("_Cutoff"))
                        material.SetFloat("_Cutoff", 0.001f);
                    break;
                default:
                    break;
            }
        }

        private void OnPiecesRegistered() {
            PieceManager.OnPiecesRegistered -= OnPiecesRegistered;

            foreach (CustomPiece piece in renderSprites) {
                StartCoroutine(RenderSprite(piece));
            }
        }

        // Generates de recipe 2D icon image
        private IEnumerator RenderSprite(CustomPiece piece) {
            yield return null;

            // Render the icon with the custom shader
            piece.Piece.m_icon = RenderManager.Instance.Render(new RenderManager.RenderRequest(piece.PiecePrefab) {
                Width = 64,
                Height = 64,
                Rotation = RenderManager.IsometricRotation * Quaternion.Euler(0f, -45f, 0f), // Try to match Valheim's angle
                UseCache = false,
                TargetPlugin = Info.Metadata,
            });

        }

        static void LogAllLODGroups() {
            LODGroup[] lodGroups = FindObjectsOfType<LODGroup>();

            Debug.Log($"[LOD Logger] Found {lodGroups.Length} LODGroups in the scene.");

            foreach (LODGroup lodGroup in lodGroups) {
                if (lodGroup.gameObject != null && lodGroup.gameObject.name.Contains("OreStacks_Honey_pile") || lodGroup.gameObject.name.Contains("New")) {
                    Debug.Log($"[LOD Logger] LODGroup on '{lodGroup.gameObject?.name}':");

                    LOD[] lods = lodGroup.GetLODs();

                    for (int i = 0; lods != null && i < lods.Length; i++) {
                        Debug.Log($"   LOD {i}: Screen Relative Transition Height = {lods[i].screenRelativeTransitionHeight}");

                        foreach (Renderer renderer in lods[i].renderers) {
                            Debug.Log($"      Renderer ID: {renderer?.GetInstanceID()} | Object: {renderer?.gameObject?.name}");
                            Debug.Log($"      Owner of Renderer ID : {renderer?.gameObject?.GetInstanceID()} | Object: {renderer?.gameObject?.name}");

                            // Check if the renderer's GameObject has a parent
                            Transform parent = renderer?.transform?.parent;
                            if (parent != null) {
                                Debug.Log($"      Parent Object ID: {parent.gameObject?.GetInstanceID()} | Object: {parent.gameObject?.name}");
                                Debug.Log($"      Parent is a Component: {parent.gameObject?.GetType()}");

                                // Check if the parent has another parent (going up the hierarchy)
                                Transform grandParent = parent?.parent;
                                if (grandParent != null) {
                                    Debug.Log($"      Grandparent Object ID: {grandParent.gameObject?.GetInstanceID()} | Object: {grandParent.gameObject?.name}");
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
