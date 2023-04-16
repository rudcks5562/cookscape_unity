using UnityEngine;
using System;

public class ElevatorShaft : MonoBehaviour
{
    private ElevatorCabin cabinScp;
    public int FloorIndex { get; private set; }

    void Start ()
    {
	
	}

	void Update ()
    {
	
	}

    public void CallCabin()//todo call
    {
        cabinScp.Call(this);
    }

    internal void SetCabin(ElevatorCabin cabinScp)
    {
        this.cabinScp = cabinScp;
    }

    internal void SetFloorIndex(int floorIndex)
    {
        this.FloorIndex = floorIndex;
    }

    internal void OnOpenDoors()
    {
        throw new NotImplementedException();
    }

    internal void OnCloseDoors()
    {
        throw new NotImplementedException();
    }

    public void GotoFloor(int destinationFloor)//todo call
    {
        if (destinationFloor != FloorIndex)
        {
            cabinScp.GotoFloor(this, destinationFloor);
        }
    }
}
