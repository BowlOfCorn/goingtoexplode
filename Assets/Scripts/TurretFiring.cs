using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class TurretFiring : NetworkBehaviour
{
    [SerializeField] private GameObject turretHead, arrowPrefab;
    private List<Collider2D> players = new List<Collider2D>();
    private ToolsItems owner;
    private float fireTimer;
    [SerializeField] private float resetFireTimer;
    private int minDistanceIndex;
    private float minDistance;
    private void Start()
    {
        if (!IsOwner) { return; }
        fireTimer = 2f;
    }
    private void Update()
    {
        if (!IsOwner) { return; }
        fireTimer -= Time.deltaTime;
        
        if (players.Count>0)
        {
            
            TurretFire();
            if (fireTimer <= 0)
            {
                fireTimer = resetFireTimer;
                TurretFireServerRpc();
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        
        if(collision.gameObject.CompareTag("Player") && !players.Contains(collision) && collision.gameObject.GetComponent<NetworkObject>().OwnerClientId != OwnerClientId)
        {
            
            players.Add(collision);
        }
        
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && players.Contains(collision) && collision.gameObject.GetComponent<NetworkObject>().OwnerClientId != OwnerClientId)
        {

            players.Remove(collision);
        }
    }
    private void TurretFire()
    {
        minDistance = Vector2.Distance(turretHead.transform.position, players[0].transform.position);
        minDistanceIndex = 0;
        for(int i = 0; i < players.Count; i++)
        {
            if(Vector2.Distance(turretHead.transform.position, players[i].transform.position) < minDistance)
            {
                minDistance = Vector2.Distance(turretHead.transform.position, players[i].transform.position);
                minDistanceIndex = i;
            }
        }
        Vector2 rotate = turretHead.transform.position - players[minDistanceIndex].gameObject.transform.position;
        float rot = Mathf.Atan2(rotate.y, rotate.x) * Mathf.Rad2Deg;
        turretHead.transform.rotation = Quaternion.Euler(0, 0, rot + 180);
        
    }
    [ServerRpc(RequireOwnership =false)]
    private void TurretFireServerRpc()
    {
        
        NetworkObject instanceNetworkObject = Instantiate(arrowPrefab, turretHead.transform.position + turretHead.transform.right * 0.2f, turretHead.transform.rotation).GetComponent<NetworkObject>();
        instanceNetworkObject.SpawnWithOwnership(OwnerClientId);
    }
    public ToolsItems Owner
    {
        get { return owner; }
        set { owner = value; }
    }
}
