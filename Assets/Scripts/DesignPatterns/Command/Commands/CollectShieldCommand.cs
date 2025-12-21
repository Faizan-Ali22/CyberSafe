using UnityEngine;

/// <summary>
/// Collect Shield Command - Collects a shield pickup (for command-based collection tracking).
/// </summary>
// public class CollectShieldCommand : ICommand
// {
    // private MazeGameManager _gameManager;
    // private ShieldPickup _shieldPickup;
    // //private PoolableShield _poolableShield;
    // private bool _isPoolable;

    // /// <summary>
    // /// Creates a command for collecting a regular shield.
    // /// </summary>
    // public CollectShieldCommand(MazeGameManager gameManager, ShieldPickup shield)
    // {
    //     _gameManager = gameManager;
    //     _shieldPickup = shield;
    //     _isPoolable = false;
    // }

    /// <summary>
    /// Creates a command for collecting a poolable shield.
    /// </summary>
    // public CollectShieldCommand(MazeGameManager gameManager, PoolableShield shield)
    // {
    //     _gameManager = gameManager;
    //     _poolableShield = shield;
    //     _isPoolable = true;
    // }

    /// <summary>
    /// Executes the shield collection.
    /// </summary>
    // public void Execute()
    // {
    //     if (_gameManager == null) return;

    //     if (_isPoolable && _poolableShield != null)
    //     {
    //         _gameManager.OnPoolableShieldCollected(_poolableShield);
    //         Debug.Log("[CollectShieldCommand] Poolable shield collected via command.");
    //     }
    //     else if (!_isPoolable && _shieldPickup != null)
    //     {
    //         _gameManager.OnShieldCollected(_shieldPickup);
    //         Debug.Log("[CollectShieldCommand] Shield collected via command.");
    //     }
    // }

    /// <summary>
    /// Undo is not supported for shield collection.
    /// </summary>
    // public void Undo()
    // {
    //     Debug.Log("[CollectShieldCommand] Undo not supported for shield collection.");
    // }

    /// <summary>
    /// Returns whether the shield can be collected.
    /// </summary>
    // public bool CanExecute()
    // {
    //     if (_gameManager == null) return false;
        
    //     if (_isPoolable)
    //     {
    //         return _poolableShield != null && _poolableShield.gameObject.activeInHierarchy;
    //     }
    //     return _shieldPickup != null && _shieldPickup.gameObject.activeInHierarchy;
    // }
// }
