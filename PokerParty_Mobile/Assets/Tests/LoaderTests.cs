using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

public class LoaderTests
{
    private GameObject loaderGameObject;
    private Loader loader;

    [SetUp]
    public void SetUp()
    {
        loaderGameObject = new GameObject("Loader");
        loader = loaderGameObject.AddComponent<Loader>();
        
        GameObject loadingIndicator = new GameObject("LoadingIndicator");
        loadingIndicator.SetActive(false);
        
        typeof(Loader).GetField("loader", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(loader, loadingIndicator);
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(loaderGameObject);
    }

    [Test]
    public void Loader_Instance_IsAssigned()
    {
        Assert.AreEqual(loader, Loader.instance);
    }

    [Test]
    public void StartLoading_ActivatesLoader()
    {
        loader.StartLoading();
        
        GameObject loadingIndicator = (GameObject)typeof(Loader).GetField("loader", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(loader);
        
        Assert.IsTrue(loadingIndicator.activeSelf);
    }

    [Test]
    public void StopLoading_DeactivatesLoader()
    {
        loader.StartLoading();
        
        loader.StopLoading();
        
        GameObject loadingIndicator = (GameObject)typeof(Loader).GetField("loader", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(loader);
        
        Assert.IsFalse(loadingIndicator.activeSelf);
    }
}