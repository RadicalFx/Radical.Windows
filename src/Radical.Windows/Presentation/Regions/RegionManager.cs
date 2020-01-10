using Radical.Linq;
using Radical.Windows.Presentation.ComponentModel;
using System;
using System.Collections.Generic;

namespace Radical.Windows.Presentation.Regions
{
    /// <summary>
    /// Default region manager.
    /// </summary>
    public class RegionManager : IRegionManager
    {
        readonly IDictionary<string, IRegion> regions = new Dictionary<string, IRegion>();

        /// <summary>
        /// Registers the supplied region in this region manager.
        /// </summary>
        /// <param name="region">The region to register.</param>
        public void RegisterRegion( IRegion region )
        {
            if( regions.ContainsKey( region.Name ) )
            {
                throw new InvalidOperationException();
            }

            regions.Add( region.Name, region );
        }

        /// <summary>
        /// Gets the <see cref="Radical.Windows.Presentation.ComponentModel.IRegion"/> with the specified name.
        /// </summary>
        public IRegion this[ string name ]
        {
            get { return regions[ name ]; }
        }


        /// <summary>
        /// Gets the region registered with the given name.
        /// </summary>
        /// <param name="name">The name of the region.</param>
        /// <returns>
        /// The searched region, or an ArgumentOutOfRangeException if no region is registered with the given name.
        /// </returns>
        public IRegion GetRegion( string name )
        {
            return this[ name ];
        }

        /// <summary>
        /// Gets the region.
        /// </summary>
        /// <typeparam name="TRegion">The type of the region.</typeparam>
        /// <param name="name">The name.</param>
        /// <returns>
        /// The searched region, or an ArgumentOutOfRangeException if no region is registered with the given name.
        /// </returns>
        public TRegion GetRegion<TRegion>( string name ) where TRegion : IRegion
        {
            return ( TRegion )GetRegion( name );
        }

        /// <summary>
        /// Tries the get region.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="region">The region.</param>
        /// <returns></returns>
        public bool TryGetRegion( string name, out IRegion region )
        {
            return regions.TryGetValue( name, out region );
        }

        /// <summary>
        /// Tries to get the region.
        /// </summary>
        /// <typeparam name="TRegion">The type of the region.</typeparam>
        /// <param name="regionName">Name of the region.</param>
        /// <param name="region">The region.</param>
        /// <returns>
        ///   <c>True</c> if the region has been found; otherwise <c>false</c>.
        /// </returns>
        public bool TryGetRegion<TRegion>( string regionName, out TRegion region ) where TRegion : IRegion
        {
            if (TryGetRegion(regionName, out IRegion rg) && rg is TRegion)
            {
                region = (TRegion)rg;
                return true;
            }

            region = default( TRegion );
            return false;
        }

        /// <summary>
        /// Closes this region manager, the close process is invoked by the host at close time.
        /// </summary>
        public void Shutdown()
        {
            regions.Values.ForEach( r => r.Shutdown() );
            regions.Clear();
        }

        /// <summary>
        /// Gets all the registered the regions.
        /// </summary>
        /// <returns>
        /// All the registered the regions.
        /// </returns>
        public IEnumerable<IRegion> GetAllRegisteredRegions()
        {
            return regions.Values;
        }
    }
}
