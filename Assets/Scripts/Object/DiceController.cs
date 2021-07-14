using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceController : MonoBehaviour
{
    private new Rigidbody rigidbody;
    private float recentStopTime = 0.0f;
    public bool isRolling { get; private set; }
    public int number { get; private set; }

    private TableManager tableManager;

    private void Awake() {
        rigidbody = GetComponent<Rigidbody>();
        tableManager = TableManager.getInstance();
        
        ResetProperty();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if(isRolling) {
            float mag = rigidbody.velocity.magnitude + rigidbody.angularVelocity.magnitude;
            if(mag == 0.0f) {
                float currentTime = Time.fixedTime;
                float elapsedStopTime = currentTime - recentStopTime;

                if(0.2f <= elapsedStopTime) {
                    isRolling = false;
                    CheckNumber();

                    if(tableManager != null) {
                        tableManager.DecreaseRollingCount();
                    }
                }
            }
            else {
                recentStopTime = Time.fixedTime;
            }
        }
    }

    private void OnEnable() {
        ResetProperty();
    }

    public void Roll() {
        if(isRolling) {
            return;
        }

        isRolling = true;
        recentStopTime = Time.fixedTime;
        rigidbody.AddTorque(new Vector3(Random.Range(-300, 300), 0, Random.Range(-300, 300)));
        rigidbody.AddForce(Vector3.up * 20.0f, ForceMode.Impulse);

        if(tableManager != null) {
            tableManager.IncreaseRollingCount();
        }
    }

    private void CheckNumber() {
        Vector3 rayPositon = transform.position + new Vector3(0, 2.0f, 0);
        Vector3 rayDirection = Vector3.down;
        float rayDistance = 2.0f;
        RaycastHit hit;

        bool ret = Physics.Raycast(rayPositon, rayDirection, out hit, rayDistance);

        if(ret) {
            string faceName = hit.collider.name;
            switch(faceName) {
                case "Face1": { number = 1; break; }
                case "Face2": { number = 2; break; }
                case "Face3": { number = 3; break; }
                case "Face4": { number = 4; break; }
                case "Face5": { number = 5; break; }
                case "Face6": { number = 6; break; }
                default: {
                    Debug.Log("Check Failure : " + this.name.ToString() + " -> " + faceName.ToString());
                    break;
                }
            }
        }
    }

    private void ResetProperty() {
         if(rigidbody != null) {
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }
        transform.rotation = Quaternion.identity;
        isRolling = false;
        number = 0;
    }
}
