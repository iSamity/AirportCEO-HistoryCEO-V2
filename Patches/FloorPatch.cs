using AirportCEOHistoryCEO.History;
using AirportCEOHistoryCEO.Models;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace AirportCEOHistoryCEO.Patches;

/// <summary>
/// This patch is for building a wall or change floor type
/// </summary>
[HarmonyPatch]
internal class AddTileToGridPatch
{
    static List<TileableWithOldQuality> tileables = new List<TileableWithOldQuality>();

    internal class AddTileState
    {
        public byte OldQuality;
    }

    [HarmonyPatch(typeof(PlaceableFloor), nameof(PlaceableFloor.AddTileToGrid))]
    [HarmonyPrefix]
    internal static void AddTileToGridPrefix(Vector4 position, ref AddTileState __state)
    {
        if (!SaveLoadGameDataController.loadComplete)
        {
            return;
        }


        if (Singleton<TiledObjectsManager>.Instance.TryGetTileable(position, out var tileable))
        {
            __state = new AddTileState
            {
                OldQuality = tileable.Quality
            };
        }
    }

    [HarmonyPatch(typeof(PlaceableFloor), nameof(PlaceableFloor.AddTileToGrid))]
    [HarmonyPostfix]
    internal static void AddTileToGridPostfix(Vector4 position, ref AddTileState __state)
    {
        if (!SaveLoadGameDataController.loadComplete)
        {
            return;
        }

        if (Singleton<TiledObjectsManager>.Instance.TryGetTileable(position, out var tileable))
        {
            tileables.Add(new TileableWithOldQuality(tileable, __state.OldQuality));
        }
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


        var action = new FloorAction(tileables);

        tileables.Clear();

        HistoryManager.AddToHistory(action);
        Plugin.Logger.LogInfo($"Added TileableAction to history for tileable object");
    }

}

internal class FloorAction : IHistoryAction
{
    List<TileableWithOldQuality> tileables { get; set; }
    public FloorAction(List<TileableWithOldQuality> _tileables)
    {
        tileables = new List<TileableWithOldQuality>();
        tileables.AddRange(_tileables);
    }

    public void Undo()
    {
        tileables.ForEach(t =>
        {
            t.tileable.Quality = t.oldQuality;
        });

        // This refreshes the tiles after setting the old quality back
        TileMerger.MergeTileablesByType(Enums.TileType.Floor);
    }

    public void Redo()
    {
        //Singleton<TiledObjectsManager>.Instance.AddTileable(tileable);
    }
}


internal class TileableWithOldQuality
{
    public ITileable tileable { get; set; }
    public byte oldQuality { get; set; }
    public TileableWithOldQuality(ITileable _tileable, byte _oldQuality)
    {
        tileable = _tileable;
        oldQuality = _oldQuality;
    }
}