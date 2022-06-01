using Microsoft.Xna.Framework.Input;
using System.ComponentModel.DataAnnotations;

namespace Hygenus
{
    class Controls
    {
        [Display(Order = 1)]
        public Keys Stop { get; set; }

        [Display(Order = 2)]
        public Keys Left { get; set; }

        [Display(Order = 3)]
        public Keys Right { get; set; }

    }
}
