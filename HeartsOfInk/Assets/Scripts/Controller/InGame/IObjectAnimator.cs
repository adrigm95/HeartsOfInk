using Assets.Scripts.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Controller.InGame
{
    public interface IObjectAnimator
    {
        void Animate();
        void EndAnimation();
    }
}
