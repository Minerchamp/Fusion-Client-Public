using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Fusion.Networking
{
    [Serializable, Obfuscation]
    public class Movement
    {

        public enum MovementInfo
        {
            Right, Left, Forword, Backwards, Null
        }

        public MovementInfo Move = MovementInfo.Forword;

        public enum MovementInfo2
        {
            RightAndLeftBack, Zigzag, Line, FrontLine, AllLeftBack, LineBack, AllRightBack, HeadStack, LineRight, LineLeft, LeftAndRight, FrontLeftAndRight, Null
        }

        public MovementInfo2 Move2 = MovementInfo2.Line;

        public Movement(MovementInfo move, MovementInfo2 move2)
        {
            Move = move;
            Move2 = move2;
        }
    }
}
