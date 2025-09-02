using AirportCEOHistoryCEO.History;
using AirportCEOHistoryCEO.Models;
using HarmonyLib;
using System;


namespace AirportCEOHistoryCEO.Patches;

/// <summary>
/// This patch is for when you quick hire/fire an employee from the employee container UI 
/// Not when looking at the actual employee 
/// </summary>
[HarmonyPatch(typeof(EmployeeContainerUI), nameof(EmployeeContainerUI.QuickHireFireEmployee))]
internal static class EmployeeContainerQuickHireFirePatch
{
    public static void Postfix(EmployeeController employee, Action refreshPanel)
    {
        // TODO: Check if employee is being hired or fired
        // and if this data is correctly set before postfix
        //if (!employee.EmployeeModel.isHired)
        //{
            
        //}

        if(!Singleton<CandidateController>.Instance.CanHireEmployee(employee.EmployeeType))
        {
            return;
        }


        var action = new EmployeeQuickHireFireAction(employee);
        HistoryManager.AddToHistory(action);
        Plugin.Logger.LogInfo($"Added EmployeeQuickHireFireAction to history");
    }
}

internal class EmployeeQuickHireFireAction : IHistoryAction
{
    private EmployeeController employee { get; set; }

    public EmployeeQuickHireFireAction(EmployeeController _employee)
    {
        employee = _employee;
    }

    public void Undo()
    {
        if (!employee)
        {
            return;
        }

        // If the user manually fired the employee we don't need to do it anymore
        if (!employee.EmployeeModel.isHired)
        {
            return;
        }

        Plugin.Logger.LogInfo($"Undoing EmployeeQuickHireFireAction");
        Singleton<CandidateController>.Instance.FireEmployee(employee);
    }

    public void Redo()
    {
        Plugin.Logger.LogInfo($"Redoing EmployeeQuickHireFireAction");
        Singleton<CandidateController>.Instance.HireEmployee(employee);

    }
}