using AirportCEOHistoryCEO.History;
using AirportCEOHistoryCEO.Models;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace AirportCEOHistoryCEO.Patches;

/// <summary>
/// This patch is for building tileables like foundations or taxiways
/// </summary>
[HarmonyPatch]
internal class AddTileablePatch
{
    static List<ITileable> tileables = new List<ITileable>();

    [HarmonyPatch(typeof(TiledObjectsManager), nameof(TiledObjectsManager.AddTileable))]
    [HarmonyPostfix]
    internal static void AddTileablePostfix(ITileable tileable)
    {
        if (!SaveLoadGameDataController.loadComplete)
        {
            return;
        }

        tileables.Add(tileable);
    }

    [HarmonyPatch(typeof(TileMerger), nameof(TileMerger.MergeSurroundingTiles))]
    [HarmonyPostfix]
    internal static void TileMergerPostfix(Vector3[] positions, ITileSource tileSource)
    {
        if (!SaveLoadGameDataController.loadComplete)
        {
            return;
        }

        if (tileables.Count == 0)
        {
            return;
        }

        var action = new TileableAction(tileables);

        tileables.Clear();

        HistoryManager.AddToHistory(action);
        Plugin.Logger.LogInfo($"Added TileableAction to history for tileable object");
    }
}

internal class TileableAction : IHistoryAction
{
    List<ITileable> tileables { get; set; }
    public TileableAction(List<ITileable> _tileables)
    {
        tileables = new List<ITileable>();
        tileables.AddRange(_tileables);
    }

    public void Undo()
    {
        Singleton<TiledObjectsManager>.Instance.RemoveTileables(tileables, true);
    }

    public void Redo()
    {
        //Singleton<TiledObjectsManager>.Instance.AddTileable(tileable);
    }
}
