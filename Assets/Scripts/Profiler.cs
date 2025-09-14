using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Profiler : MonoBehaviour
{
    private class Entry
    {
        public string name;
        public float durationMs;
    }

    private List<Entry> results = new List<Entry>();

    private void Awake()
    {
        StartCoroutine(ProfileAll());
    }

    private IEnumerator ProfileAll()
    {
        // Wait one frame so all scripts exist
        yield return null;

        var allBehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);

        foreach (var mb in allBehaviours)
        {
            if (mb == this) continue;

            yield return ProfileMethod(mb, "Awake");
        }

        foreach (var mb in allBehaviours)
        {
            if (mb == this) continue;

            yield return ProfileMethod(mb, "OnEnable");
        }

        foreach (var mb in allBehaviours)
        {
            if (mb == this) continue;

            yield return ProfileMethod(mb, "Start");
        }

        Report();
    }

    private IEnumerator ProfileMethod(MonoBehaviour mb, string methodName)
    {
        var type = mb.GetType();
        var method = type.GetMethod(methodName,
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);

        if (method != null)
        {
            var sw = Stopwatch.StartNew();
            method.Invoke(mb, null);
            sw.Stop();

            results.Add(new Entry
            {
                name = $"{type.Name}.{methodName}",
                durationMs = sw.ElapsedMilliseconds
            });

            // Yield to avoid freezing editor/game
            yield return null;
        }
    }

    private void Report()
    {
        results.Sort((a, b) => b.durationMs.CompareTo(a.durationMs)); // slowest first

        UnityEngine.Debug.Log("===== General Startup Profiler Report =====");
        foreach (var e in results)
        {
            UnityEngine.Debug.Log($"{e.name}: {e.durationMs} ms");
        }
        UnityEngine.Debug.Log("==========================================");
    }
}
