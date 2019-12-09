﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Configuration;

namespace Radical.Windows.Presentation
{
    static class Tracing
    {
        static readonly object syncRoot = new object();
        static TraceSource _source;

        public static TraceSource Source 
        {
            get
            {
                if( _source == null ) 
                {
                    lock( syncRoot ) 
                    {
                        if( _source == null )
                        {
                            var name =  ConfigurationManager
                                .AppSettings[ "radical/windows/presentation/diagnostics/defaultTraceSourceName" ]
                                .Return( s => s, "Radical.Windows.Presentation" );

                            _source = new TraceSource( name );
                        }
                    }
                }

                return _source;
            }
        }
    }
}
