using Radical.ComponentModel.Messaging;
using Radical.Messaging;
using Radical.Windows.Presentation.ComponentModel;

namespace Radical.Windows.Presentation.Messaging.Handlers
{
    class ViewLoadedHandler: AbstractMessageHandler<ViewLoaded>, INeedSafeSubscription
    {
        readonly IRegionInjectionHandler autoMappingHandler;
        readonly IViewResolver viewProvider;
        readonly IConventionsHandler conventions;
        readonly IRegionService regionService;

        public ViewLoadedHandler(IViewResolver viewProvider, IConventionsHandler conventions, IRegionService regionService, IRegionInjectionHandler autoMappingHandler)
        {
            this.viewProvider = viewProvider;
            this.conventions = conventions;
            this.regionService = regionService;
            this.autoMappingHandler = autoMappingHandler;
        }

        public override void Handle(object sender, ViewLoaded message )
        {
            var view = message.View;
            if ( regionService.HoldsRegionManager( view ) )
            {
                var manager = regionService.GetRegionManager( view );
                var regions = manager.GetAllRegisteredRegions();

                foreach ( var region in regions )
                {
                    var allViewTypes = autoMappingHandler.GetViewsInterestedIn( region.Name );

                    foreach ( var viewType in allViewTypes )
                    {
                        autoMappingHandler.Inject( 
                            ()=> viewProvider.GetView( viewType ), 
                            region );
                    }
                }
            }
        }
    }
}
