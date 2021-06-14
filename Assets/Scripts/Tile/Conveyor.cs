using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyor : Tile4Way
{
    // protected override bool ShouldMerge(Tile t, int dirToCheck)
    // {
    //     return t != null;
    // }

    // protected override void RemoveNeighbor(int index)
    // {
    //     base.RemoveNeighbor(index);
    //     //conveyorFlow[5 - index] = FlowDir.Unset;
    //     //CalculateFlows();
    // }

    // protected override void AddNeighbor(Tile tile, int index)
    // {
    //     base.AddNeighbor(tile, index);
    //     //CalculateFlows();
    // }

    // void CalculateFlows()
    // {
    //     int[] counts = new int[3];
    //     for (int i = 0; i < 6; i++)
    //     {
    //         if (ShouldMerge(neighbors[i], 5 - i))
    //         {
    //             counts[(int)conveyorFlow[i] + 1]++;
    //         }
    //         else
    //         {
    //             conveyorFlow[i] = FlowDir.Unset;
    //         }
    //     }

    //     //Forcing outputs
    //     if (counts[1] == 1)
    //     {
    //         if (counts[0] == 0 && counts[2] > 0)
    //         {
    //             //there is one unset but no outputs for the inputs
    //             for (int i = 0; i < 6; i++)
    //             {
    //                 if (ShouldMerge(neighbors[i], 5 - i) && conveyorFlow[i] == FlowDir.Unset)
    //                 {
    //                     conveyorFlow[i] = FlowDir.Out;
    //                     neighbors[i].ForceConveyorFlow(5 - i, FlowDir.In);
    //                     break;
    //                 }
    //             }
    //         }
    //         if (counts[2] == 0 && counts[0] > 0)
    //         {
    //             //there is one unset but no inputs for the outputs
    //             for (int i = 0; i < 6; i++)
    //             {
    //                 if (ShouldMerge(neighbors[i], 5 - i) && conveyorFlow[i] == FlowDir.Unset)
    //                 {
    //                     conveyorFlow[i] = FlowDir.In;
    //                     neighbors[i].ForceConveyorFlow(5 - i, FlowDir.Out);
    //                     break;
    //                 }
    //             }
    //         }
    //     }

    //     //Unforcing outputs
    //     if (counts[0] == 0 && counts[2] == 0)
    //     {
    //         for (int i = 0; i < 6; i++)
    //         {
    //             if (ShouldMerge(neighbors[i], 5 - i))
    //             {
    //                 neighbors[i].UnforceConveyorFlow(5 - i);
    //             }
    //         }
    //     }
    // }
}
