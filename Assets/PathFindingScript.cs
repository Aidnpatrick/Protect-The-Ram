using System.Collections.Generic;
using UnityEngine;

public class PathFindingScript : MonoBehaviour
{
    public GameControlScript gameControlScript;


    GameObject FindTile(int tileID)
    {
        foreach(GameObject tile in gameControlScript.tiles)
            if(tile.GetComponent<TileScript>().id == tileID) return tile;
        return null;
    }

    int GetTileID(GameObject tile)
    {
        return tile.GetComponent<TileScript>().id;
    }

    List<int> ScanTiles(GameObject currentLocation)
    {
        List<int> closeTileID = new List<int>();
        foreach(GameObject tile in gameControlScript.tiles)
        {
            if(Vector3.Distance(tile.transform.position, currentLocation.transform.position) < 0.5f)
            {
                if(!tile.GetComponent<TileScript>().isBlocked)
                    closeTileID.Add(GetTileID(tile));
            }
        }
        return closeTileID;
    } 

    public List<GameObject> PathFind(GameObject start, GameObject end)
    {
        Dictionary<float, (float ,float, float)> data = new Dictionary<float, (float, float, float)>();

        //making the costs
        //index = tileID
        int index = 0;
        foreach(GameObject tile in gameControlScript.tiles)
        {
            //tile ID, G cost, H cost, F cost
            float G_Cost = Vector3.Distance(tile.transform.position, start.transform.position);
            float H_Cost = Vector3.Distance(tile.transform.position, end.transform.position);
            float F_Cost = G_Cost + H_Cost;
            data[index] = (G_Cost, H_Cost, F_Cost);
            index++;
        }
        List<GameObject> path = new List<GameObject>
        {
            start
        };
        
        int currentSpot = 0;
        while(true)
        {
            //scans close tiles (tile ids)
            List<int> CloseTileIDs = ScanTiles(path[currentSpot]);

            //chooses smallest F cost
            
            float smallest_F_Cost = Mathf.Infinity;
            int closestID = 0;
            for(int i = 1; i < CloseTileIDs.Count; i++)
            {
                if(data[CloseTileIDs[i]].Item3 < smallest_F_Cost)
                {
                    smallest_F_Cost = data[CloseTileIDs[i]].Item3; 
                    closestID = CloseTileIDs[i];
                }
            }
            // add new path
            path.Add(FindTile(closestID));

            if(data[closestID].Item2 == 0)
                break;

            currentSpot++;
        }
        return path;        
    }

    
}
