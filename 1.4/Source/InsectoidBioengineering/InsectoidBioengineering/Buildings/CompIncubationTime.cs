using Verse;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsectoidBioengineering

{
    public class CompIncubationTime : ThingComp
    {

      

        public CompProperties_IncubationTime Props
        {
            get
            {
                return (CompProperties_IncubationTime)this.props;
            }
        }

       

    }
}


