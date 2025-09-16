using Nodes;

namespace AirportCEOHistoryCEO.History.Candidate;

internal static class CandidateHelper
{
    internal static void ReHireEmployee(EmployeeController employeeController)
    {
        if (!employeeController)
        {
            return;
        }

        if (employeeController.EmployeeModel.isHired)
        {
            // TODO check if this can even happen
            Plugin.Logger.LogError($"Employee is still hired for some reason");
            return;
        }

        var allPersons = Singleton<AirportController>.Instance.allPersons;

        if (allPersons == null)
        {
            return;
        }

        var candidateController = Singleton<CandidateController>.Instance;

        if (!candidateController)
        {
            return;
        }


        if (allPersons.ContainsKey(employeeController.employeeModel.ReferenceID))
        {
            HireEmployee(employeeController);
        }
        else
        {
            EmployeeController newEmployee = Singleton<ObjectPoolController>.Instance.GetEmployee();
            newEmployee.Initialize(employeeController.EmployeeType);

            SetExistingEmployee(newEmployee, employeeController);


            newEmployee.EmployeeModel.isAtAirport = false;
            newEmployee.EmployeeModel.isHired = false;
            Singleton<AirportController>.Instance.AddPersonToList(newEmployee);
            HireEmployee(newEmployee);
        }

        var instance = Singleton<EmployeePanelUI>.Instance;

        if (instance)
        {
            // Refreshes staff view
            instance.GenerateEmployeeContainers();
        }
    }

    private static void HireEmployee(EmployeeController employee)
    {
        var candidateController = Singleton<CandidateController>.Instance;
        candidateController.HireEmployee(employee);

        // This renders the employee in the employee container field
        employee.employeeModel.isFired = false;
        employee.PersonModel.isLeaving = false;
    }

