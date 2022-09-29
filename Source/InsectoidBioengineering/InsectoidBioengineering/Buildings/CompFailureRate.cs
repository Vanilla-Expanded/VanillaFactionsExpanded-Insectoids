using Verse;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsectoidBioengineering

{
    public class CompFailureRate : ThingComp
    {

      

        public CompProperties_FailureRate Props
        {
            get
            {
                return (CompProperties_FailureRate)this.props;
            }
        }

       

    }
}


