using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServerImplementation
{
    /// <summary>
    /// Creates instances of PlayerInputStorage implementations
    /// </summary>
    public interface IPlayerInputStorageFactory<PlayerInput>
    {
        PlayerInputStorage<PlayerInput> CreateNewStorage(IPlayerInputProcessor<PlayerInput> playerInputProcessor);
    }
}
