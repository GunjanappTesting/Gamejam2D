using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Demo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Type personType = typeof(Person);
        object personInstance = Activator.CreateInstance(personType);

        // Print all the methods of the Person class
        MethodInfo methods = personType.GetMethod("A", BindingFlags.NonPublic | BindingFlags.Instance|BindingFlags.Public);
        methods?.Invoke(personInstance, null); 

        methods = personType.GetMethod("B");
        methods?.Invoke(personInstance, null); 

        methods = personType.GetMethod("C");
        methods?.Invoke(personInstance, null);
    }

}
public class Person
{
    public void A() => Debug.Log("Hello From Method A!");
    public void B() => Debug.Log("Hello From Method B!");
    public void C() => Debug.Log("Hello From Method C!");
}