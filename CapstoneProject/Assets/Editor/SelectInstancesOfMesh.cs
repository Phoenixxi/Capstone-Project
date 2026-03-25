using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class SelectInstancesOfMeshes
{
    [MenuItem("Tools/Select All Instances Of Selected Meshes")]
    static void SelectAllInstancesOfSelectedMeshes()
    {
        // Grab all selected Mesh assets from the Project window
        Mesh[] selectedMeshes = Selection.objects.OfType<Mesh>().ToArray();

        if (selectedMeshes.Length == 0)
        {
            Debug.LogWarning("Select one or more Mesh assets in the Project window.");
            return;
        }

        HashSet<Mesh> meshSet = new HashSet<Mesh>(selectedMeshes);
        HashSet<GameObject> matches = new HashSet<GameObject>();

        // Regular MeshFilter objects
        MeshFilter[] allFilters = Object.FindObjectsByType<MeshFilter>(FindObjectsSortMode.None);
        foreach (MeshFilter mf in allFilters)
        {
            if (mf.sharedMesh != null && meshSet.Contains(mf.sharedMesh))
            {
                matches.Add(mf.gameObject);
            }
        }

        // Skinned meshes too, just in case
        SkinnedMeshRenderer[] allSkinned = Object.FindObjectsByType<SkinnedMeshRenderer>(FindObjectsSortMode.None);
        foreach (SkinnedMeshRenderer smr in allSkinned)
        {
            if (smr.sharedMesh != null && meshSet.Contains(smr.sharedMesh))
            {
                matches.Add(smr.gameObject);
            }
        }

        if (matches.Count == 0)
        {
            Debug.Log("No instances of the selected meshes were found in the open scene(s).");
            return;
        }

        Selection.objects = matches.ToArray();
        Debug.Log($"Selected {matches.Count} scene object(s) using {selectedMeshes.Length} mesh asset(s).");
    }
}