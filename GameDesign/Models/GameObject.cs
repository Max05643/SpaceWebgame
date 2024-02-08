using GameDesign.GameState;
using GameDesign.Models.Components;
using GameDesign.Models.Components.Interfaces;
using GameDesign.Utils;
using GameServerDefinitions;
using System.Collections.Generic;

namespace GameDesign.Models;


/// <summary>
/// Represents an object in game
/// </summary>
public class GameObject
{
    /// <summary>
    /// Rotation of the object in radians
    /// </summary>
    public float Angle { get; set; } = 0;

    /// <summary>
    /// Current game state manager
    /// </summary>
    public GameStateManager GameStateManager { get; set; }

    /// <summary>
    /// GameObject's unique identificator
    /// </summary>
    public virtual int Id { get; private set; }


    /// <summary>
    /// Parent object of this object. The object is positioned in parent's coordinate space.
    /// If null, object is positioned in global coordinate space
    /// </summary>
    public readonly GameObject? parent;


    /// <summary>
    /// Children of this object
    /// </summary>
    public IReadOnlyList<GameObject> Children => children;

    /// <summary>
    /// Children of this object
    /// </summary>
    private readonly List<GameObject> children = new List<GameObject>();

    /// <summary>
    /// Set to true when Destroy() is called
    /// </summary>
    public virtual bool IsDestroyed { get; protected set; } = false;

    /// <summary>
    /// The name of the object. Used for debug
    /// </summary>
    public virtual string Name { get; private set; }

    /// <summary>
    /// Object's position in 2d game space
    /// </summary>
    public virtual Vector2 Position => GetComponent<IPositionProvider>().Position;

    /// <summary>
    /// Object's velocity in 2d game space
    /// </summary>
    public virtual Vector2 Velocity
    {
        get
        {
            if (HasComponent<PhysicalComponent>())
            {
                return GetComponent<PhysicalComponent>().Velocity;
            }
            else
            {
                return Vector2.Zero;
            }
        }
    }
    

    /// <summary>
    /// All components attached to GameObject
    /// </summary>
    private readonly Dictionary<Type, Component> components = new Dictionary<Type, Component>();



    /// <summary>
    /// Adds new component to this GameObject wich will be indexed by a specified type
    /// </summary>
    public virtual void AttachComponent(Component component)
    {

        var currentType = component.GetType();

        bool result = components.TryAdd(currentType, component);

        if (!result)
            throw new ArgumentException($"GameObject {Name} already contains a component of type {(component.GetType())}");
    }




    /// <summary>
    /// Does GameObject have component attached?
    /// </summary>
    public virtual bool HasComponent<T>()
    {
        return components.Any(pair => pair.Key.IsAssignableTo(typeof(T)));
    }

    /// <summary>
    /// Returns the requested component. Throws if component is not present
    /// </summary>
    public virtual T GetComponent<T>()
    {
        if (typeof(T).IsSubclassOf(typeof(Component)) && components.TryGetValue(typeof(T), out Component? value))
        {
            return (T)(object)value;
        }
        else
        {

            var result = components.Keys.FirstOrDefault(key => key.IsAssignableTo(typeof(T)));

            if (result == null)
                throw new ArgumentException($"GameObject {Name} does not contain a component of type {typeof(T)}");
            else
                return (T)(object)components[result];
        }
    }


    /// <summary>
    /// Creates new GameObject with specified name
    /// </summary>
    /// <param name="name">Name of the object for debug</param>
    public GameObject(string name, GameStateManager gameStateManager, GameObject? parent = null)
    {
        GameStateManager = gameStateManager;
        Name = name;
        Id = gameStateManager.sceneManager.GetUniqueId();
        this.parent = parent;

        if (parent != null) //Register this object as another's child if necessary
        {
            parent.AddChild(this);
        }
    }

    /// <summary>
    /// Adds child to this component
    /// </summary>
    private void AddChild(GameObject child)
    {
        children.Add(child);
    }

    /// <summary>
    /// Removes child from this component
    /// </summary>
    private void RemoveChild(GameObject child)
    {
        children.Remove(child);
    }

    /// <summary>
    /// Does all cleanup that is needed to delete an object from the game
    /// </summary>
    public virtual void Destroy()
    {
        IsDestroyed = true;
        foreach (var component in components.Values)
        {
            component.Destroy();
        }

        foreach (var child in children.ToList()) //If this object is removed then all his children are removed as well
        {
            child.RemoveThisObject();
        }

        if (parent != null) //Remove this object as another's child if necessary
        {
            parent.RemoveChild(this);
        }
    }

    /// <summary>
    /// Called every frame before physical calculation
    /// </summary>
    /// <param name="deltaTime">Time between frames in seconds</param>
    public virtual void BeforePhysicalCalculation(float deltaTime, IPlayerInputProvider<PlayerInput> playerInputProvider)
    {
        foreach (var component in components.Values)
        {
            component.BeforePhysicalCalculation(deltaTime, playerInputProvider);
        }
    }

    /// <summary>
    /// Called every frame after physical calculation
    /// </summary>
    /// <param name="deltaTime">Time between frames in seconds</param>
    public virtual void AfterPhysicalCalculation(float deltaTime, IPlayerInputProvider<PlayerInput> playerInputProvider)
    {
        foreach (var component in components.Values)
        {
            component.AfterPhysicalCalculation(deltaTime, playerInputProvider);
        }
    }

}
