using AirportCEOHistoryCEO.History;
using AirportCEOHistoryCEO.Models;
using HarmonyLib;


namespace AirportCEOHistoryCEO.Patches;

/// <summary>
/// This patch is for when you hire an employee using the selected employee view or all employees
/// </summary>
[HarmonyPatch(typeof(CandidateController), nameof(CandidateController.HireEmployee))]
internal static class CandidateHireEmployeePatch
{
    [HarmonyPostfix]
    public static void PostfixPatch(CandidateController __instance, EmployeeController employee)
    {
        // TODO: Check if employee is being hired or fired
        // and if this data is correctly set before postfix
        //if (!employee.EmployeeModel.isHired)
        //{

        //}

        if(!__instance.CanHireEmployee(employee.EmployeeType))
        {
            return;
        }


        var action = new HireEmployeeAction(employee);
        HistoryManager.AddToHistory(action);
        Plugin.Logger.LogInfo($"Added HireEmployeeAction to history");
    }
}

internal class HireEmployeeAction : IHistoryAction
{
    private EmployeeController employee { get; set; }

    public HireEmployeeAction(EmployeeController _employee)
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

        Plugin.Logger.LogInfo($"Undoing HireEmployeeAction");
        Singleton<CandidateController>.Instance.FireEmployee(employee);
    }

    public void Redo()
    {
        Plugin.Logger.LogInfo($"Redoing HireEmployeeAction");
        Singleton<CandidateController>.Instance.HireEmployee(employee);
    }
}