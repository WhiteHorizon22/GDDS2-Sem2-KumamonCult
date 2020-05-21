using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour {

    public Move Player;

    // For tracking the swipe.
    float SwipeDuration = 0f;
    Vector2 CumulativePosition;
    short FingerId = -1;
    public float SwipeThreshold = 0.05f;

    // For rendering trails when swiping.
    public GameObject TrailPrefab;
    GameObject TrailPrefabInstance;

    void Update() {
        if(Input.touchCount > 0) {
            if(Player == null) return;
            
            for(int i=0;i<Input.touchCount;i++) {

                Touch t = Input.GetTouch(i);
                if(t.fingerId > 0 && t.fingerId != FingerId) continue;

                // Check for swipes only if we are moving the finger.
                if(t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary) {
                    
                    // Create a new trailer if a new finger hits.
                    if(FingerId < 0) {
                        FingerId = (short)t.fingerId;
                        //TrailPrefabInstance = GameObject.Instantiate(TrailPrefab,Camera.main.ScreenToWorldPoint(t.position),Quaternion.Euler(0,0,0));
                    }

                    // If there is a trail, update its position.
                    //if(TrailPrefabInstance) TrailPrefabInstance.transform.position = Camera.main.ScreenToWorldPoint(t.position);

                    // Record swipe duration and position.
                    SwipeDuration += Time.deltaTime;
                    CumulativePosition += t.deltaPosition;

                    // If deltaPosition is zero, this means there is no swipe.
                    if(SwipeDuration > SwipeThreshold) {

                        //float angle = Vector2.Angle(Vector2.right,t.deltaPosition);
                        float angle = Mathf.Atan2(-t.deltaPosition.y,t.deltaPosition.x) * Mathf.Rad2Deg;

                        if(angle > -45f && angle < 45f) {
                            Player.LeftKey();
                        } else if(angle >= 45f && angle < 135f) {
                            Player.UpKey();
                        } else if((angle >= 135f && angle < 180f) || (angle > -180f && angle < -135f)) {
                            Player.RightKey();
                        } else {
                            Player.DownKey();
                        }

                        // Resets the swipe once its registered.
                        SwipeDuration = 0f;
                        CumulativePosition = Vector2.zero;
                        FingerId = -1;
                        /*
                        if(TrailPrefabInstance) {
                            GameObject.Destroy(TrailPrefabInstance,0.4f);
                            TrailPrefabInstance = null;
                        }*/
                    }
                } else {
                    // Resets the swipe once its registered.
                    SwipeDuration = 0f;
                    CumulativePosition = Vector2.zero;
                    FingerId = -1;
                    /*
                    if(TrailPrefabInstance) {
                        GameObject.Destroy(TrailPrefabInstance,0.4f);
                        TrailPrefabInstance = null;
                    }*/
                }
            }
        }
    }
}
