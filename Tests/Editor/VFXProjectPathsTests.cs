using System.IO;
using NUnit.Framework;

namespace WTFGames.Hephaestus.VFX.Editor.Tests
{
    public class VFXProjectPathsTests
    {
        private const string ProjectRoot = "/Projects/Game";

        [Test]
        public void ToAbsolute_RelativePath_IsResolvedAgainstProjectRoot()
        {
            Assert.AreEqual(Path.Combine(ProjectRoot, "Assets/Scripts"), VFXProjectPaths.ToAbsolute("Assets/Scripts", ProjectRoot));
        }

        [Test]
        public void ToAbsolute_AbsolutePath_IsKept()
        {
            Assert.AreEqual("/Shared/Enums", VFXProjectPaths.ToAbsolute("/Shared/Enums", ProjectRoot));
        }

        [TestCase("")]
        [TestCase(null)]
        public void ToAbsolute_EmptyPath_ReturnsEmpty(string path)
        {
            Assert.AreEqual(string.Empty, VFXProjectPaths.ToAbsolute(path, ProjectRoot));
        }

        [Test]
        public void ToProjectRelative_FolderInsideProject_IsRelative()
        {
            Assert.AreEqual("Assets/Scripts/VFX", VFXProjectPaths.ToProjectRelative("/Projects/Game/Assets/Scripts/VFX", ProjectRoot));
        }

        [Test]
        public void ToProjectRelative_ProjectRootWithTrailingSlash_IsHandled()
        {
            Assert.AreEqual("Assets", VFXProjectPaths.ToProjectRelative("/Projects/Game/Assets", ProjectRoot + "/"));
        }

        [Test]
        public void ToProjectRelative_WindowsSeparators_AreNormalized()
        {
            Assert.AreEqual("Assets/Scripts", VFXProjectPaths.ToProjectRelative(@"C:\Game\Assets\Scripts", @"C:\Game"));
        }

        [Test]
        public void ToProjectRelative_FolderOutsideProject_StaysAbsolute()
        {
            Assert.AreEqual("/Shared/Enums", VFXProjectPaths.ToProjectRelative("/Shared/Enums", ProjectRoot));
        }

        [Test]
        public void ToProjectRelative_SiblingWithSamePrefix_StaysAbsolute()
        {
            // "/Projects/GameTools" starts with "/Projects/Game" but is not inside it.
            Assert.AreEqual("/Projects/GameTools/Enums", VFXProjectPaths.ToProjectRelative("/Projects/GameTools/Enums", ProjectRoot));
        }

        [Test]
        public void GetClosestExistingFolder_ExistingFolder_ReturnsIt()
        {
            var folder = Path.GetTempPath();

            Assert.AreEqual(folder, VFXProjectPaths.GetClosestExistingFolder(folder, "fallback"));
        }

        [Test]
        public void GetClosestExistingFolder_MissingFolder_ReturnsClosestParent()
        {
            var parent = Path.GetFullPath(Path.GetTempPath()).TrimEnd(Path.DirectorySeparatorChar);
            var missing = Path.Combine(parent, "HephaestusVFXMissing", "Nested");

            Assert.AreEqual(parent, VFXProjectPaths.GetClosestExistingFolder(missing, "fallback"));
        }

        [TestCase("")]
        [TestCase(null)]
        public void GetClosestExistingFolder_EmptyPath_ReturnsFallback(string path)
        {
            Assert.AreEqual("fallback", VFXProjectPaths.GetClosestExistingFolder(path, "fallback"));
        }
    }
}
