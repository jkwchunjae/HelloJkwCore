using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibra.Page;

public partial class SingleArmComponent : JkwPageBase
{
    [Parameter]
    public SingleArm Arm { get; set; }
}
