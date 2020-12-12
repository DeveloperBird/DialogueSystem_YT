using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Event", menuName = "Event")]
public class EventBehaviour : ScriptableObject {

    public void TestEvent()
    {
        Debug.Log("Test event successful!");
        Destroy(References.instance.testGameObject);

        //any logic here
    }

    public void TestEvent02()
    {
        Debug.Log("Test event 02 successful!");
        //any logic here
    }


    public void TestEvent03()
    {
        Debug.Log("Test event 03 successful!");
        //any logic here
    }
}
