using AirportCEOHistoryCEO.History;
using AirportCEOHistoryCEO.Models;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AirportCEOHistoryCEO.Patches;

[HarmonyPatch]
internal class TiledObjectsManagerPatch
{
     static List<ITileable> tileables = new List<ITileable>();   

    [HarmonyPatch(typeof(TiledObjectsManager), nameof(TiledObjectsManager.AddTileable))]
    [HarmonyPostfix]
    internal static void AddTileablePatch(ITileable tileable)
    {
        if (!SaveLoadGameDataController.loadComplete)
        {
            return;
        }

        tileables.Add(tileable);
    }

    [HarmonyPatch(typeof(TileMerger), nameof(TileMerger.MergeSurroundingTiles))]
    [HarmonyPostfix]
    internal static void TileMergerPatch(Vector3[] positions, ITileSource tileSource)
    {
        if (!SaveLoadGameDataController.loadComplete)
        {
            return;
        }


        var action = new TileableAction(tileables);
        HistoryManager.AddToHistory(action);
        Plugin.Logger.LogInfo($"Added TileableAction to history for tileable object");
    }
}

internal class  TileableAction: IHistoryAction
{
    List<ITileable> tileables { get; set; }
    public TileableAction(List<ITileable> tileables)
    {
        this.tileables = tileables;
    }

    public void Redo()
    {
        //Singleton<TiledObjectsManager>.Instance.AddTileable(tileable);
    }
    public void Undo()
    {
       Singleton<TiledObjectsManager>.Instance.RemoveTileables(tileables, true);
    }

}
