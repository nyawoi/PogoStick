using AetharNet.Moonbow.Experimental.Templates;
using AetharNet.Moonbow.Experimental.Utilities;
using Plukit.Base;
using Staxel.Client;
using Staxel.Core;
using Staxel.Logic;
using Staxel.Modding;

namespace AetharNet.PogoStick
{
    public class PogoStickHook : ModHookTemplate, IModHookV4
    {
        private Timestep _lastBounceTime;
        private bool _isBouncing;
        
        public override void UniverseUpdateBefore(Universe universe, Timestep step)
        {
            if (!GameUtilities.IsClient || GameUtilities.ClientMainLoop == null) return;

            var controller = GameUtilities.ClientMainLoop.AccessField<AvatarController>("_avatarController");
            var avatar = GameUtilities.ClientMainLoop.Avatar();
            
            if (controller == null || avatar == null) return;

            var activeItemStack = controller.ActiveItem(avatar);
            
            if (activeItemStack.IsNull() || activeItemStack.Item.Kind != "staxel.item.Whistle") return;

            if (controller.Controller.ControlState(ControlAxis.Jump).Down && !_isBouncing)
            {
                _isBouncing = true;
            }

            if (!controller.Physics.StandingPosition.HasValue) return;
            
            if (controller.Controller.ControlState(ControlAxis.Sneak).Down && _isBouncing)
            {
                _isBouncing = false;
            }
            else if (_isBouncing && step.Step - _lastBounceTime.Step > Constants.PlayerJumpGracePeriod)
            {
                controller.Physics.AddForce(new Vector3D(0.0d, Constants.PlayerJumpForce * 1.5d, 0.0d));
                _lastBounceTime = step;
            }
        }
    }
}