using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Level : MonoBehaviour {
    [SerializeField] private int ghostCharges;
    [SerializeField] private int queueSize;
    [SerializeField] private int moveSize = 1;
    [SerializeField] private int windSize = 2;
    [SerializeField] private int mimicSize = 3;
    [SerializeField] private int bootSize = 2;
    [SerializeField] private Vector3Int playerStartLoc;
    [SerializeField] private List<Vector3Int> mimicStartLocs;
    [SerializeField] private List<VariationType> variationTypes;

    public int getGhostCharges() { return ghostCharges; }
    public int getQueueSize() { return queueSize; }
    public int getMoveSize() { return moveSize; }
    public int getWindSize() { return windSize; }
    public int getMimicSize() { return mimicSize; }
    public int getBootSize() { return bootSize; }
    public Vector3Int getPlayerStartLoc() { return playerStartLoc; }
    public List<Vector3Int> getMimicStartLocs() { return mimicStartLocs; }
    public List<VariationType> getStartVariations() { return variationTypes; }

}