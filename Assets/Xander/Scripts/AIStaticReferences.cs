using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStaticReferences : MonoBehaviour {

	public static Transform Player { get; private set; }
    public static Rigidbody PlayerRB { get; private set; }
    public static Transform RandomPlayerPart
    {
        get
        {
            bool removed = false;
            for(int i = playerPartsList.Count - 1; i >= 0; --i)
            {
                if (playerPartsList[i] == null)
                {
                    playerPartsList.RemoveAt(i);
                    removed = true;
                }
            }

            if (removed)
                allPlayerParts = playerPartsList.ToArray();

            return allPlayerParts[Random.Range(0, allPlayerParts.Length)];
        }
    }

    [SerializeField]
    private Transform player;
    [SerializeField]
    private Rigidbody playerRB;

    private static List<Transform> playerPartsList;
    private static Transform[] allPlayerParts;



    private void Awake()
    {
        Player = player;
        PlayerRB = playerRB;

        allPlayerParts = player.GetComponentsInChildren<Transform>();
        playerPartsList = new List<Transform>(allPlayerParts);
    }

}
