using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour {

    public Collider minimapBoundingBox;
	void Start () {
        MinimapManager.Instance.UpdateMinimap(minimapBoundingBox);
	}
	
	void Update () {
		
	}
}
