using UnityEngine;

public class TurnLights : MonoBehaviour
{
    private bool lightsOn = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Light[] lights;
    void Start()
    {
        TurnOnLights();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TurnOnLights()
    {
        foreach (Light light in lights)
        {
            light.enabled = lightsOn;
        }
        lightsOn = !lightsOn;
    }
}
