using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class ScytheHitbox : NetworkBehaviour
{
    // Start is called before the first frame update
    private Collider2D player;
    private List<Collider2D> Objs = new List<Collider2D>();
    [SerializeField] ToolsItems scytheState;


    // Update is called once per frame
    void Update()
    {
        if (Objs.Count!=0 && !scytheState.Swinging)
        {
            Objs.Clear();
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {

        if (!IsOwner || !scytheState.Scythe || !scytheState.Swinging) { return; }
        
        
        if (!Objs.Contains(collision) && collision.gameObject.CompareTag("PlantingUnit") && collision.gameObject.GetComponent<PlantedCorn>().ClientID==OwnerClientId && collision.gameObject.GetComponent<PlantedCorn>().Grow && scytheState.Corn<scytheState.MaxCorn)
        {
            Objs.Add(collision);
            PlantedCorn Unit= collision.gameObject.GetComponent<PlantedCorn>();
            Unit.GrowStateServerRpc(false);
            
            if (Unit.Corn )
            {
                scytheState.Corn+=3;
                scytheState.Seeds++;
                
                
            }
            if (Unit.GoldenCorn)
            {
                scytheState.Corn += 15;
                scytheState.GoldenSeeds++;

                
            }
            
            Unit.DisableGrowingServerRpc();
        }
        if (!Objs.Contains(collision) && collision.gameObject.CompareTag("Player") && collision.gameObject.GetComponent<NetworkObject>().OwnerClientId != OwnerClientId)
        {
            Objs.Add(collision);

            player = collision;

            player.gameObject.GetComponent<Health>().ChangeHPServerRpc(20);
            
        }
    }
    
}
