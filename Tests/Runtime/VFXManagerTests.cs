using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Zenject;
using Object = UnityEngine.Object;

namespace WTFGames.Hephaestus.VFX.Tests
{
    public class VFXManagerTests
    {
        private const string PrefabName = "TestVFXPrefab";

        private DiContainer _container;
        private VFXManagerConfig _config;
        private VFXLibrary _library;
        private GameObject _prefab;
        private IVFXManager _manager;

        [SetUp]
        public void SetUp()
        {
            _prefab = new GameObject(PrefabName);

            _library = ScriptableObject.CreateInstance<VFXLibrary>();
            _library.vfxList.Add(new VFXNamePair { vfxType = (int)TestVFXType.SPARK, vfxPrefab = _prefab });

            _config = ScriptableObject.CreateInstance<VFXManagerConfig>();
            _config.vfxLibrary = _library;

            _container = new DiContainer();
            _container.BindInstance(_config);
            HephaestusVFXManagerInstaller.Install(_container);

            _manager = _container.Resolve<IVFXManager>();
        }

        [TearDown]
        public void TearDown()
        {
            _container.Resolve<IDisposable>().Dispose();

            // Destroy immediately so nothing leaks into the next test's counts.
            foreach (var gameObject in FindSceneObjects<Transform>().Select(transform => transform.gameObject)
                         .Where(gameObject => gameObject.name.StartsWith(PrefabName)))
            {
                Object.DestroyImmediate(gameObject);
            }

            foreach (var handler in FindSceneObjects<VFXManagerHandler>())
            {
                Object.DestroyImmediate(handler.gameObject);
            }

            Object.DestroyImmediate(_library);
            Object.DestroyImmediate(_config);
        }

        [Test]
        public void Installer_BindsOneManagerForAllInterfaces()
        {
            Assert.IsInstanceOf<VFXManager>(_manager);
            Assert.AreSame(_manager, _container.Resolve<IInitializable>());
            Assert.AreSame(_manager, _container.Resolve<IDisposable>());
        }

        [Test]
        public void Initialize_CreatesHandlerThatSurvivesSceneLoads()
        {
            Initialize();

            var handlers = FindSceneObjects<VFXManagerHandler>();

            Assert.AreEqual(1, handlers.Length);
            Assert.AreEqual("DontDestroyOnLoad", handlers[0].gameObject.scene.name);
        }

        [Test]
        public void Initialize_CalledTwice_KeepsSingleHandler()
        {
            Initialize();
            Initialize();

            Assert.AreEqual(1, FindSceneObjects<VFXManagerHandler>().Length);
        }

        [UnityTest]
        public IEnumerator Dispose_DestroysHandler()
        {
            Initialize();

            _container.Resolve<IDisposable>().Dispose();
            yield return null;

            Assert.AreEqual(0, FindSceneObjects<VFXManagerHandler>().Length);
        }

        [Test]
        public void PlayVFX_BeforeInitialize_LogsErrorAndSpawnsNothing()
        {
            LogAssert.Expect(LogType.Error, new Regex("not initialized"));

            _manager.PlayVFX(TestVFXType.SPARK, Vector3.zero);

            Assert.AreEqual(0, GetInstances().Length);
        }

        [Test]
        public void PlayVFX_SpawnsPrefabAtWorldPosition()
        {
            Initialize();

            _manager.PlayVFX(TestVFXType.SPARK, new Vector3(1f, 2f, 3f));

            var instances = GetInstances();
            Assert.AreEqual(1, instances.Length);
            Assert.AreEqual(new Vector3(1f, 2f, 3f), instances[0].transform.position);
            Assert.IsNull(instances[0].transform.parent);
        }

        [Test]
        public void PlayVFX_WithParent_ParentsInstanceAndKeepsWorldPosition()
        {
            Initialize();

            var parent = new GameObject(PrefabName + "Parent").transform;
            parent.position = new Vector3(10f, 0f, 0f);

            _manager.PlayVFX(TestVFXType.SPARK, new Vector3(1f, 2f, 3f), parent);

            var instance = GetInstances().Single();
            Assert.AreSame(parent, instance.transform.parent);
            Assert.AreEqual(new Vector3(1f, 2f, 3f), instance.transform.position);
        }

        [Test]
        public void PlayVFX_KeepsPrefabRotation()
        {
            _prefab.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
            Initialize();

            _manager.PlayVFX(TestVFXType.SPARK, Vector3.zero);

            Assert.That(Quaternion.Angle(_prefab.transform.rotation, GetInstances().Single().transform.rotation), Is.LessThan(0.01f));
        }

        [Test]
        public void PlayVFX_UnmappedKey_LogsWarningAndSpawnsNothing()
        {
            Initialize();
            LogAssert.Expect(LogType.Warning, new Regex("No prefab is mapped to MISSING"));

            _manager.PlayVFX(TestVFXType.MISSING, Vector3.zero);

            Assert.AreEqual(0, GetInstances().Length);
        }

        [UnityTest]
        public IEnumerator PlayVFX_NonLoopingParticles_DestroysInstanceAfterLifetime()
        {
            AddParticleSystem(loop: false, duration: 0.1f, lifetime: 0.1f);
            Initialize();

            _manager.PlayVFX(TestVFXType.SPARK, Vector3.zero);
            yield return null;
            Assert.AreEqual(1, GetInstances().Length, "The instance is destroyed too early.");

            yield return new WaitForSeconds(0.5f);
            Assert.AreEqual(0, GetInstances().Length, "The instance outlived its particles.");
        }

        [UnityTest]
        public IEnumerator PlayVFX_LoopingParticles_KeepsInstance()
        {
            AddParticleSystem(loop: true, duration: 0.1f, lifetime: 0.1f);
            Initialize();

            _manager.PlayVFX(TestVFXType.SPARK, Vector3.zero);
            yield return new WaitForSeconds(0.5f);

            Assert.AreEqual(1, GetInstances().Length);
        }

        [UnityTest]
        public IEnumerator PlayVFX_WithoutParticles_KeepsInstance()
        {
            Initialize();

            _manager.PlayVFX(TestVFXType.SPARK, Vector3.zero);
            yield return new WaitForSeconds(0.2f);

            Assert.AreEqual(1, GetInstances().Length);
        }

        private void Initialize()
        {
            _container.Resolve<IInitializable>().Initialize();
        }

        private void AddParticleSystem(bool loop, float duration, float lifetime)
        {
            var particleSystem = _prefab.AddComponent<ParticleSystem>();

            // The duration can only be changed while the system is stopped.
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            var main = particleSystem.main;
            main.loop = loop;
            main.duration = duration;
            main.startLifetime = lifetime;
            main.startDelay = 0f;
        }

        private static GameObject[] GetInstances()
        {
            return FindSceneObjects<Transform>()
                .Select(transform => transform.gameObject)
                .Where(gameObject => gameObject.name == PrefabName + "(Clone)")
                .ToArray();
        }

        private static T[] FindSceneObjects<T>() where T : Component
        {
            // Unlike FindObjectsOfType, this also finds objects in the DontDestroyOnLoad scene on every Unity version.
            return Resources.FindObjectsOfTypeAll<T>()
                .Where(component => component.gameObject.scene.IsValid())
                .ToArray();
        }
    }
}
