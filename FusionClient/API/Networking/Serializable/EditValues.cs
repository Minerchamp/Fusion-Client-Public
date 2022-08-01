using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Fusion.Networking
{
    [Serializable, Obfuscation]
    public class EditValues
    {
        public float OrbitSpeed = 1f;

        public float BotDistance = 1f;

        public int SpinbotSpeed = 1;

        public EditValues(float speed, float distance, int spinbotspeed)
        {
            OrbitSpeed = speed;
            BotDistance = distance;
            SpinbotSpeed = spinbotspeed;
        }
    }
}
