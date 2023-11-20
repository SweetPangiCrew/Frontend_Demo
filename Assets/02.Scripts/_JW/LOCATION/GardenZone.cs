using System.Collections.Generic;
using UnityEngine;


public class GardenZone : MonoBehaviour
{
    // Garden
    private Vector2 gardenMin = new Vector2(-13.03f,-7.51f);
    private Vector2 gardenMax = new Vector2(-7.27f,-3.23f);

    // Luna House
    private Vector2 lunaHouseMin = new Vector2(-13.91f,1.06f);
    private Vector2 lunaHouseMax = new Vector2(-7.15f,5.82f);



    private List<Vector2> NPCPosition = new List<Vector2>();
    public NPCData _NPCData;
 

    void Update()
    {
        if(_NPCData != null)
        {
            for(int i = 0; i < _NPCData.NPC.Count; i++)
            {
                Vector2 _NPCPosition = _NPCData.NPC[i]._location;

                if(IsInGarden(_NPCPosition))
                {
                    _NPCData.NPC[i]._locationName = "garden";
                    //Debug.Log(_NPCData.NPC[i] + "is in the garden!");
                }
                else if(IsInLunaHouse(_NPCPosition))
                {
                    _NPCData.NPC[i]._locationName = "Luna's house";
                    //Debug.Log(_NPCData.NPC[i] + "is in the Luna's House!");
                }
            }
         
        }
        
     }
    
    
    #region LUNAHOUSE
    bool IsInLunaHouse(Vector2 point)
    {
        return point.x >= lunaHouseMin.x && point.x <= lunaHouseMax.x
            && point.y >= lunaHouseMin.y && point.y <= lunaHouseMax.y;
    }
    #endregion 

    #region GARDEN
    bool IsInGarden(Vector2 point)
    {
        return point.x >= gardenMin.x && point.x <= gardenMax.x
            && point.y >= gardenMin.y && point.y <= gardenMax.y;
    }
    #endregion 
}
