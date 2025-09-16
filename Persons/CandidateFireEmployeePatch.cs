using AirportCEOHistoryCEO.History;
using AirportCEOHistoryCEO.History.Models;
using AirportCEOModLoader.Core;
using HarmonyLib;


namespace AirportCEOHistoryCEO.Persons;

/// <summary>
/// This patch is for when you fire an employee using the selected employee view or all employees
/// </summary>
[HarmonyPatch(typeof(CandidateController), nameof(CandidateController.FireEmployee))]
internal static class CandidateFireEmployeePatch
{
    [HarmonyPostfix]
    public static void PostfixPatch(CandidateController __instance, EmployeeController employee)
    {
        // TODO: Check if employee is being hired or fired
        // and if this data is correctly set before postfix
        //if (!employee.EmployeeModel.isHired)
        //{

        //}
        var action = new FireEmployeeAction(employee);
        HistoryManager.AddToHistory(action);
        Plugin.Logger.LogInfo($"Added FireEmployeeAction to history");
    }
}


internal class FireEmployeeAction : IHistoryAction
{
    EmployeeController employeeController = Singleton<ObjectPoolController>.Instance.GetEmployee();


    public FireEmployeeAction(EmployeeController _employeeController)
    {
        try
        {
            employeeController.Initialize(_employeeController.employeeModel.employeeType);
            employeeController.employeeModel = _employeeController.employeeModel;
            employeeController.PersonModel = _employeeController.PersonModel;
        }

        catch (System.Exception e)
        {
            Plugin.Logger.LogError($"Error while initializing employee controller: {ExceptionUtils.ProccessException(e)}");
        }
    }

    public void Undo()
    {
        CandidateHelper.ReHireEmployee(employeeController);
    }

    public void Redo()
    {
        //Plugin.Logger.LogInfo($"Redoing FireEmployeeAction");
        //Singleton<CandidateController>.Instance.FireEmployee(employee);
    }
}