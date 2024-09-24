using System.Collections.Generic;

public class AnimationData
{
    // Dictionary to store animation names for each layer
    private readonly Dictionary<int, List<string>> _animationLayers;

    public AnimationData()
    {
        _animationLayers = new Dictionary<int, List<string>>
        {
            { 0, new List<string> { "Body_Eating", "Body_Eating_Expression 1", "Body_Eating_Expression 2", "Body_Eating_Expression 3", "Body_Happy 1", "Body_Happy 2", "Body_Happy 3", "Body_Idle", "Body_Sitting", "Body_Holding", "Body_Tap" } },
            { 1, new List<string> { "Hands_Idle", "Hands_Holding", "Hands_Eating", "Hands_Eating_Expression 1", "Hands_Eating_Expression 2", "Hands_Eating_Expression 3", "Hands_Happy 1", "Hands_Happy 2", "Hands_Happy 3", "Hands_Sitting", "Hands_Tap"} },
            { 2, new List<string> { "Legs_Idle", "Legs_Holding", "Legs_Eating" , "Legs_Eating_Expression 1", "Legs_Eating_Expression 2", "Legs_Eating_Expression 3", "Legs_Happy 1", "Legs_Happy 2", "Legs_Happy 3", "Legs_Sitting", "Legs_Tap" } }
        };
    }

    // Method to retrieve animation names for a specific layer
    public List<string> GetAnimationsForLayer(int layerIndex)
    {
        if (_animationLayers.ContainsKey(layerIndex))
        {
            return _animationLayers[layerIndex];
        }

        return null; // Return null if the layer does not exist
    }
}

