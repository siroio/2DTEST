using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class DicTest : MonoBehaviour
{
    private Dictionary<int, string> intDictionary = new Dictionary<int, string>();
    private Dictionary<TestEnum, string> enumDictionary = new Dictionary<TestEnum, string>();

    private const int TestSize = 1000;
    private const int SearchKey = 500;

    private void Start()
    {
        InitializeDictionaries();
        UnityEngine.Debug.Log("Starting performance test...");

        MeasurePerformance("Int Dictionary", () =>
        {
            string value;
            intDictionary.TryGetValue(UnityEngine.Random.Range(0, SearchKey), out value);
        });

        MeasurePerformance("Enum Dictionary", () =>
        {
            string value;
            enumDictionary.TryGetValue((TestEnum)UnityEngine.Random.Range(0, SearchKey), out value);
        });
    }

    private void InitializeDictionaries()
    {
        for (int i = 0; i < TestSize; i++)
        {
            intDictionary[i] = "Value " + i;
            enumDictionary[(TestEnum)i] = "Value " + i;
        }
    }

    private void MeasurePerformance(string testName, Action action)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        for (int i = 0; i < 1000000; i++)
        {
            action();
        }

        stopwatch.Stop();
        UnityEngine.Debug.Log($"{testName}: {stopwatch.ElapsedMilliseconds} ms");
    }

    public enum TestEnum
    {
        Value0 = 0,
        Value1 = 1,
        Value2 = 2,
        Value3 = 3,
        // 最大値まで自動キャスト可能
        Value999 = 999
    }
}
