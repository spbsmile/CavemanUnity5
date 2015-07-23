using Caveman.Setting;
using UnityEngine;

namespace Caveman.Network
{
    public class ServerMessage
    {
        private readonly JSONObject contentObject;

        public ServerMessage(string content)
        {
            if (content != "[]")
            {
                Debug.Log("from server " + content);    
            }
            contentObject = new JSONObject(content);
        }

        public void SendMessageToListener(IServerListener listener)
        {
            if (contentObject.IsArray)
            {
                foreach (var jsonItem in contentObject.list)
                {
                    SendMessageToListener(listener, jsonItem, jsonItem[ServerParams.ActionType].str);
                }
            }
            else
            {
                SendMessageToListener(listener, contentObject, contentObject[ServerParams.ActionType].str);
            }
        }


        private void SendMessageToListener(IServerListener listener, JSONObject action, string type)
        {
            var pointServer = (action[ServerParams.X] != null && action[ServerParams.Y] != null)
                ? new Vector2(action[ServerParams.X].f, action[ServerParams.Y].f)
                : Vector2.zero;
            var point = Convector(pointServer);
            var key = GenerateKey(point);
            var playerId = action[ServerParams.UserId]!= null ?action[ServerParams.UserId].str: null;
            if (type.Equals(ServerParams.StoneAddedAction))
            {
                listener.WeaponAddedReceived(key, point);
            }
            else if (type.Equals(ServerParams.StoneRemovedAction))
            {
                listener.WeaponRemovedReceived(key);
            }
            else if (type.Equals(ServerParams.MoveAction))
            {
                listener.MoveReceived(playerId, point);
            }
            else if (type.Equals(ServerParams.PickWeaponAction))
            {
                listener.PickWeaponReceived(playerId, key);
            }
            else if (type.Equals(ServerParams.BonusAddedAction))
            {
                listener.BonusAddedReceived(key, point);
            }
            else if (type.Equals(ServerParams.PickBonusAction))
            {
                listener.PickBonusReceived(playerId, key);
            }
            else if (type.Equals(ServerParams.UseWeaponAction))
            {                                        //aim
                listener.UseWeaponReceived(playerId, point);
            }
            else if (type.Equals(ServerParams.RespawnAction))
            {
                listener.RespawnReceived(playerId, point);
            } 
            //todo ServerParams.LoginAction LogoutAction &
            else if (type.Equals(ServerParams.LoginAction))
            {
                listener.LoginReceived(playerId);
            }
            else if (type.Equals(ServerParams.PlayerDeadAction))
            {
                listener.PlayerDeadResceived(playerId);
            }
        }

        private string GenerateKey(Vector2 point)
        {
            return point.x + ":" + point.y;
        }

        private Vector2 Convector(Vector2 point)
        {
            var x = (point.x / Multiplayer.HeigthMapServer) * Settings.HeightMap;
            var y = (point.y / Multiplayer.WidthMapServer) * Settings.WidthMap;
            return new Vector2(x, y);
        }
    }
}