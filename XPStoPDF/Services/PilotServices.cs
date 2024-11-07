using Ascon.Pilot.SDK;
using System.ComponentModel.Composition;

namespace XPStoPDF.Services
{
    //public interface IPilotServices
    //{
    //    IObjectModifier Modifier { get; }
    //    IObjectsRepository Repository { get; }
    //    IPilotDialogService DialogService { get; }
    //    ITabServiceProvider TabService { get; }
    //    IAttributeFormatParser AttributeFormatParser { get; }
    //    ISearchService SearchService { get; }
    //    ITransitionManager TransitionManager { get; }
    //}

    [Export(typeof(PilotServices))]
    public class PilotServices //: IPilotServices
    {
        [ImportingConstructor]
        public PilotServices(IObjectModifier objectModifier,
                             IObjectsRepository objectsRepository,
                             IPilotDialogService pilotDialogService,
                             ITabServiceProvider tabServiceProvider,
                             ISearchService searchService,
                             IAttributeFormatParser attributeFormatParser,
                             ITransitionManager transitionManager,
                             IFileProvider fileProvider,
                             IPersonalSettings personalSettings,
                             IFileSaver fileSaver,
                             IXpsRender xpsRender,
                             IXpsMerger xpsMerger
                             )
        {
            Modifier = objectModifier;
            Repository = objectsRepository;
            DialogService = pilotDialogService;
            TabService = tabServiceProvider;
            AttributeFormatParser = attributeFormatParser;
            SearchService = searchService;
            TransitionManager = transitionManager;
            FileProvider = fileProvider;
            PersonalSettings = personalSettings;
            FileSaver = fileSaver;
            XpsRender = xpsRender;
            XpsMerger = xpsMerger;
    }

        public static IObjectModifier Modifier { get; private set; }
        public static IObjectsRepository Repository { get; private set; }
        public static IPilotDialogService DialogService { get; private set; }
        public static ITabServiceProvider TabService { get; private set; }
        public static IAttributeFormatParser AttributeFormatParser { get; private set; }
        public static ISearchService SearchService { get; private set; }
        public static ITransitionManager TransitionManager { get; private set; }
        public static IFileProvider FileProvider { get; private set; }
        public static IPersonalSettings PersonalSettings { get; private set; }
        public static IFileSaver FileSaver { get; private set; }
        public static IXpsRender XpsRender { get; private set; }
        public static IXpsMerger XpsMerger { get; private set; }
    }
}