    private static void SetExistingEmployee(EmployeeController newEmployee, EmployeeController existingEmployee)
    {
        var neem = newEmployee.EmployeeModel;
        var eeem = existingEmployee.EmployeeModel;

        // This fixes the issue that the avatar of the employee looks different after rehiring 
        neem.personApperance = eeem.personApperance;

        // If this is fails it wont render the person
        // Also makes it so the outside bus will try to get this person 
        neem.hasDeboarded = true;

        neem.Position = eeem.Position;
        neem.Rotation = eeem.Rotation;
        neem.floor = eeem.floor;
        neem.reference = eeem.reference;
        neem.personType = eeem.personType;
        neem.wealthClass = eeem.wealthClass;
        neem.gender = eeem.gender;
        neem.firstName = eeem.firstName;
        neem.lastName = eeem.lastName;
        neem.originCity = eeem.originCity;
        neem.originCountry = eeem.originCountry;
        neem.birthDate = eeem.birthDate;
        neem.age = eeem.age;
        neem.weight = eeem.weight;
        neem.height = eeem.height;
        neem.needs = eeem.needs;
        neem.assetHandler = eeem.assetHandler;
        neem.Surroundings = eeem.surroundings;
        neem.isOccupied = eeem.isOccupied;
        neem.isCarryingTrash = eeem.isCarryingTrash;
        neem.permissions = eeem.permissions;
        neem.isAtAirport = eeem.isAtAirport;
        neem.isAllowedToPassSecurity = eeem.isAllowedToPassSecurity;
        neem.hasPassedSecurity = eeem.hasPassedSecurity;
        neem.isBoardingTransport = eeem.isBoardingTransport;
        neem.isOnTransferringVehicle = eeem.isOnTransferringVehicle;
        neem.shouldLeaveAirport = eeem.shouldLeaveAirport;
        neem.isLeaving = eeem.isLeaving;
        neem.isVisitor = eeem.isVisitor;
        neem.isPaused = eeem.isPaused;
        neem.timeArrivedAtAirport = eeem.timeArrivedAtAirport;
        neem.timeStartedWaitingForActivity = eeem.timeStartedWaitingForActivity;
        neem.currentTransitStructureReferenceID = eeem.currentTransitStructureReferenceID;
        neem.currentInteractionItemReferenceID = eeem.currentInteractionItemReferenceID;
        neem.persistentDeskReferenceID = eeem.persistentDeskReferenceID;
        neem.currentInteractionItemGroupDependency = eeem.currentInteractionItemGroupDependency;
        neem.currentInteractionPointIndex = eeem.currentInteractionPointIndex;
        neem.currentRoomReferenceID = eeem.currentRoomReferenceID;
        neem.currentSecureZoneExitReferenceID = eeem.currentSecureZoneExitReferenceID;
        neem.currentGenericZoneType = eeem.currentGenericZoneType;
        neem.currentSpecificZoneType = eeem.currentSpecificZoneType;
        neem.currentRoomType = eeem.currentRoomType;
        neem.passedSecurityCheckpointReferenceID = eeem.passedSecurityCheckpointReferenceID;
        neem.assignedVehicleParkingSpaceIndex = eeem.assignedVehicleParkingSpaceIndex;
        neem.structureTypeToIgnore = eeem.structureTypeToIgnore;
        neem.currentQueuePointIndex = eeem.currentQueuePointIndex;
        neem.isQueueing = eeem.isQueueing;
        neem.isMovingThroughStaticQueue = eeem.isMovingThroughStaticQueue;
        neem.nbrOfNodesWalked = eeem.nbrOfNodesWalked;
        neem.shouldProcessObjectThroughSecurity = eeem.shouldProcessObjectThroughSecurity;
        neem.isTransitioning = eeem.isTransitioning;
        if (eeem.originCity == null)
        {
            neem.originCity = "";
        }
        if (eeem.originCountry == null)
        {
            neem.originCountry = "";
        }
        neem.spawnedFromSerializer = false;

        if (eeem.isTransitioning)
        {
            Walkalator output2;
            if (SingletonNonDestroy<GridController>.Instance.TryGetItemFromPosition<IFloorTransition>(eeem.FloorPosition, out var output))
            {
                if (output.TransitionData != null)
                {
                    PersonNode randomLowerTransitionNode = output.TransitionData.GetRandomLowerTransitionNode();
                    if (randomLowerTransitionNode != null)
                    {
                        neem.Position = randomLowerTransitionNode.worldPosition;
                        neem.floor = randomLowerTransitionNode.Floor;
                    }
                }
            }
            else if (SingletonNonDestroy<GridController>.Instance.TryGetItemFromPosition<Walkalator>(eeem.FloorPosition, out output2))
            {
                neem.Position = output2.StartPosition;
            }
            neem.isTransitioning = false;
        }


        neem.skill = eeem.skill;
        neem.currentActivity = eeem.currentActivity;
        neem.recentlyFailedActivities = eeem.recentlyFailedActivities;
        neem.currentActionDescriptionList = eeem.currentActionDescriptionList;
        neem.currentJobTaskReferenceID = eeem.currentJobTaskReferenceID;
        neem.currentConstructionMaterialReferenceID = eeem.currentConstructionMaterialReferenceID;
        neem.isFired = eeem.isFired;
        neem.isHired = eeem.isHired;
        neem.isWorking = eeem.isWorking;
        neem.isPerformingJobTask = eeem.isPerformingJobTask;
        neem.isOnStrike = eeem.isOnStrike;
        neem.trainedIndex = eeem.trainedIndex;
        neem.isAtStaffRoom = eeem.isAtStaffRoom;
        neem.isAtJobTaskLocation = eeem.isAtJobTaskLocation;
        neem.assignedTerminalNbr = eeem.assignedTerminalNbr;
        neem.IDNbr = eeem.IDNbr;
        neem.salary = eeem.salary;
        neem.educationSeed = eeem.educationSeed;
        neem.degreeSeed = eeem.degreeSeed;
        neem.GPA = eeem.GPA;
        neem.experienceSeed = eeem.experienceSeed;
        neem.yearsOfExperience = eeem.yearsOfExperience;
        neem.informationSeed = eeem.informationSeed;
        neem.hiringDate = eeem.hiringDate;
    }
}
