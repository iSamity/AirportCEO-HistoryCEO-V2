using AirportCEOHistoryCEO.History.Models;
using AirportCEOModLoader.Core;
using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AirportCEOHistoryCEO.History;

public class HistoryManager
{
    private static Stack<IHistoryAction> undoStack = new Stack<IHistoryAction>();
    private static Stack<IHistoryAction> redoStack = new Stack<IHistoryAction>();

    public static void Log()
    {
        logger.LogInfo($"Undo Stack Count: {undoStack.Count}, Redo Stack Count: {redoStack.Count}");
        logger.LogInfo($"Undo Stack: {string.Join(", ", undoStack.Select(a => a.GetType().Name))}");
        logger.LogInfo($"Redo Stack: {string.Join(", ", redoStack.Select(a => a.GetType().Name))}");
    }

    public static void AddToHistory(IHistoryAction action)
    {
        undoStack.Push(action);
        redoStack.Clear(); // Clear redo stack when a new action is added   
    }
    public static void Undo()
    {
        if (undoStack.Count <= 0)
        {
            return;
        }

        try
        {
            var undoAction = undoStack.Pop();
            undoAction.Undo();
            redoStack.Push(undoAction);
        }
        catch (Exception e)
        {
            logger.LogError($"Error while undoing action: {e.Message}");
            ExceptionUtils.ProccessException(e);
        }
    }
    public static void Redo()
    {
        if (redoStack.Count <= 0)
        {
            return;
        }

        try
        {
            var redoAction = redoStack.Pop();
            redoAction.Redo();
            undoStack.Push(redoAction);
        }
        catch (Exception e)
        {
            ExceptionUtils.ProccessException(e);
            logger.LogError($"Error while redoing action: {e.Message}");
        }
    }

    static ManualLogSource logger => AirportCEOHistoryCEO.Plugin.Logger ?? throw new InvalidOperationException("Logger is not initialized.");
}
