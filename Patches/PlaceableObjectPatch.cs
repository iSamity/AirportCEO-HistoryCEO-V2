using AirportCEOHistoryCEO.History;
using AirportCEOHistoryCEO.Models;
using HarmonyLib;

namespace AirportCEOHistoryCEO.Patches;

[HarmonyPatch(typeof(PlaceableObject), nameof(PlaceableObject.ChangeToPlaced))]
internal static class PlaceableObjectPatch
{
    public static void Postfix(PlaceableObject __instance, bool setFolder)
    {
        // This happens on load it will place some iems
        if(__instance.IsNotPlacedByPlayer || !SaveLoadGameDataController.loadComplete)
        {
            return;
        }

        var action = new PlaceableObjectAction(__instance);
        HistoryManager.AddToHistory(action);
        Plugin.Logger.LogInfo($"Added TestAction to history for construction object");

    }
}

internal class PlaceableObjectAction : IHistoryAction
{
    private PlaceableObject constructionObject { get; set; }

    public PlaceableObjectAction(PlaceableObject plo)
    {
        constructionObject = plo;
    }

    public void Undo()
    {
        Plugin.Logger.LogInfo($"Undoing TestAction for construction object");
        Singleton<DemolitionController>.Instance.DemolishObject(constructionObject);
    }

    public void Redo()
    {
        Plugin.Logger.LogInfo($"Redoing TestAction for construction object");
        //Singleton<DevelopmentActionController>.Instance.AddObjectToOperationsQueue(constructionObject, operationType);


        //Doesn't work as expected need to find another way to redo the placement
        //constructionObject.ChangeToPlaced(setFolder);
    }

}