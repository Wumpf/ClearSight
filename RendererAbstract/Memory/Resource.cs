using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClearSight.RendererAbstract.Resources;

namespace ClearSight.RendererAbstract.Memory
{
    class Resource : DeviceChild
    {
        /// <summary>
        /// Heap on which this resource was created.
        /// If null the resource is either destroyed or was created using Device.CreateCommitedResource
        /// </summary>
        public Heap Heap { get; private set; } = null;


    }
}
