using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace WTFGames.Hephaestus.VFX.Editor.Tests
{
    public class VFXAssetsSetupTests
    {
        // A throwaway folder; the setup only searches it, so the project's own VFX assets stay untouched.
        private const string Folder = "Assets/__HephaestusVFXTests";

        private static readonly string[] SearchFolders = { Folder };

        [SetUp]
        public void SetUp()
        {
            AssetDatabase.DeleteAsset(Folder);
        }

        [TearDown]
        public void TearDown()
        {
            AssetDatabase.DeleteAsset(Folder);
        }

        [Test]
        public void SetUp_EmptyFolder_CreatesAllAssets()
        {
            VFXAssetsSetup.SetUp(Folder, SearchFolders);

            Assert.IsNotNull(Load<VFXLibraryConstants>());
            Assert.IsNotNull(Load<VFXLibrary>());
            Assert.IsNotNull(Load<VFXManagerConfig>());
            Assert.IsNotNull(Load<HephaestusVFXManagerSOInstaller>());
        }

        [Test]
        public void SetUp_EmptyFolder_LinksAssets()
        {
            VFXAssetsSetup.SetUp(Folder, SearchFolders);

            var constants = Load<VFXLibraryConstants>();
            var library = Load<VFXLibrary>();
            var config = Load<VFXManagerConfig>();

            Assert.AreSame(constants, library.vfxLibraryConstants);
            Assert.AreSame(library, config.vfxLibrary);
            Assert.AreSame(config, GetInstallerConfig());
            Assert.AreEqual(Folder, constants.enumsPath);
        }

        [Test]
        public void SetUp_SecondRun_ChangesNothing()
        {
            VFXAssetsSetup.SetUp(Folder, SearchFolders);

            CollectionAssert.IsEmpty(VFXAssetsSetup.SetUp(Folder, SearchFolders));
        }

        [Test]
        public void SetUp_KeepsAssignedReferences()
        {
            VFXAssetsSetup.SetUp(Folder, SearchFolders);

            var otherConstants = ScriptableObject.CreateInstance<VFXLibraryConstants>();
            AssetDatabase.CreateAsset(otherConstants, $"{Folder}/OtherConstants.asset");

            var library = Load<VFXLibrary>();
            library.vfxLibraryConstants = otherConstants;
            EditorUtility.SetDirty(library);

            VFXAssetsSetup.SetUp(Folder, SearchFolders);

            Assert.AreSame(otherConstants, Load<VFXLibrary>().vfxLibraryConstants);
        }

        [Test]
        public void SetUp_KeepsCustomEnumsPath()
        {
            VFXAssetsSetup.SetUp(Folder, SearchFolders);

            var constants = Load<VFXLibraryConstants>();
            constants.enumsPath = "Assets/Scripts";
            EditorUtility.SetDirty(constants);

            VFXAssetsSetup.SetUp(Folder, SearchFolders);

            Assert.AreEqual("Assets/Scripts", Load<VFXLibraryConstants>().enumsPath);
        }

        [Test]
        public void SetUp_DeletedAssetAndEmptyLink_AreRestored()
        {
            VFXAssetsSetup.SetUp(Folder, SearchFolders);

            AssetDatabase.DeleteAsset(PathOf<HephaestusVFXManagerSOInstaller>());
            var config = Load<VFXManagerConfig>();
            config.vfxLibrary = null;
            EditorUtility.SetDirty(config);

            var report = VFXAssetsSetup.SetUp(Folder, SearchFolders);

            CollectionAssert.AreEquivalent(new[]
            {
                $"created {PathOf<HephaestusVFXManagerSOInstaller>()}",
                "linked VFXManagerConfig.vfxLibrary -> VFXLibrary",
                "linked HephaestusVFXManagerSOInstaller.vfxManagerConfig -> VFXManagerConfig"
            }, report);
            Assert.AreSame(Load<VFXLibrary>(), Load<VFXManagerConfig>().vfxLibrary);
            Assert.AreSame(Load<VFXManagerConfig>(), GetInstallerConfig());
        }

        [Test]
        public void SetUp_ReusesExistingAssetFromAnotherFolder()
        {
            var customFolder = $"{Folder}/Custom";
            AssetDatabase.CreateFolder("Assets", "__HephaestusVFXTests");
            AssetDatabase.CreateFolder(Folder, "Custom");

            var customLibrary = ScriptableObject.CreateInstance<VFXLibrary>();
            AssetDatabase.CreateAsset(customLibrary, $"{customFolder}/MyLibrary.asset");

            VFXAssetsSetup.SetUp(Folder, SearchFolders);

            Assert.IsNull(Load<VFXLibrary>(), "A second library was created instead of reusing the existing one.");
            Assert.AreSame(customLibrary, Load<VFXManagerConfig>().vfxLibrary);
            Assert.AreSame(Load<VFXLibraryConstants>(), customLibrary.vfxLibraryConstants);
        }

        [Test]
        public void SetUp_SearchFolderMissing_DoesNotSearchWholeProject()
        {
            // Nothing exists yet, so everything must be created in the test folder.
            var report = VFXAssetsSetup.SetUp(Folder, SearchFolders);

            Assert.AreEqual(4, report.FindAll(line => line.StartsWith("created ")).Count);
        }

        private static string PathOf<T>()
        {
            return $"{Folder}/{typeof(T).Name}.asset";
        }

        private static T Load<T>() where T : Object
        {
            return AssetDatabase.LoadAssetAtPath<T>(PathOf<T>());
        }

        private static Object GetInstallerConfig()
        {
            return new SerializedObject(Load<HephaestusVFXManagerSOInstaller>()).FindProperty("vfxManagerConfig").objectReferenceValue;
        }
    }
}
