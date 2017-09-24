using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RemoteDoors
{
    public static class InjectionEntry
    {
        /// <summary>
        /// Basically called after every update
        /// </summary>
        public static void OnEnergyConsumerSetConnectionStatus(EnergyConsumer energyConsumer)
        {
            if (!MaterialColor.State.ConfiguratorState.RemoteDoorsEnabled) return;

            var door = energyConsumer.GetComponent<Door>();

            if (door != null && energyConsumer.HasWire)
            {
                if (energyConsumer.IsPowered)
                {
                    if (door.IsOpen())
                    {
                        door.Close();
                    }

                    if (door.controlState != Door.ControlState.Closed)
                    {
                        door.controlState = Door.ControlState.Closed;
                        door.RefreshControlState();
                    }
                }
                else
                {
                    if (!door.IsOpen())
                    {
                        door.Open();
                    }

                    if (door.controlState != Door.ControlState.Opened)
                    {
                        door.controlState = Door.ControlState.Opened;
                        door.RefreshControlState();
                    }
                }
            }
        }
    }
}
