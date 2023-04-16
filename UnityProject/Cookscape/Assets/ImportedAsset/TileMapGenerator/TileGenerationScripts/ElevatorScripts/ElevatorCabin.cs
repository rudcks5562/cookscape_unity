using System.Collections.Generic;
using UnityEngine;

public class ElevatorCabin : MonoBehaviour
{
    public float CabinSpeed = 3.0f;
    public float BusyTimeOut = 5.0f;//seconds

    private ElevatorShaft targetShaft = null;
    private ElevatorShaft lastTargetShaft = null;
    private List<ElevatorShaft> shaftList = new List<ElevatorShaft>();


    enum CabinState
    {
        free,
        busy,
        busyTimeout,
    }

    private CabinState cabinState = CabinState.free;
    private float busyTime;

    void Start()
    {
	
	}

    internal void AddShaft(ElevatorShaft shaft)
    {
        Debug.Assert(shaft.FloorIndex == shaftList.Count);

        shaftList.Add(shaft);
    }

	void Update()
    {
	    if (cabinState == CabinState.busy)
        {
            Debug.Assert(targetShaft != null);

            Vector3 curPos = gameObject.transform.position;
            Vector3 targetPos = targetShaft.gameObject.transform.position;

            Vector3 newPos = Vector3.Lerp(curPos, targetPos, CabinSpeed * Time.deltaTime);

            gameObject.transform.position = newPos;

            if (Vector3.Distance(newPos, targetPos) < 0.001f)
            {
                gameObject.transform.position = targetPos;
                busyTime = Time.timeSinceLevelLoad;
                cabinState = CabinState.busyTimeout;

                lastTargetShaft = targetShaft;
                targetShaft = null;

                lastTargetShaft.OnOpenDoors();
            }
            else
            {
                gameObject.transform.position = newPos;
            }
        }
        else
        {
            if (cabinState == CabinState.busyTimeout)
            {
                if (busyTime + BusyTimeOut < Time.timeSinceLevelLoad)
                {
                    cabinState = CabinState.free;
                    lastTargetShaft.OnCloseDoors();
                }
            }
        }
	}

    public void OpenCurrentDoorsFromInside()//todo call
    {
        busyTime = Time.timeSinceLevelLoad;
        cabinState = CabinState.busyTimeout;
        lastTargetShaft.OnOpenDoors();
    }

    internal void Call(ElevatorShaft callerShaft)
    {
        if (cabinState == CabinState.free)
        {
            cabinState = CabinState.busy;
            targetShaft = callerShaft;
        }
        else if (cabinState == CabinState.busyTimeout && callerShaft == lastTargetShaft)
        {
            OpenCurrentDoorsFromInside();
        }
    }

    internal void GotoFloor(ElevatorShaft commanderShaft, int destinationFloor)
    {
        bool applyGotoFloor = (cabinState == CabinState.free) || (commanderShaft == lastTargetShaft && cabinState == CabinState.busyTimeout);
        if (applyGotoFloor)
        {
            ElevatorShaft destinationShaft = shaftList[destinationFloor];

            cabinState = CabinState.busy;
            targetShaft = destinationShaft;
        }
    }
}
